using ProtocolMaster.Component.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedulino
{
    [Export(typeof(ProtocolMaster.Component.Model.IVisualizer))]
    [ExportMetadata("Symbol", new string[] { "Visualizer", "Header 1", "Header 2", "Header 3" })]
    public class CategoricalVisualizer : IVisualizer
    {

    }
}
