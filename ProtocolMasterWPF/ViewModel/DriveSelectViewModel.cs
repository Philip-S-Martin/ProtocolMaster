using ProtocolMasterWPF.Model.Google;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolMasterWPF.ViewModel
{
    public class DriveSelectViewModel
    {
        public GAuth Auth { get => GAuth.Instance; }
        public GDrive Drive { get => GDrive.Instance; }

        public DriveSelectViewModel()
        {
            
        }
    }
}
