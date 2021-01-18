using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterCore.Protocol
{
    interface IExtensionMeta
    {
        string Name { get; }
        string Version { get; }
    }
}
