using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProtocolMasterWPF.Model
{
    public class NoFileSelection : Streamer
    {
        public override Stream StartStream()
        {
            return null;
        }
        public override string ToString()
        {
            return "No File Selected";
        }
    }
}
