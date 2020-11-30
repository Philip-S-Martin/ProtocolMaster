using ProtocolMaster.Model.Protocol.Interpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Model.Protocol.NullExtensions
{
    [InterpreterMeta("None", "")]
    public class NullInterpreter : IInterpreter
    {
        public void Cancel()
        {
        }

        public List<ProtocolEvent> Generate(string protocolName)
        {
            return null;
        }
    }


}
