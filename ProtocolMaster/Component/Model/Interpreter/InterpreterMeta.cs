using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace ProtocolMaster.Component.Model
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InterpreterMeta : ExportAttribute
    {
        public string Name { get; private set; }
        public string Version { get; private set; }
        public string[] PageHeadersCSV { get; private set; }

        public InterpreterMeta(string name, string version, params string[] pageHeadersCSV) : base(typeof(IInterpreter))
        {
            this.Name = name;
            this.Version = version;
            this.PageHeadersCSV = pageHeadersCSV;
        }

        public InterpreterMeta(IDictionary<string, object> inputs)
        {
            PageHeadersCSV = (string[])inputs["PageHeadersCSV"];
            Name = (string)inputs["Name"];
            Version = (string)inputs["Version"];
        }
        public override string ToString()
        {
            return Name + " " + Version;
        }
    }
}
