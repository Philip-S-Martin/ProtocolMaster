using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolMasterWPF.Helpers
{
    public class DeviceInformationEmpty
    {
        public static DeviceInformationEmpty NullEquivalent { get => _nullEquivalent; }
        static DeviceInformationEmpty _nullEquivalent = new DeviceInformationEmpty();
        public string Name { get => "None Selected"; }
    }
}
