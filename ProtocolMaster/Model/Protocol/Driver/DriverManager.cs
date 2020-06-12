using ProtocolMaster.Model.Debug;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ProtocolMaster.Model.Protocol.Driver
{
    internal class DriverManager : ExtensionManager<IDriver, DriverMeta>
    {
        IDriver driver;

        public event EventHandler OnProtocolStart;
        public event EventHandler OnProtocolEnd;
        protected void ProtocolStart()
        {
            OnProtocolStart?.Invoke(this, new EventArgs());
        }
        protected void ProtocolEnd()
        {
            OnProtocolEnd?.Invoke(this, new EventArgs());
        }
        public void Run(List<ProtocolEvent> data)
        {
            driver = CreateSelectedExtension();

            driver.Setup(data);
            UIDispatcher.Invoke(() => { ProtocolStart(); }, DispatcherPriority.Send);
            driver.Start();
            UIDispatcher.Invoke(() => { ProtocolEnd(); }, DispatcherPriority.Send);

            DisposeSelectedExtension();
        }
    }
}
