using ProtocolMasterCore.Protocol;
using ProtocolMasterCore.Protocol.Driver;
using ProtocolMasterCore.Protocol.Interpreter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ProtocolMasterWPF.Model
{
    [Serializable]
    public sealed class ExtensionMetaSetting : IExtensionMeta
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public ExtensionMetaSetting() { }
        public ExtensionMetaSetting(IExtensionMeta from)
        {
            Name = from.Name; Version = from.Version;
        }
        public bool Equals([AllowNull] IExtensionMeta other)
        {
            return this.Name == other.Name && this.Version == other.Version;
        }
    }
}
