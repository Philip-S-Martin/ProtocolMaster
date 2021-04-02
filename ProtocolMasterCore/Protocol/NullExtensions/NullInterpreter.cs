using ProtocolMasterCore.Protocol.Interpreter;
using System.Collections.Generic;

namespace ProtocolMasterCore.Protocol.NullExtensions
{
    [InterpreterMeta("None Selected", "")]
    public class NullInterpreter : IInterpreter
    {
        public bool IsCanceled { get; set; }

        public string ProtocolLabel => "No Protocol";

        public List<ProtocolEvent> Generate(string protocolName)
        {
            return null;
        }
    }


}
