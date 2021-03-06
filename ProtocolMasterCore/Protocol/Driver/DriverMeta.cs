﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;

namespace ProtocolMasterCore.Protocol.Driver
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DriverMeta : ExportAttribute, IExtensionMeta
    {
        public string Name { get; private set; }
        public string Version { get; private set; }
        public string[] HandlerLabels { get; private set; }

        public DriverMeta(string name, string version, params string[] eventNames) : base(typeof(IDriver))
        {
            this.Name = name;
            this.Version = version;
            this.HandlerLabels = eventNames;
        }
        public DriverMeta()
        { 
        }

        public DriverMeta(IDictionary<string, object> inputs)
        {
            HandlerLabels = (string[])inputs["HandlerLabels"];
            Name = (string)inputs["Name"];
            Version = (string)inputs["Version"];
        }

        public override string ToString()
        {
            return Name + " " + Version;
        }

        public bool Equals([AllowNull] IExtensionMeta other)
        {
            return  this.Name == other.Name && this.Version == other.Version;
        }
    }
}
