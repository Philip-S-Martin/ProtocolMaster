using ProtocolMaster.Component.Model;
using ProtocolMaster.Component.Model.Driver;
using ProtocolMaster.Component.Debug;
using System.ComponentModel.Composition;
using System.IO.Ports;
using System.Collections.Generic;
using System.Threading;
using System.Net.Http.Headers;
using System.Collections.Concurrent;
using System;
using System.Xml;
using System.Diagnostics;

namespace ProtocolMaster.Component.Model.NullExtensions
{
    [DriverMeta("None", "")]
    public class NullDriver : IDriver
    {
        public ConcurrentQueue<VisualData> VisualData { get { return null; } }
        public Progress<DriverProgress> CurrentProgress { get; set; }
        public void Cancel()
        {
        }

        public void ProcessData(List<DriveData> dataList)
        {
        }

        public void Run()
        {
        }
    }
}
