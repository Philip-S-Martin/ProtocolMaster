using ProtocolMasterCore.Protocol.Driver;
using ProtocolMasterCore.Protocol.Interpreter;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolMasterCore.Protocol
{
    public class InterpretAndDriveProtocol
    {
        // Import All Extensions so that they can be Composed (ComposeParts())
        public DriverManager DriverManager { get; private set; }
        public InterpreterManager InterpreterManager { get; private set; }
        // Composition Container for all extension managers
        private readonly CompositionContainer _container;
        public List<ProtocolEvent> Data { get; private set; }

        bool isRunning;
        bool isReady;

        private CancellationToken cancelToken;
        private CancellationTokenSource tokenSource;

        public InterpretAndDriveProtocol(string directory)
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
        ~InterpretAndDriveProtocol()
        {
            Terminate();
        }
        public void LoadExtensions()
        {
            DriverManager.LoadOptions(_container);
            InterpreterManager.LoadOptions(_container);
        }
        public void Interpret(Stream stream, string argument = null)
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
            Data = InterpreterManager.GenerateData(stream, argument);
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
                DriverManager.Run(Data);
            }
        }
        public void Cancel()
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
        private void Terminate()
        {
            if (InterpreterManager.IsRunning)
            {
                InterpreterManager.CancelRunningExtension();
            }
            if (DriverManager.IsRunning)
            {
                DriverManager.CancelRunningExtension();
            }
            if (tokenSource != null && !tokenSource.IsCancellationRequested)
                tokenSource.Cancel();
            isRunning = false;
        }
    }
}
