using ExcelDataReader;

namespace ProtocolMasterCore.Protocol.Interpreter
{
    public abstract class ExcelDataInterpreter
    {
        protected IExcelDataReader DataReader { get; private set; }
        internal void SetReader(IExcelDataReader newReader)
        {
            DataReader = newReader;
        }
    }
}
