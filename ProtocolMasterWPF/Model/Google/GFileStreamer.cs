using Google.Apis.Drive.v3.Data;
using ProtocolMasterCore.Utility;
using System;
using System.IO;
using File = Google.Apis.Drive.v3.Data.File;

namespace ProtocolMasterWPF.Model.Google
{
    public class GFileStreamer : IStreamStarter
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

        public Stream StartStream()
        {
            Stream result;
            try
            {
                result = GDrive.Instance.Download(ID);
            }
            catch (Exception e)
            {
                result = null;
                Log.Error($"Could not open GDrive File {Name}, Exception: {e}");
            }
            return result;
        }
    }
}
