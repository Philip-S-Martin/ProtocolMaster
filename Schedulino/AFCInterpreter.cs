using ProtocolMaster.Component.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulino
{
    [InterpreterMeta("SchedulinoDriverTest", "1.1", "A,B,C", "D,E,F")]
    public class LegacyAFCInterpreter : IInterpreter
    {
        public List<DriveData> Data => throw new NotImplementedException();

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }


}
