using ProtocolMaster.Component.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulinoDriver
{
    [Export(typeof(ProtocolMaster.Component.Model.IInterpreter))]
    [ExportMetadata("Symbol", new string[] { "LegacyAFCInterpreter", "Header 1", "Header 2", "Header 3" })]
    public class LegacyAFCInterpreter : IInterpreter
    {

    }

    
}
