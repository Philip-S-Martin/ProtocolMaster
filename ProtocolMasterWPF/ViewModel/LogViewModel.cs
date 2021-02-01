using System.Collections.ObjectModel;

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
