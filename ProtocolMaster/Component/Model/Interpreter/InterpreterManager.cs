using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model
{
    internal interface IInterpreterManager
    {
        void Print();
        void Select(InterpreterMeta target);
        void Run();

    }

    [Export(typeof(IInterpreterManager))]
    internal class InterpreterManager : IInterpreterManager
    {
        [ImportMany]
        IEnumerable<ExportFactory<IInterpreter, InterpreterMeta>> _interpreters;

        ExportFactory<IInterpreter, InterpreterMeta> interpreterFactory;
        ExportLifetimeContext<IInterpreter> interpreterContext;
        IInterpreter interpreter;

        private Task runTask;
        private CancellationToken cancelToken;
        private CancellationTokenSource tokenSource;

        // interpreter thread management
        public void Print()
        {
            foreach (ExportFactory<IInterpreter, InterpreterMeta> interpreter in _interpreters)
            {
                App.Window.Timeline.ListInterpreter(interpreter.Metadata);
                Log.Error("Interpreter found: " + interpreter.Metadata.Name +" version " + interpreter.Metadata.Version);

                for (int i = 0; i < interpreter.Metadata.PageHeadersCSV.Length; i++)
                {
                    foreach (string val in interpreter.Metadata.PageHeadersCSV[i].Split(','))
                        Log.Error("Header " + i + ": " + val);
                }
            }
        }

        public void Select(InterpreterMeta target)
        {
            foreach (ExportFactory<IInterpreter, InterpreterMeta> i in _interpreters)
            {
                if (i.Metadata == target)
                {
                    interpreterFactory = i;
                }
            }
        }


        public void Run()
        {
            interpreterContext = interpreterFactory.CreateExport();
            interpreter = interpreterContext.Value;

            tokenSource = new CancellationTokenSource();
            cancelToken = tokenSource.Token;

            runTask = Task.Run(new Action(() =>
            {
                // Register interpreter cancel
                cancelToken.Register(new Action(() => interpreter.Cancel()));
                // pre-fill event data
                interpreter.Run();
            }), tokenSource.Token);

            interpreterContext.Dispose();
        }
    }
}
