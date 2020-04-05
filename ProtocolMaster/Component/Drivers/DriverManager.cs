using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Drivers
{
    class DriverManager
    {
        public static ConcurrentBag<string> currentPorts = new ConcurrentBag<string>();
        private static ConcurrentDictionary<string, bool> allPorts = new ConcurrentDictionary<string, bool>();
        public static bool TryAdd<T>(string targetPort) where T : Driver, new()
        {
            
            Driver.Start<T>(targetPort);
            return true;
        }

        public static bool TryRemove(string targetPort)
        {
            throw new NotImplementedException();
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
    }
}
