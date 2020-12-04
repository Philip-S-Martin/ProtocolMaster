using ProtocolMaster.Model.Protocol;
using ProtocolMaster.Model.Protocol.Driver;
using ProtocolMaster.Model.Debug;
using System.ComponentModel.Composition;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http.Headers;
using System.Collections.Concurrent;
using System;
using System.Xml;
using System.Diagnostics;

namespace ProtocolMaster.Model.Protocol.NullExtensions
{
    [DriverMeta("None", "")]
    public class NullDriver : IDriver
    {
        public void Cancel()
        {
        }

        public bool Setup(List<ProtocolEvent> dataList)
        {
            return false;
        }

        public void Start()
        {
        }
    }
}
