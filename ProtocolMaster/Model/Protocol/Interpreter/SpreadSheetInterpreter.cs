using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Model.Protocol.Interpreter
{
    public abstract class SpreadSheetInterpreter
    {
        protected IExcelDataReader DataReader { get; private set; }
        internal void SetReader(IExcelDataReader newReader)
        {
            DataReader = newReader;
        }

        
    }
}
