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

        private void PublishButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if((GFileStreamer)SelectList.SelectedItem != null) ((GFileStreamer)SelectList.SelectedItem).PublishAndLink();
        }

        private void OpenDriveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.TryOpenURI(this, "https://drive.google.com/drive/");
        }

        private void RefreshButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GDrive.Instance.RefreshAvailable();
        }

        private void OpenInDriveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((GFileStreamer)SelectList.SelectedItem != null) ((GFileStreamer)SelectList.SelectedItem).OpenInBrowser();
        }

        private void DownloadLocalButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if ((GFileStreamer)SelectList.SelectedItem != null) ((GFileStreamer)SelectList.SelectedItem).DownloadToLocal("xlsx");
        }
    }
}
