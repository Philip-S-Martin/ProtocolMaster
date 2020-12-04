using ExcelDataReader;
using ProtocolMaster.Model.Debug;
using ProtocolMaster.Model.Protocol.Interpreter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolMaster.Model.Protocol.Interpreter
{
    internal class InterpreterManager : ExtensionManager<IInterpreter, InterpreterMeta>
    {        
        IInterpreter interpreter;

        public List<ProtocolEvent> GenerateData(string selectionID)
        {
            interpreter = CreateSelectedExtension();

            if (typeof(ExcelDataInterpreter).IsAssignableFrom(interpreter.GetType()))
            {
                ExcelDataInterpreter spreadSheetInterpreter = interpreter as ExcelDataInterpreter;
                Stream nfs = Model.Google.GDrive.Instance.Download(selectionID);

                if (nfs != null)
                    spreadSheetInterpreter.SetReader(ExcelReaderFactory.CreateReader(nfs));
                else return null;
            }
            
            // pre-fill event data
            List<ProtocolEvent> result = interpreter.Generate("Protocol");
            DisposeSelectedExtension();
            return result;
        }
    }
}
