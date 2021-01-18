using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterCore.Prompt
{
    public class PromptTargetStore
    {
        public UserSelectHandler UserSelect { get; set; }

        public PromptTargetStore()
        {
            UserSelect = DefaultPrompts.UserSelect;
        }
    }
}
