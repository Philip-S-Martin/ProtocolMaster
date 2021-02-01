using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProtocolMasterWPF.Model
{
    public interface IStreamStarter
    {
        public Stream StartStream();
    }
}
