using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterWPF.Model
{
    internal class LocalFileStreamer : Streamer
    {
        public string Name { get => Path.GetFileNameWithoutExtension(LocalFile.FullName); }
        public FileInfo LocalFile { get; private set; }
        public LocalFileStreamer(FileInfo source)
        {
            LocalFile = source;
        }
        public override Stream StartStream()
        {
            Stream result;
            try
            {
                result = LocalFile.Open(FileMode.Open);
            }
            catch (Exception e)
            {
                Log.Error($"Could not open Local File {Name}, Exception: {e}");
                result = null;
            }
            return result;
        }
        public override string ToString()
        {
            return Name;
        }
        public Task Delete()
        {
            var task = Task.Run(() => LocalFile.Delete());
            task.ContinueWith(
                (task) => LocalFileStore.Instance.RefreshFiles(),
                TaskScheduler.FromCurrentSynchronizationContext());
            return task;
        }
        public Task Open()
        {
            var task = Task.Run(() => App.TryOpenURI(this, LocalFile.FullName));
            return task;
        }
    }
}
