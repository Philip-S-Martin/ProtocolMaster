using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model.Config
{
    class ConfigFile
    {
        public string Name { get; private set; }
        Dictionary<string, ConfigItem> configurations;

        public ConfigFile(string name)
        {
            Name = name;

        }
    }
}
