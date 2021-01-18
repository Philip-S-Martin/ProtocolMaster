using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterCore.Utility
{
    public sealed class AppEnvironment
    {
        private static readonly AppEnvironment instance = new AppEnvironment();
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AppEnvironment()
        {
        }

        Dictionary<string, string> locations;
        string appData;
        string assembly;

        private AppEnvironment()
        {
            locations = new Dictionary<string, string>();
            appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ProtocolMaster";
            assembly = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            Directory.CreateDirectory(appData);
            Directory.CreateDirectory(assembly);
        }
        public static bool TryAddLocationAppData(string label, string subPath, out string fullPath) => instance.I_TryAddLocationAppData(label, subPath, out fullPath);
        private bool I_TryAddLocationAppData(string label, string subPath, out string fullPath)
        {
            fullPath = Path.Combine(appData, subPath);
            if (locations.TryAdd(label, fullPath))
            {
                Directory.CreateDirectory(fullPath);
                return true;
            }
            else return false;
        }
        public static bool IryAddLocationAssembly(string label, string subPath, out string fullPath) => instance.I_TryAddLocationAssembly(label, subPath, out fullPath);
        private bool I_TryAddLocationAssembly(string label, string subPath, out string fullPath)
        {
            fullPath = Path.Combine(assembly, subPath);
            if (locations.TryAdd(label, fullPath))
            {
                Directory.CreateDirectory(fullPath);
                return true;
            }
            else return false;
        }
    }
}
