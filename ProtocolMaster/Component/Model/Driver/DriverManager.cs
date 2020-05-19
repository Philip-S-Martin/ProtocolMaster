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
        void Select(DriverMeta target);
        void Run();
    }

    [Export(typeof(IDriverManager))]
    internal class DriverManager : IDriverManager
    {
        ExportFactory<IDriver, DriverMeta> driverFactory;
        ExportLifetimeContext<IDriver> driverContext;
        IDriver driver;

        private Task runTask;
        private CancellationToken cancelToken;
        private CancellationTokenSource tokenSource;

        [ImportMany]
        private IEnumerable<ExportFactory<IDriver, DriverMeta>> Drivers { get; set; }

        // Driver thread management
        public void Print()
        {
            foreach (ExportFactory<IDriver, DriverMeta> i in Drivers)
            {
                App.Window.Timeline.ListDriver(i.Metadata);
                Log.Error("Driver found: " + i.Metadata.Name + " version " + i.Metadata.Version);
            }
        }

        public void Select(DriverMeta target)
        {
            foreach (ExportFactory<IDriver, DriverMeta> i in Drivers)
            {
                if (i.Metadata == target)
                {
                    driverFactory = i;
                }
            }
        }

        public void Run()
        {
            driverContext = driverFactory.CreateExport();
            driver = driverContext.Value;

            List<DriveData> data = new List<DriveData>();

            tokenSource = new CancellationTokenSource();
            cancelToken = tokenSource.Token;

            runTask = Task.Run(new Action(() =>
            {
                // Register driver cancel
                cancelToken.Register(new Action(() => driver.Cancel()));
                // pre-fill event data
                driver.ProcessData(data);
                // Loop through driver
                driver.Run();
            }), tokenSource.Token);

            driverContext.Dispose();
        }

        public void Cancel()
        {
            tokenSource.Cancel();
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
