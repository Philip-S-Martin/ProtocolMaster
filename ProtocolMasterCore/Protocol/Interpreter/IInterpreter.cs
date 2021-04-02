using System.Collections.Generic;

namespace ProtocolMasterCore.Protocol.Interpreter
{
    public interface IInterpreter : IExtension
    {
        string ProtocolLabel { get; }
        List<ProtocolEvent> Generate(string argument);
    }
}
