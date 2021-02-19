using System;
using System.Collections.Generic;

namespace ProtocolMasterCore.Protocol.Driver
{
    public delegate void DriverTimeEvent();
    public class DriverManager : ExtensionManager<IDriver, DriverMeta>
    {
        IDriver driver;
        public DriverTimeEvent OnProtocolStart;
        public DriverTimeEvent OnProtocolEnd;
        internal bool Run(List<ProtocolEvent> data)
        {
            bool didStart = false;
            driver = CreateSelectedExtension();
            if (driver.Setup(data))
            {
                didStart = true;
                OnProtocolStart?.Invoke();
                driver.Start();
                OnProtocolEnd?.Invoke();
            }
            DisposeSelectedExtension();
            return didStart;
        }
    }
}
