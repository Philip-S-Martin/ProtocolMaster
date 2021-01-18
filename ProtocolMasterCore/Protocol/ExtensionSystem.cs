using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ProtocolMasterCore.Protocol.Interpreter;
using ProtocolMasterCore.Protocol.Driver;
using ProtocolMasterCore.Prompt;

namespace ProtocolMasterCore.Protocol
{
    internal class ExtensionSystem
    {
        // Import All Extensions so that they can be Composed (ComposeParts())
        public DriverManager DriverManager { get; private set; }
        public InterpreterManager InterpreterManager { get; private set; }
        // Composition Container for all extension managers
        private readonly CompositionContainer _container;
        public List<ProtocolEvent> Data { get; private set; }

        bool isRunning;
        bool isReady;

        Task<List<ProtocolEvent>> generator;
        private Task runTask;
        private CancellationToken cancelToken;
        private CancellationTokenSource tokenSource;

        public ExtensionSystem(string directory)
        {
            isRunning = false;
            isReady = false;

            DriverManager = new DriverManager();
            InterpreterManager = new InterpreterManager();

            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IExtension).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(directory));
            _container = new CompositionContainer(catalog);
        }

        ~ExtensionSystem()
        {
            Terminate();
        }
        public void LoadExtensions()
        {
            DriverManager.LoadOptions(_container);
            InterpreterManager.LoadOptions(_container);
        }
        public void Interpret(string selectionID, string argument, FileStream fs)
        {
            if (isRunning)
            {
                throw new Exception("Cannot interpret a new protocol while a protocol is running");
            }
            else if (isReady)
            {
                Reset();
                //then fall through
            }
            isReady = true;
            tokenSource = new CancellationTokenSource();
            cancelToken = tokenSource.Token;
            generator = Task.Factory.StartNew<List<ProtocolEvent>>(() =>
            {
                cancelToken.Register(new Action(() =>
                {
                    if (InterpreterManager.IsRunning)
                    {
                        InterpreterManager.CancelRunningExtension();
                    }
                }));
                return InterpreterManager.GenerateData(selectionID, argument, fs);
            }, TaskCreationOptions.LongRunning);

            Task UITask = generator.ContinueWith((data) =>
            {
                List<ProtocolEvent> result = generator.Result;
                Data = result;
                if (result != null)
                {
                    InterpreterManager.OnEventsLoaded(result);
                }
            });
        }
        public void Run()
        {
            if (isRunning)
            {
                throw new Exception("A protocol is already running");
            }
            else if (!isReady)
            {
                throw new Exception("A protocol cannot be run before it is ready");
            }
            else
            {
                isReady = false;
                isRunning = true;
                Progress<DriverProgress> driverProgress = new Progress<DriverProgress>();

                Task UITask = generator.ContinueWith((data) =>
                {
                    runTask = Task.Run(new Action(() =>
                    {
                        cancelToken.Register(new Action(() =>
                        {
                            if (DriverManager.IsRunning)
                            {
                                DriverManager.CancelRunningExtension();
                            }
                        }));
                        DriverManager.Run(Data);
                    }), tokenSource.Token);

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        public void End()
        {
            if (!isRunning)
            {
                throw new Exception("A protocol cannot be cancelled/ended when not running");
            }
            Terminate();
        }
        public void Reset()
        {
            if (isRunning)
            {
                throw new Exception("A protocol cannot be reset when running");
            }
            Terminate();
        }
        public void Terminate()
        {
            if (tokenSource != null && !tokenSource.IsCancellationRequested)
                tokenSource.Cancel();
            isRunning = false;
        }
    }
}
