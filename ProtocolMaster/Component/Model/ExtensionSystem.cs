using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ProtocolMaster.Component.Model
{

    internal class ExtensionSystem
    {
        // Import All Extensions so that they can be Composed (ComposeParts())
        [Import(typeof(IDriverManager))]
        private IDriverManager driverManager;
        public IDriverManager Drivers { get => driverManager; private set => driverManager = value; }

        [Import(typeof(IInterpreterManager))]
        private IInterpreterManager interpreterManager;
        public IInterpreterManager Interpreters { get => interpreterManager; private set => interpreterManager = value; }

        [Import(typeof(IVisualizerManager))]
        private IVisualizerManager visualizerManager;
        public IVisualizerManager Visualizers { get => visualizerManager; private set => visualizerManager = value; }

        // Composition Objects
        private readonly CompositionContainer _container;

        public ExtensionSystem()
        {
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

            driverManager.Print();
            interpreterManager.Print();
            visualizerManager.Print();
        }

        public void Run()
        {
            //interpreterManager.Run();
            //driverManager.
        }
    }
}
