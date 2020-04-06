using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace ProtocolMaster.Component.Model
{
    public class DriverManager
    {
        [ImportMany]
        IEnumerable<Lazy<IDriver, IDriverData>> _drivers;

        private CompositionContainer _container;

        public DriverManager()
        {
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(DriverManager).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Driver"));
            _container = new CompositionContainer(catalog);
            Log.Error("Drivers Location: " + Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Driver");

            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Log.Error(compositionException.ToString());
            }

            foreach (Lazy<IDriver, IDriverData> i in _drivers)
            {
                App.Window.Timeline.ListDriver(i.Metadata.Symbol);
                Log.Error("Driver found: " + i.Metadata.Symbol);
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
