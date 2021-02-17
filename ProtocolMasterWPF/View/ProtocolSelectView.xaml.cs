using ProtocolMasterWPF.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for ProtocolSelectView.xaml
    /// </summary>
    public partial class ProtocolSelectView : UserControl
    {
        ISelectView CurrentSelector;
        public ProtocolSelectView()
        {
            InitializeComponent();
        }
        private void DriveTab_Checked(object sender, RoutedEventArgs e)
        {
            DataContext = DriveSelect;
            CurrentSelector = DriveSelect;
        }
        private void PublishedTab_Checked(object sender, RoutedEventArgs e)
        {
            DataContext = PublishedSelect;
            CurrentSelector = PublishedSelect;
        }
        private void LocalTab_Checked(object sender, RoutedEventArgs e)
        {
            DataContext = LocalSelect;
            CurrentSelector = LocalSelect;
        }
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.DialogHost.Close("SessionDialog", CurrentSelector.SelectList.SelectedItem);
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.DialogHost.Close("SessionDialog");
        }
    }
}
