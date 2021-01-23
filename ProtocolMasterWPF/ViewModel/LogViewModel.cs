using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ProtocolMasterWPF.ViewModel
{
    public class LogViewModel
    {
        public ObservableCollection<string> LogText { get; private set; }
        public LogViewModel()
        {
            LogText = new ObservableCollection<string>();
        }
    }
}
