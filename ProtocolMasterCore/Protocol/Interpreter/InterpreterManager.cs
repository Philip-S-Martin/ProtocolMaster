using ExcelDataReader;
using System.Collections.Generic;
using System.IO;

namespace ProtocolMasterCore.Protocol.Interpreter
{
    public delegate void ProtocolEventsLoader(List<ProtocolEvent> events);
    public class InterpreterManager : ExtensionManager<IInterpreter, InterpreterMeta>
    {
        IInterpreter interpreter;
        public ProtocolEventsLoader OnEventsLoaded;
        internal List<ProtocolEvent> GenerateData(Stream stream, string argument = null)
        {
            interpreter = CreateSelectedExtension();

            if (typeof(ExcelDataInterpreter).IsAssignableFrom(interpreter.GetType()))
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
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
