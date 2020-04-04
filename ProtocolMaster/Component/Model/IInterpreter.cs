using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model
{
    public struct InterpreterSymbol
    {
        string name;
        // this will be used to select the correct filetype and validate that the driver is compatible
        // for example, an IFileDiscriminator could look for folders of CSVs with a certain set of headers
        // IFileDiscriminator fileDiscriminator; 
    }
    public interface IInterpreterData
    {
        InterpreterSymbol Symbol { get; }
    }
    public interface IInterpreter
    {
    }
}
