using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Model
{
    public delegate string CallDropdownHandler(string[] keys);
    public interface ICallDropdown
    {
        public CallDropdownHandler CallDropdown { set; }
    }
}
