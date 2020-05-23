using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ProtocolMaster.Component.Model
{

    internal class ExtensionSystem
    {
        // Import All Extensions so that they can be Composed (ComposeParts())
        [Import(typeof(IDriverManager))]
        public IDriverManager Drivers { get; private set; }

        [Import(typeof(IInterpreterManager))]
        public IInterpreterManager Interpreters { get; private set; }

        [Import(typeof(IVisualizerManager))]
        public IVisualizerManager Visualizers { get; private set; }

        // Composition Objects
        private readonly CompositionContainer _container;

        bool isRunning;

        private Task runTask;
        private CancellationToken cancelToken;
        private CancellationTokenSource tokenSource;

        public ExtensionSystem()
        {
            isRunning = false;
            string targetDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Extensions";
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(InterpreterManager).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(targetDir));
            _container = new CompositionContainer(catalog);
            Log.Error("Extension Location: " + targetDir);
            this._container.ComposeParts(this);
            try
            {

            }
            catch (CompositionException compositionException)
            {
                Log.Error(compositionException.ToString());
            }
        }
        public void PrepExtensions()
        {
            Drivers.Load();
            Interpreters.Load();
            Visualizers.Load();
        }

        public void Run()
        {
            if (isRunning == false)
            {
                isRunning = true;
                tokenSource = new CancellationTokenSource();
                cancelToken = tokenSource.Token;

                runTask = Task.Run(new Action(() =>
                {
                    cancelToken.Register(new Action(() =>
                        {
                            //Interpreters.Cancel(); 
                            Drivers.Cancel();
                        }));
                    Drivers.Run(Interpreters.Generate());
                }), tokenSource.Token);
            }
        }
        public void Cancel()
        {
            if (Drivers.IsRunning == true)
            {
                Drivers.Cancel();
            }
            isRunning = false;
        }
    }
}
