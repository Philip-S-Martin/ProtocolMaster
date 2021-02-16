using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ProtocolMasterWPF.Model
{
    internal class WebFileStreamer : IStreamStarter
    {
        public string Name { get => WebFile.Name; }
        public PublishedWebFile WebFile { get; private set; }
        public Stream StartStream()
        {
            WebClient webClient = new WebClient();
            Stream result;
            try
            {
                result = webClient.OpenRead(WebFile.URL);
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
