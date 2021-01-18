using System;
using System.Collections.Generic;

namespace ProtocolMasterCore.Protocol.Driver
{
    internal class DriverManager : ExtensionManager<IDriver, DriverMeta>
    {
        IDriver driver;
        public event EventHandler OnProtocolStart;
        public event EventHandler OnProtocolEnd;
        public bool Run(List<ProtocolEvent> data)
        {
            bool didStart = false;
            driver = CreateSelectedExtension();
            if (driver.Setup(data))
            {
                didStart = true;
                OnProtocolStart?.Invoke(this, new EventArgs());
                driver.Start();
                OnProtocolEnd?.Invoke(this, new EventArgs());
            }
            DisposeSelectedExtension();
            return didStart;
        }
    }
}
