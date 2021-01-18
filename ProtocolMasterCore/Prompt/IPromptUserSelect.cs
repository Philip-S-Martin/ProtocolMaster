using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterCore.Prompt
{
    public delegate string UserSelectHandler(string[] keys);
    public interface IPromptUserSelect
    {
        public UserSelectHandler UserSelectPrompt { set; }
    }
}
