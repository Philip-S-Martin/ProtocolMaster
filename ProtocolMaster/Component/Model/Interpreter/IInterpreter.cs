using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ProtocolMaster.Component.Model
{
    public interface IInterpreter
    {
        List<DriveData> Data { get; }
        void Generate(string protocolName);
        void Cancel();
    }
}
