using ProtocolMaster.Component.Model.Interpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model.NullExtensions
{
    [InterpreterMeta("None", "")]
    public class NullInterpreter : IInterpreter
    {
        public List<DriveData> Data { get { return null; } }

        public void Cancel()
        {
        }

        public void Generate(string protocolName)
        {
        }
    }


}
