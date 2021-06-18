using MaterialDesignThemes.Wpf;
using ProtocolMasterWPF.Model;
using ProtocolMasterWPF.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for ProtocolSelectDialog.xaml
    /// </summary>
    public partial class ProtocolSelectDialog : UserControl
    {
        public static readonly DependencyProperty CurrentSelectorProperty =
            DependencyProperty.Register("CurrentSelector", typeof(ISelectView), typeof(ProtocolSelectDialog));
        public ISelectView CurrentSelector
        {
            get { return (ISelectView)GetValue(CurrentSelectorProperty); }
            set { SetValue(CurrentSelectorProperty, value); }
        }
        private RadioButton LastTab { get; set; }
        private Session sessionControl;
        internal static void ShowDialog(Session SessionControl) => DialogHost.Show(new ProtocolSelectDialog(SessionControl), "SessionDialogHost");
        internal ProtocolSelectDialog(Session sessionControl)
        {
            string openTab = Settings.Default.ExperimentDefaultTab;
            this.sessionControl = sessionControl;
            InitializeComponent();
            if (DriveTab.Name == openTab) DriveTab.IsChecked = true;
            else if (PublishedTab.Name == openTab) PublishedTab.IsChecked = true;
            else if (LocalTab.Name == openTab) LocalTab.IsChecked = true;
        }
        private void ChangeSelector(ISelectView selector,object sender, RoutedEventArgs e)
        {
            if (CurrentSelector != null) CurrentSelector.SelectList.SelectedItem = null;
            CurrentSelector = selector;
            selector.SelectList.Items.Refresh();
            LastTab = sender as RadioButton;
        }
        private void DriveTab_Checked(object sender, RoutedEventArgs e) => ChangeSelector(DriveSelect, sender, e);
        private void PublishedTab_Checked(object sender, RoutedEventArgs e) => ChangeSelector(PublishedSelect, sender, e);
        private void LocalTab_Checked(object sender, RoutedEventArgs e) => ChangeSelector(LocalSelect, sender, e);
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ExperimentDefaultTab = LastTab.Name;
            Settings.Default.Save();
            MaterialDesignThemes.Wpf.DialogHost.Close("SessionDialogHost");
            var result = CurrentSelector.SelectList.SelectedItem;
            if (result == null) sessionControl.CancelSelection();
            else sessionControl.MakeSelection(result);
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ExperimentDefaultTab = LastTab.Name;
            Settings.Default.Save();
            MaterialDesignThemes.Wpf.DialogHost.Close("SessionDialogHost");
        }
    }
}
