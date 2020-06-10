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
    internal class DriverManager : IExtensionManager<IDriver, DriverMeta>
    {
        IDriver driver;

        public event EventHandler OnProtocolStart;
        protected void ProtocolStart()
        {
            OnProtocolStart?.Invoke(this, new EventArgs());
        }
        public async void Run(List<DriveData> data)
        {
            driver = CreateSelectedExtension();

            driver.Setup(data);
            UIDispatcher.Invoke(() => { ProtocolStart(); }, DispatcherPriority.Send);
            driver.Start();

            DisposeSelectedExtension();
        }
    }
}
