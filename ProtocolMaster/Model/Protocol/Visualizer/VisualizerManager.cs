using ProtocolMaster.Model.Debug;
using ProtocolMaster.Model.Protocol.Visualizer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Model.Protocol.Visualizer
{
    internal interface IVisualizerManager
    {
        void Load();
        void Select(VisualizerMeta target);
        VisualizerMeta Selected { get; }

    }

    [Export(typeof(IVisualizerManager))]
    internal class VisualizerManager : IVisualizerManager
    {
        [ImportMany]
        private IEnumerable<ExportFactory<IVisualizer, VisualizerMeta>> Visualizers { get; set; }

        ExportFactory<IVisualizer, VisualizerMeta> visualizerFactory;
        ExportLifetimeContext<IVisualizer> visualizerContext;
        IVisualizer visualizer;
        public VisualizerMeta Selected { get { return visualizerFactory.Metadata; } }

        public void Load()
        {
            foreach (ExportFactory<IVisualizer, VisualizerMeta> i in Visualizers)
            {
                App.Window.TimelineView.ListVisualizer(i.Metadata);
                if (i.Metadata.Name == "None" && i.Metadata.Version == "")
                {
                    Select(i.Metadata);
                }
                Log.Error("Visualizer found: '" + i.Metadata.Name + "' version: '" + i.Metadata.Version + "'");
            }
            App.Window.TimelineView.ShowSelectedVisualizer();
        }
        public void Select(VisualizerMeta target)
        {
            foreach (ExportFactory<IVisualizer, VisualizerMeta> i in Visualizers)
            {
                if (i.Metadata == target)
                {
                    visualizerFactory = i;
                }
            }
        }
    }
}
