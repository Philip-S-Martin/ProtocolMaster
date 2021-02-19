using System;
using System.Collections.Generic;
using System.IO;

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
        string documents;
        string assembly;

        private AppEnvironment()
        {
            locations = new Dictionary<string, string>();
            appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ProtocolMaster");
            documents = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ProtocolMaster");
            assembly = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            Directory.CreateDirectory(appData);
            Directory.CreateDirectory(documents);
            Directory.CreateDirectory(assembly);
        }
        public static bool TryAddLocationAppData(string label, string subPath, out string fullPath) 
            => instance.TryAddLocation(label, instance.appData, subPath, out fullPath);
        public static bool TryAddLocationDocuments(string label, string subPath, out string fullPath) 
            => instance.TryAddLocation(label, instance.documents, subPath, out fullPath);
        public static bool TryAddLocationAssembly(string label, string subPath, out string fullPath) 
            => instance.TryAddLocation(label, instance.assembly, subPath, out fullPath);
        private bool TryAddLocation(string label, string basePath, string subPath, out string fullPath)
        {
            fullPath = Path.Combine(basePath, subPath);
            if (locations.TryAdd(label, fullPath))
            {
                Directory.CreateDirectory(fullPath);
                return true;
            }
            else return false;
        }

        public static bool TryGetLocation(string label, out string fullPath) => instance.I_TryGetLocation(label, out fullPath);
        private bool I_TryGetLocation(string label, out string fullPath) => locations.TryGetValue(label, out fullPath);

        public static string GetLocation(string label) => instance.I_GetLocation(label);
        private string I_GetLocation(string label) => locations[label];

    }
}
