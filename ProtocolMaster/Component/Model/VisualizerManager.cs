using ProtocolMaster.Component.Debug;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model
{
    internal interface IVisualizerManager
    {
        void Print();
    }

    [Export(typeof(ProtocolMaster.Component.Model.IVisualizerManager))]
    internal class VisualizerManager : IVisualizerManager
    {
        [ImportMany]
        IEnumerable<Lazy<IVisualizer, IExtensionData>> _drivers;
        public void Print()
        {
            foreach (Lazy<IVisualizer, IExtensionData> i in _drivers)
            {
                App.Window.Timeline.ListVisualizer(i.Metadata.Symbol[0]);
                Log.Error("Visualizer found: " + i.Metadata.Symbol[0]);
            }
        }
    }
}
