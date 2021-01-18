using System.Collections.Generic;

namespace ProtocolMasterCore.Protocol.Interpreter
{
    public interface IInterpreter : IExtension
    {
        List<ProtocolEvent> Generate(string protocolName);
    }
}
