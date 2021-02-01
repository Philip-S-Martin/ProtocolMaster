using ProtocolMasterCore.Protocol;
using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProtocolMasterWPF.Model
{
    internal enum SessionState
    {
        NotReady,
        Selecting,
        Ready,
        Running,
        Viewing,
    }
    internal class Session
    {
        string extensionDir;
        private InterpretAndDriveProtocol Protocol { get; set; }
        public List<IExtensionMeta> Options { get; private set; }
        public Session()
        {
            AppEnvironment.TryAddLocationAssembly("Extensions", "Extensions", out extensionDir);
            Protocol = new InterpretAndDriveProtocol(extensionDir);
            Protocol.DriverManager.OnOptionsLoaded += LoadOptions;
        }
        private void LoadOptions(List<IExtensionMeta> options)
        {
            Options = options;
        }
        public void Preview()
        {
            Protocol.LoadExtensions();
        }
        public void Start()
        {

        }
        public void Stop()
        {

        }
        public void Reset()
        {

        }
    }
}
