using ProtocolMaster.Component.Model;
using ProtocolMaster.Component.Debug;
using System.ComponentModel.Composition;

namespace LegacyAFC
{
    [Export(typeof(ProtocolMaster.Component.Model.IInterpreter))]
    [ExportMetadata("Symbol", new object[]{"LegacyAFCInterpreter"} )]
    public class LegacyAFCInterpreter : IInterpreter
    {
        public LegacyAFCInterpreter()
        {
            int i = 0;
            i++;
        }
    }
}
