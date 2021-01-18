using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterCore.Prompt
{
    public static class DefaultPrompts
    {
        public static string UserSelect(string[] options)
        {
            if(options != null && options.Length > 0 && options[0] != null)
                return options[0];
            return "null";
        }
    }
}
