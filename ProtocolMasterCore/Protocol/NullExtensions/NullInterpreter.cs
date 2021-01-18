using ProtocolMasterCore.Protocol.Interpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterCore.Protocol.NullExtensions
{
    [InterpreterMeta("None", "")]
    public class NullInterpreter : IInterpreter
    {
        public bool IsCanceled { get; set; }
        public List<ProtocolEvent> Generate(string protocolName)
        {
            return null;
        }
    }


}
