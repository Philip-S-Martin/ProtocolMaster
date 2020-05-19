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
        private IEnumerable<ExportFactory<IVisualizer, VisualizerMeta>> Visualizers { get; set; }
        public void Print()
        {
            foreach (ExportFactory<IVisualizer, VisualizerMeta> i in Visualizers)
            {
                App.Window.Timeline.ListVisualizer(i.Metadata);
                Log.Error("Visualizer found: " + i.Metadata.Name + " version " + i.Metadata.Version);
            }
        }
    }
}
