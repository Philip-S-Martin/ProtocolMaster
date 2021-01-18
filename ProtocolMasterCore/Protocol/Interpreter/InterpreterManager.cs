using ExcelDataReader;
using System.Collections.Generic;
using System.IO;

namespace ProtocolMasterCore.Protocol.Interpreter
{
    public delegate void ProtocolEventsLoader(List<ProtocolEvent> events);
    internal class InterpreterManager : ExtensionManager<IInterpreter, InterpreterMeta>
    {        
        IInterpreter interpreter;
        public ProtocolEventsLoader OnEventsLoaded;
        public List<ProtocolEvent> GenerateData(string selectionID, string argument, Stream stream)
        {
            interpreter = CreateSelectedExtension();

            if (typeof(ExcelDataInterpreter).IsAssignableFrom(interpreter.GetType()))
            {
                ExcelDataInterpreter spreadSheetInterpreter = interpreter as ExcelDataInterpreter;
                if (stream != null)
                    spreadSheetInterpreter.SetReader(ExcelReaderFactory.CreateReader(stream));
                else return null;
            }
            // pre-fill event data
            List<ProtocolEvent> result = interpreter.Generate(argument);
            if (interpreter.IsCanceled) return null;
            DisposeSelectedExtension();
            OnEventsLoaded?.Invoke(result);
            return result;
        }
    }
}
