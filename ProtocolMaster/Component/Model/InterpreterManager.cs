using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace ProtocolMaster.Component.Model
{
    class InterpreterManager
    {
        [ImportMany]
        IEnumerable<Lazy<IInterpreter, IInterpreterData>> _interpreters;

        private CompositionContainer _container;

        public InterpreterManager()
        {
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(InterpreterManager).Assembly));
            catalog.Catalogs.Add(new DirectoryCatalog(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Interpreter"));
            _container = new CompositionContainer(catalog);
            Log.Error("Interpreters Location: " + Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Interpreter");

            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Log.Error(compositionException.ToString());
            }

            foreach (Lazy<IInterpreter, IInterpreterData> i in _interpreters)
            {
                App.Window.Timeline.ListInterpreter(i.Metadata.Symbol[0].ToString());
                Log.Error("Interpreter found: " + i.Metadata.Symbol[0].ToString());
            }
        }
    }
}
