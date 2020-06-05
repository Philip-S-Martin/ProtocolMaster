using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Component.Model.Config
{
    public enum ConfigMode
    {
        NONE = 0,
        STRING,
        OPTION
    }
    class ConfigItem
    {
        public string Key { get; set; }
        public string Value
        {
            get => Value; 
            set
            {
                if (Mode == ConfigMode.OPTION)
                {
                    if (options.Contains(value))
                        Value = value;
                    else throw new ArgumentException("New value does not belong to configuration options, and the ConfigItem uses ConfigMode OPTION");
                }
                else
                    value = Value;
            }
        }
        public ConfigMode Mode { get; set; }

        List<string> options;
        public ConfigItem(string key, string value)
        {
            Key = key;
            Value = value;
            options = new List<string>();
        }
        public void AddOption(string newOption)
        {
            if (options.Contains(newOption))
            {
                throw new ArgumentException("Argument newOption is not unique");
            }
            else
            {
                options.Add(newOption);
            }
        }
        public void ResetOptions()
        {
            options = new List<string>();
        }
    }
}
