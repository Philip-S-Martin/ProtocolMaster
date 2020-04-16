using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model
{

    internal class ExtensionManager
    {
        // Import All Extensions so that they can be Composed (ComposeParts())
        [Import(typeof(IDriverManager))]
        private IDriverManager driverManager;
        public IDriverManager DriverManager { get => driverManager; private set => driverManager = value; }

        [Import(typeof(IInterpreterManager))]
        private IInterpreterManager interpreterManager;
        public IInterpreterManager InterpreterManager { get => interpreterManager; private set => interpreterManager = value; }

        [Import(typeof(IVisualizerManager))]
        private IVisualizerManager visualizerManager;
        public IVisualizerManager VisualizerManager { get => visualizerManager; private set => visualizerManager = value; }

        // Composition Objects
        private CompositionContainer _container;

        public ExtensionManager()
        {
            string targetDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Extensions";
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(InterpreterManager).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(targetDir));
            _container = new CompositionContainer(catalog);
            Log.Error("Extension Location: " + targetDir);

            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Log.Error(compositionException.ToString());
            }

            driverManager.Print();
            interpreterManager.Print();
            visualizerManager.Print();
        }
    }
}
