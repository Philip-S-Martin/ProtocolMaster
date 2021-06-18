using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ProtocolMasterWPF.Model
{
    internal class PublishedFileStreamer : IStreamStarter
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public Stream StartStream()
        {
            WebClient webClient = new WebClient();
            Stream result;
            try
            {
                string filepath = Path.Combine(PublishedFileStore.Instance.Directory, "PubCache.file");
                webClient.DownloadFile(URL, filepath);
                FileInfo file = new FileInfo(filepath);
                result = file.Open(FileMode.Open);
            }
            catch(Exception e)
            {
                result = null;
                Log.Error($"Could not open Web File {Name}, Exception: {e}");
            }
            return result;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
