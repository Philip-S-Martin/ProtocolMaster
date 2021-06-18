using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProtocolMasterWPF.Model
{
    internal class LocalFileStreamer : IStreamStarter
    {
        public string Name { get => Path.GetFileNameWithoutExtension(LocalFile.FullName); }
        public FileInfo LocalFile { get; private set; }
        public LocalFileStreamer(FileInfo source)
        {
            LocalFile = source;
        }
        public Stream StartStream()
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
    }
}
