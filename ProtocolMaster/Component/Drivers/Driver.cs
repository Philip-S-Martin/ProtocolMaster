using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Drivers
{
    public abstract class Driver
    {
        private SerialPort serialPort;
        private string targetPort;
        protected SerialPort SerialConnection { get => serialPort; }

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
                if(SerialConnection.IsOpen) SerialConnection.Close();
            }
        }
        protected virtual void Setup(){}

        protected async virtual Task Loop(){}

        protected virtual void Exit(){}
    }
}
