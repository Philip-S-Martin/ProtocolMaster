using ProtocolMaster.Component.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulino
{
    [InterpreterExtension("LegacyAFCInterpreter", 1, "A,B,C", "D,E,F")]
    public class LegacyAFCInterpreter : IInterpreter
    {

    }

    
}
