
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class Log : UserControl
    {
        public Log()
        {
            InitializeComponent();
        }

        // Log Functions
        public void LogToView(string logString)
        {
            Paragraph addLog = new Paragraph();
            addLog.Inlines.Add(logString);
            LogDocument.Blocks.Add(addLog);
        }

        public void Log_Folder_Click(object sender, RoutedEventArgs e)
        {
            ProtocolMaster.Model.Debug.Log.Instance.OpenFolder();
        }
        public void Log_Test(object sender, RoutedEventArgs e)
        {
            ProtocolMaster.Model.Debug.Log.Error("Test");
        }
    }
}
