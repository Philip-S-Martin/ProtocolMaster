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
    internal class InterpreterManager : IExtensionManager<IInterpreter, InterpreterMeta>
    {        
        IInterpreter interpreter;

        public List<DriveData> GenerateData()
        {
            interpreter = CreateSelectedExtension();

            if (typeof(SpreadSheetInterpreter).IsAssignableFrom(interpreter.GetType()))
            {
                SpreadSheetInterpreter spreadSheetInterpreter = interpreter as SpreadSheetInterpreter;
                FileStream nfs = File.Open(Log.Instance.AppData + "\\Protocols\\Copy of example_protocols.xlsx", FileMode.Open, FileAccess.Read);
                spreadSheetInterpreter.SetReader(ExcelReaderFactory.CreateReader(nfs));
            }
            // pre-fill event data
            interpreter.Generate("TEST");

            List<DriveData> result = interpreter.Data;
            DisposeSelectedExtension();
            return result;
        }
    }
}
