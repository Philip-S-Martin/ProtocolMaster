using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ProtocolMaster.Component.Model.Driver
{
    internal interface IDriverManager
    {
        void Load();
        void Select(DriverMeta target);
        DriverMeta Selected { get; }
        void Run(List<DriveData> data, Progress<DriverProgress> progress);
        void Cancel();
        bool IsRunning { get; }
    }

    [Export(typeof(IDriverManager))]
    internal class DriverManager : IDriverManager
    {
        ExportFactory<IDriver, DriverMeta> driverFactory;
        ExportLifetimeContext<IDriver> driverContext;
        IDriver driver;
        public bool IsRunning { get; private set; }

        [ImportMany]
        private IEnumerable<ExportFactory<IDriver, DriverMeta>> Drivers { get; set; }

        public DriverMeta Selected { get { return driverFactory.Metadata; } }

        public void Load()
        {
            foreach (ExportFactory<IDriver, DriverMeta> i in Drivers)
            {
                App.Window.Timeline.ListDriver(i.Metadata);
                if (i.Metadata.Name == "None" && i.Metadata.Version == "")
                {
                    Select(i.Metadata);
                }
                Log.Error("Driver found: '" + i.Metadata.Name + "' version: '" + i.Metadata.Version + "'");
            }
            App.Window.Timeline.ShowSelectedDriver();
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

        public void Run(List<DriveData> data, Progress<DriverProgress> progress)
        {
            driverContext = driverFactory.CreateExport();
            driver = driverContext.Value;
            driver.CurrentProgress = progress;

            IsRunning = true;
            // pre-fill event data
            driver.ProcessData(data);
            // Loop through driver
            App.Current.Dispatcher.Invoke(() => App.Window.Timeline.StartAnimation());
            driver.Run();
            IsRunning = false;

            driverContext.Dispose();
        }

        public void Cancel()
        {
            Log.Error("CANCELLATION REQUESTED");
            driver.Cancel();
            IsRunning = false;
        }

        public List<DriveData> TestData_A()
        {
            return new List<DriveData>()
            {
                new DriveData("DigitalDuration",
                    new KeyValuePair<string, string>("SignalPin", "4"),
                    new KeyValuePair<string, string>("DurationPin", "5"),
                    new KeyValuePair<string, string>("TimeStartMs", "1000"),
                    new KeyValuePair<string, string>("TimeEndMs", "2000")),
                new DriveData("DigitalDuration",
                    new KeyValuePair<string, string>("SignalPin", "6"),
                    new KeyValuePair<string, string>("DurationPin", "7"),
                    new KeyValuePair<string, string>("TimeStartMs", "1500"),
                    new KeyValuePair<string, string>("TimeEndMs", "2500")),
                new DriveData("DigitalStringDuration",
                    new KeyValuePair<string, string>("SignalRange", "8:11"),
                    new KeyValuePair<string, string>("DurationPin", "12"),
                    new KeyValuePair<string, string>("Value", "1"),
                    new KeyValuePair<string, string>("TimeStartMs", "1000"),
                    new KeyValuePair<string, string>("TimeEndMs", "1500")),
                new DriveData("DigitalStringDuration",
                    new KeyValuePair<string, string>("SignalRange", "8:11"),
                    new KeyValuePair<string, string>("DurationPin", "12"),
                    new KeyValuePair<string, string>("Value", "2"),
                    new KeyValuePair<string, string>("TimeStartMs", "1500"),
                    new KeyValuePair<string, string>("TimeEndMs", "2000")),
                new DriveData("DigitalStringDuration",
                    new KeyValuePair<string, string>("SignalRange", "8:11"),
                    new KeyValuePair<string, string>("DurationPin", "12"),
                    new KeyValuePair<string, string>("Value", "3"),
                    new KeyValuePair<string, string>("TimeStartMs", "2000"),
                    new KeyValuePair<string, string>("TimeEndMs", "2500")),
                new DriveData("DigitalStringDuration",
                    new KeyValuePair<string, string>("SignalRange", "8:11"),
                    new KeyValuePair<string, string>("DurationPin", "12"),
                    new KeyValuePair<string, string>("Value", "4"),
                    new KeyValuePair<string, string>("TimeStartMs", "2500"),
                    new KeyValuePair<string, string>("TimeEndMs", "3000")),
                new DriveData("DigitalStringDuration",
                    new KeyValuePair<string, string>("SignalRange", "8:11"),
                    new KeyValuePair<string, string>("DurationPin", "12"),
                    new KeyValuePair<string, string>("Value", "5"),
                    new KeyValuePair<string, string>("TimeStartMs", "3000"),
                    new KeyValuePair<string, string>("TimeEndMs", "3500")),
                new DriveData("DigitalStringDuration",
                    new KeyValuePair<string, string>("SignalRange", "8:11"),
                    new KeyValuePair<string, string>("DurationPin", "12"),
                    new KeyValuePair<string, string>("Value", "6"),
                    new KeyValuePair<string, string>("TimeStartMs", "3500"),
                    new KeyValuePair<string, string>("TimeEndMs", "4000")),
                new DriveData("DigitalStringDuration",
                    new KeyValuePair<string, string>("SignalRange", "8:11"),
                    new KeyValuePair<string, string>("DurationPin", "12"),
                    new KeyValuePair<string, string>("Value", "7"),
                    new KeyValuePair<string, string>("TimeStartMs", "4000"),
                    new KeyValuePair<string, string>("TimeEndMs", "4500"))

            };
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
