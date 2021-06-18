using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterWPF.Model
{
    internal class PublishedFileStreamer : Streamer
    {
        private string name;
        public string Name
        {
            get => name; set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        private string url;
        public string URL
        {
            get => url; set
            {
                url = value;
                OnPropertyChanged();
            }
        }
        public override Stream StartStream()
        {
            WebClient webClient = new WebClient();
            Stream result;
            try
            {
                string filepath = Path.Combine(PublishedFileStore.Instance.Directory, "PubCache.file");
                Uri uri = new Uri(URL);
                webClient.DownloadFile(uri, filepath);
                FileInfo file = new FileInfo(filepath);
                result = file.Open(FileMode.Open);
            }
            catch (Exception e)
            {
                result = null;
                Log.Error($"Could not open web file: (name: {Name}), (url: {URL})\nException: {e}");
            }
            return result;
        }
        public Task DownloadToLocal(string extension)
        {
            var task = Task.Run(() =>
            {
                WebClient webClient = new WebClient();
                try
                {
                    string dir = LocalFileStore.Instance.Directory;
                    string filepath = Path.Combine(dir, $"{name} (Pub).{extension}");
                    int i = 1;
                    while (File.Exists(filepath))
                        filepath = Path.Combine(dir, $"{name} (Pub) ({i++}).{extension}");
                    webClient.DownloadFile(URL, filepath);
                    FileInfo file = new FileInfo(filepath);
                }
                catch (Exception e)
                {
                    Log.Error($"Could not open Web File {Name}, Exception: {e}");
                }
            });
            task.ContinueWith(
                (task) => LocalFileStore.Instance.RefreshFiles(),
                TaskScheduler.FromCurrentSynchronizationContext());
            return task;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
