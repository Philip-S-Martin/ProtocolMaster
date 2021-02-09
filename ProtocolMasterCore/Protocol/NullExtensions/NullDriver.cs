using ProtocolMasterCore.Protocol.Driver;
using System.Collections.Generic;

namespace ProtocolMasterCore.Protocol.NullExtensions
{
    [DriverMeta("No Driver", "")]
    public class NullDriver : IDriver
    {
        public bool IsCanceled { get; set; }


        public bool Setup(List<ProtocolEvent> dataList)
        {
            return false;
        }

        public void Start()
        {
        }
    }
}
