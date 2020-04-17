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
        IEnumerable<Lazy<IVisualizer, VisualizerExtension>> _visualizers;
        public void Print()
        {
            foreach (Lazy<IVisualizer, VisualizerExtension> i in _visualizers)
            {
                App.Window.Timeline.ListVisualizer(i.Metadata.Name);
                Log.Error("Visualizer found: " + i.Metadata.Name);
            }
        }
    }
}
