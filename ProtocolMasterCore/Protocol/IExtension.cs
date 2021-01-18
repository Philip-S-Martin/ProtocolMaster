using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterCore.Protocol
{
    public interface IExtension
    {
        bool IsCanceled { get; set; }
    }
}
