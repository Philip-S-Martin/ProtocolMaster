using ProtocolMaster.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMaster.Model
{
    public static class CallHandler
    {
        public static string CallDropdown(string[] keys)
        {
            return App.Current.Dispatcher.Invoke(new Func<string>(() => { return PopupDropdown.PopupNow(keys); }));
        }
    }
}
