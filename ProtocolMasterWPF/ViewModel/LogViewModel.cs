using System.Collections.ObjectModel;
using System.Windows.Data;

namespace ProtocolMasterWPF.ViewModel
{
    public class LogViewModel
    {
        public ObservableCollection<string> LogText { get; private set; }
        private object collectionLock;
        public LogViewModel()
        {
            LogText = new ObservableCollection<string>();
            collectionLock = new object();
            BindingOperations.EnableCollectionSynchronization(LogText, collectionLock);
        }

        public void AddLog(string text)
        {
            lock (collectionLock)
            {
                LogText.Add(text);
            }
        }
    }
}
