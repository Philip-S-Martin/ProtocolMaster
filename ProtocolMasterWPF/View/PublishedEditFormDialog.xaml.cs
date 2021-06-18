using MaterialDesignThemes.Wpf;
using ProtocolMasterWPF.Model;
using ProtocolMasterWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for PublishedEditFormDialog.xaml
    /// </summary>
    public partial class PublishedEditFormDialog : UserControl
    {
        PublishedProtocolDataViewModel viewModel;
        
        internal PublishedEditFormDialog(PublishedFileStreamer target)
        {
            viewModel = new PublishedProtocolDataViewModel(target);
            DataContext = viewModel;
            InitializeComponent();
        }

        internal static void ShowDialog(PublishedFileStreamer target, ListBox refreshList = null)
        {
            var t = DialogHost.Show(new PublishedEditFormDialog(target), "PublishedFormHost");
            if (refreshList != null)
                t.ContinueWith((t) => { refreshList.Items.Refresh();}, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.TryUpdateTarget();
            MaterialDesignThemes.Wpf.DialogHost.Close("PublishedFormHost");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.DialogHost.Close("PublishedFormHost");
        }
    }
}
