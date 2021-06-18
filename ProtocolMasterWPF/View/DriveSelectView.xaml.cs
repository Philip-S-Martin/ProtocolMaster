using ProtocolMasterWPF.Model.Google;
using ProtocolMasterWPF.ViewModel;
using System.Windows.Controls;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for DriveSelectView.xaml
    /// </summary>
    public partial class DriveSelectView : UserControl, ISelectView
    {
        public ListBox SelectList { get => SelectListBox; }
        public DriveSelectView()
        {
            InitializeComponent();
        }
        private void SignIn_Click(object sender, System.Windows.RoutedEventArgs e) => ((App)App.Current).GoogleAuthenticate();
    }
}
