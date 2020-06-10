using ProtocolMaster.Model.Protocol.Driver;
using ProtocolMaster.Model.Protocol.Interpreter;
using ProtocolMaster.View;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProtocolMaster.ViewModel
{
    public class TimelineViewModel : ViewModelBase
    {
        //public List<MenuItemViewModel> InterpreterOptions { get; set; }
        public TimelineViewModel()
        {
            //App.Instance.Extensions.Interpreters.Interpreters.CollectionChanged += LoadInterpreterOptions;
        }

        public void LoadInterpreterOptions(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }
        public List<InterpreterMeta> Options { get; set; }
    }
}
