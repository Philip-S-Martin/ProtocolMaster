using Google.Apis.Drive.v3.Data;
using System.IO;
using File = Google.Apis.Drive.v3.Data.File;

namespace ProtocolMasterWPF.Model.Google
{
    public class GFileData : IStreamStarter
    {
        public string Name { get => GFile.Name; }
        public string ID { get => GFile.Id; }
        public File GFile { get; private set; }
        public GFileData(File gfile)
        {
            GFile = gfile;
        }
        public override string ToString()
        {
            return Name;
        }

        public Stream StartStream()
        {
            return GDrive.Instance.Download(ID);
        }
    }
}
