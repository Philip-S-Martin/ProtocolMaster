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
        IEnumerable<Lazy<IInterpreter, InterpreterExtension>> _interpreters;
        // Driver thread management
        public void Print()
        {
            foreach (Lazy<IInterpreter, InterpreterExtension> driver in _interpreters)
            {
                App.Window.Timeline.ListInterpreter(driver.Metadata.Name);
                Log.Error("Interpreter found: " + driver.Metadata.Name);

                for (int i = 0; i < driver.Metadata.PageHeadersCSV.Length; i++)
                {
                    foreach (string val in driver.Metadata.PageHeadersCSV[i].Split(','))
                        Log.Error("Header " + i + ": " + val);
                }
            }
        }
    }
}
