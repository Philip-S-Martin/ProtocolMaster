using ExcelDataReader;
using ProtocolMaster.Model.Debug;
using ProtocolMaster.Model.Protocol.Interpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolMaster.Model.Protocol.Interpreter
{
    internal interface IInterpreterManager
    {
        void Load();
        void Select(InterpreterMeta target);
        InterpreterMeta Selected { get; }


        List<DriveData> Generate();

    }

    [Export(typeof(IInterpreterManager))]
    internal class InterpreterManager : IInterpreterManager
    {
        [ImportMany]
        private IEnumerable<ExportFactory<IInterpreter, InterpreterMeta>> Interpreters { get; set; }

        ExportFactory<IInterpreter, InterpreterMeta> interpreterFactory;
        ExportLifetimeContext<IInterpreter> interpreterContext;
        IInterpreter interpreter;
        public InterpreterMeta Selected { get { return interpreterFactory.Metadata; } }

        public void Load()
        {
            foreach (ExportFactory<IInterpreter, InterpreterMeta> i in Interpreters)
            {
                App.Window.TimelineView.ListInterpreter(i.Metadata);
                if (i.Metadata.Name == "None" && i.Metadata.Version == "")
                {
                    Select(i.Metadata);
                }
                Log.Error("Interpreter found: '" + i.Metadata.Name + "' version: '" + i.Metadata.Version + "'");
            }
            App.Window.TimelineView.ShowSelectedInterpreter();
        }

        public void Select(InterpreterMeta target)
        {
            foreach (ExportFactory<IInterpreter, InterpreterMeta> i in Interpreters)
            {
                if (i.Metadata == target)
                {
                    interpreterFactory = i;
                }
            }
        }


        public List<DriveData> Generate()
        {
            interpreterContext = interpreterFactory.CreateExport();
            interpreter = interpreterContext.Value;


            if (typeof(SpreadSheetInterpreter).IsAssignableFrom(interpreter.GetType()))
            {
                SpreadSheetInterpreter spreadSheetInterpreter = interpreter as SpreadSheetInterpreter;
                FileStream nfs = File.Open(Log.Instance.AppData + "\\Protocols\\Copy of example_protocols.xlsx", FileMode.Open, FileAccess.Read);
                spreadSheetInterpreter.SetReader(ExcelReaderFactory.CreateReader(nfs));
            }
            // pre-fill event data
            interpreter.Generate("TEST");

            List<DriveData> result = interpreter.Data;
            interpreterContext.Dispose();
            return result;
        }
    }
}
