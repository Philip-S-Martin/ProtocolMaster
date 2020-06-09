using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace ProtocolMaster.Model.Protocol.Visualizer
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class VisualizerMeta : ExportAttribute
    {
        public string Name { get; private set; }
        public string Version { get; private set; }
        public string[] CategoryLabels { get; private set; }

        public VisualizerMeta(string name, string version, params string[] categoryNames) : base(typeof(IVisualizer))
        {
            this.Name = name;
            this.Version = version;
            this.CategoryLabels = categoryNames;
        }

        public VisualizerMeta(IDictionary<string, object> inputs)
        {
            CategoryLabels = (string[])inputs["CategoryLabels"];
            Name = (string)inputs["Name"];
            Version = (string)inputs["Version"];
        }
        public override string ToString()
        {
            return Name + " " + Version;
        }
    }
}
