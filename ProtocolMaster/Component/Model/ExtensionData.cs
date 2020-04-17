using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model
{

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InterpreterExtension : ExportAttribute
    {
        public string Name { get; private set; }
        public int Version { get; private set; }
        public string[] PageHeadersCSV { get; private set; }

        public InterpreterExtension(string name, int version, params string[] pageHeadersCSV) : base(typeof(IInterpreter))
        {
            this.Name = name;
            this.Version = version;
            this.PageHeadersCSV = pageHeadersCSV;
        }

        public InterpreterExtension(IDictionary<string, object> inputs)
        {
            foreach (string str in inputs.Keys)
                Debug.Log.Error("Key: " + str);
            
            PageHeadersCSV = (string[])inputs["PageHeadersCSV"];
            Name = (string)inputs["Name"];
            Version = (int)inputs["Version"];
        }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DriverExtension : ExportAttribute
    {
        public string Name { get; private set; }
        public int Version { get; private set; }
        public string[] EventNames { get; private set; }

        public DriverExtension(string name, int version, params string[] eventNames) : base(typeof(IDriver))
        {
            this.Name = name;
            this.Version = version;
            this.EventNames = eventNames;
        }

        public DriverExtension(IDictionary<string, object> inputs)
        {
            EventNames = (string[])inputs["EventNames"];
            Name = (string)inputs["Name"];
            Version = (int)inputs["Version"];
        }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class VisualizerExtension : ExportAttribute
    {
        public string Name { get; private set; }
        public int Version { get; private set; }
        public string[] CategoryNames { get; private set; }

        public VisualizerExtension(string name, int version, params string[] categoryNames) : base(typeof(IVisualizer))
        {
            this.Name = name;
            this.Version = version;
            this.CategoryNames = categoryNames;
        }

        public VisualizerExtension(IDictionary<string, object> inputs)
        {
            CategoryNames = (string[])inputs["CategoryNames"];
            Name = (string)inputs["Name"];
            Version = (int)inputs["Version"];
        }
    }
}
