using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace ProtocolMaster.Component.Model
{
    internal interface IInterpreterManager
    {
        void Print();
    }

    [Export(typeof(IInterpreterManager))]
    internal class InterpreterManager : IInterpreterManager
    {
        [ImportMany]
        IEnumerable<Lazy<IInterpreter, IExtensionData>> _drivers;
        // Driver thread management
        public void Print()
        {
            foreach (Lazy<IInterpreter, IExtensionData> i in _drivers)
            {
                App.Window.Timeline.ListInterpreter(i.Metadata.Symbol[0]);
                Log.Error("Interpreter found: " + i.Metadata.Symbol[0]);
            }
        }
    }
}
