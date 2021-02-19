using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Serialization;

namespace ProtocolMasterCore.Protocol
{
    public interface IExtensionMeta : IEquatable<IExtensionMeta>
    {
        string Name { get; }
        string Version { get; }
    }
    
}
