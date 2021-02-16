using System;

namespace ProtocolMasterCore.Protocol
{
    public interface IExtensionMeta : IEquatable<IExtensionMeta>
    {
        string Name { get; }
        string Version { get; }
    }
}
