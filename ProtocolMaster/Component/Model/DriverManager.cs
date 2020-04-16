using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model
{
    internal interface IDriverManager
    {
        void Print();
    }

    [Export(typeof(IDriverManager))]
    internal class DriverManager : IDriverManager
    {
        [ImportMany]
        IEnumerable<Lazy<IDriver, IExtensionData>> _drivers;
        // Driver thread management
        public void Print()
        {
            foreach (Lazy<IDriver, IExtensionData> i in _drivers)
            {
                App.Window.Timeline.ListDriver(i.Metadata.Symbol[0]);
                Log.Error("Driver found: " + i.Metadata.Symbol[0]);
            }
        }
        

        // Old code from when DriverManager was really more of a SerialDriverManager
        /*
        public static bool TryAdd<T>(string targetPort) where T : Driver, new()
        {
            Driver.Start<T>(targetPort);
            return true;
        }
        

        public static string[] AvailablePorts()
        {
            List<string> available = new List<string>();
            foreach(KeyValuePair<string, bool> port in allPorts)
            {
                // try to connect to each and identify it
            }
            throw new NotImplementedException();
        }
        */


        // Old code from Driver.cs, just reference for threading stuff
        /*
        public static void Start<T>(string target) where T : Driver, new()
        {
            Task startTask = new Task(() =>
            {
                T driver = new T
                {
                    serialPort = new SerialPort(target),
                    targetPort = target
                };
                driver.Run();
            });
            startTask.Start();
        }
        private async void Run()
        {
            Setup();
            if (SerialConnection.IsOpen)
            {
                if (SerialConnection.PortName == targetPort)
                {
                    while (DriverManager.currentPorts.Contains(serialPort.PortName))
                    {
                        await Loop();
                    }
                    Exit();
                }
                else
                {
                    DriverManager.currentPorts.TryTake(out targetPort);
                }
                if (SerialConnection.IsOpen) SerialConnection.Close();
            }
        }
        */
    }
}
