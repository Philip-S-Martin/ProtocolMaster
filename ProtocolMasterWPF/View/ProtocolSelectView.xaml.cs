using ProtocolMasterWPF.Properties;
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
        public static readonly DependencyProperty CurrentSelectorProperty =
            DependencyProperty.Register("CurrentSelector", typeof(ISelectView), typeof(ProtocolSelectView));
        public ISelectView CurrentSelector
        {
            get { return (ISelectView)GetValue(CurrentSelectorProperty); }
            set { SetValue(CurrentSelectorProperty, value); }
        }
        private RadioButton LastTab { get; set; }
        public ProtocolSelectView()
        {
            string openTab = Settings.Default.ExperimentDefaultTab;
            InitializeComponent();
            if (DriveTab.Name == openTab) DriveTab.IsChecked = true;
            else if (PublishedTab.Name == openTab) PublishedTab.IsChecked = true;
            else if (LocalTab.Name == openTab) LocalTab.IsChecked = true;
        }
        private void DriveTab_Checked(object sender, RoutedEventArgs e)
        {
            CurrentSelector = DriveSelect;
            LastTab = sender as RadioButton;
        }
        private void PublishedTab_Checked(object sender, RoutedEventArgs e)
        {
            CurrentSelector = PublishedSelect;
            LastTab = sender as RadioButton;
        }
        private void LocalTab_Checked(object sender, RoutedEventArgs e)
        {
            CurrentSelector = LocalSelect;
            LastTab = sender as RadioButton;
        }
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ExperimentDefaultTab = LastTab.Name;
            Settings.Default.Save();
            MaterialDesignThemes.Wpf.DialogHost.Close("SessionDialog", CurrentSelector.SelectList.SelectedItem);
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ExperimentDefaultTab = LastTab.Name;
            Settings.Default.Save();
            MaterialDesignThemes.Wpf.DialogHost.Close("SessionDialog");
        }
    }
}
