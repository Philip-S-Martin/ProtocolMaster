using Google.Apis.Drive.v3.Data;
using ProtocolMasterCore.Utility;
using System;
using System.IO;
using System.Threading.Tasks;
using File = Google.Apis.Drive.v3.Data.File;

namespace ProtocolMasterWPF.Model.Google
{
    public class GFileStreamer : Streamer
    {
        public string Name { get => GFile.Name; }
        public string ID { get => GFile.Id; }
        public File GFile { get; private set; }
        public GFileStreamer(File gfile)
        {
            GFile = gfile;
        }
        public override string ToString()
        {
            return Name;
        }
        public bool IsSheet
        {
            get => GFile.MimeType == "application / vnd.google - apps.spreadsheet" || GFile.MimeType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        public override Stream StartStream()
        {
            Stream result;
            try
            {
                result = GDrive.Instance.StreamFile(ID);
            }
            catch (Exception e)
            {
                result = null;
                Log.Error($"Could not open GDrive File {Name}, Exception: {e}");
            }
            return result;
        }
        public Task PublishAndLink()
        {
            var task = Task.Run(() =>
            {
                GDrive.Instance.Publish(GFile);
            });
            task.ContinueWith(
                (task) => PublishedFileStore.Instance.Add($"{Name}", $"https://docs.google.com/spreadsheets/d/{ID}/pub?output=xlsx"),
                TaskScheduler.FromCurrentSynchronizationContext());
            return task;
        }
        public void OpenInBrowser()
        {
            App.TryOpenURI(this, $"https://docs.google.com/spreadsheets/d/{ID}/edit?usp=sharing");
        }
        public Task DownloadToLocal(string extension)
        {
            var task = Task.Run(() =>
            {
                Stream input = GDrive.Instance.StreamFile(ID);
                string dir = LocalFileStore.Instance.Directory;
                string filepath = Path.Combine(dir, $"{Name}.{extension}");
                int i = 1;
                while (System.IO.File.Exists(filepath))
                    filepath = Path.Combine(dir, $"{Name} ({i++}).{extension}");
                FileInfo file = new FileInfo(filepath);
                FileStream output = file.OpenWrite();
                input.CopyTo(output);
                input.Close();
                output.Close();
            });
            task.ContinueWith(
                (task) => LocalFileStore.Instance.RefreshFiles(),
                TaskScheduler.FromCurrentSynchronizationContext());
            return task;
        }
    }
}
