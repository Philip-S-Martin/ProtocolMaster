using System.Windows.Controls;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : UserControl
    {
        public LogView()
        {
            InitializeComponent();
            DataContext = App.LogVM;
        }
    }
}
