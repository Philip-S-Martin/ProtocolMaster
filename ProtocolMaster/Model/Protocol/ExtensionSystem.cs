using ProtocolMaster.Model.Debug;
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
using ProtocolMaster.Model.Protocol.Interpreter;
using ProtocolMaster.Model.Protocol.Driver;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ProtocolMaster.Model.Protocol
{

    internal class ExtensionSystem
    {
        // Import All Extensions so that they can be Composed (ComposeParts())
        public DriverManager DriverManager { get; private set; }
        public InterpreterManager InterpreterManager { get; private set; }
        // Composition Container for all extension managers
        private readonly CompositionContainer _container;
        public List<DriveData> Data { get; private set; }

        bool isRunning;

        private Task runTask;
        private CancellationToken cancelToken;
        private CancellationTokenSource tokenSource;

        public ExtensionSystem()
        {
            isRunning = false;

            DriverManager = new DriverManager();
            InterpreterManager = new InterpreterManager();

            string targetDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Extensions";
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IExtension).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(targetDir));
            _container = new CompositionContainer(catalog);
            Debug.Log.Error("Extension Location: " + targetDir);
        }

        public void LoadExtensions()
        {
            DriverManager.LoadOptions(_container);
            InterpreterManager.LoadOptions(_container);
        }
        public void Run()
        {
            if (isRunning)
            {
                throw new Exception("A protocol is already running");
            }
            else
            {
                isRunning = true;
                tokenSource = new CancellationTokenSource();
                cancelToken = tokenSource.Token;
                Progress<DriverProgress> driverProgress = new Progress<DriverProgress>();
                //DriverProgress.

                Task<List<DriveData>> generator = Task.Factory.StartNew<List<DriveData>>(
                    () => InterpreterManager.GenerateData(), TaskCreationOptions.LongRunning);

                Task UITask = generator.ContinueWith((data) =>
                {
                    Data = generator.Result;
                    App.Window.TimelineView.LoadPlotData(Data);

                    runTask = Task.Run(new Action(() =>
                    {
                        cancelToken.Register(new Action(() =>
                        {
                            //Interpreters.Cancel(); 
                            DriverManager.Cancel();
                        }));
                        DriverManager.Run(Data);
                    }), tokenSource.Token);

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public void Cancel()
        {
            if(InterpreterManager.IsRunning)
            {
                InterpreterManager.Cancel();
            }
            if (DriverManager.IsRunning)
            {
                DriverManager.Cancel();
            }
            isRunning = false;
        }
    }
}
