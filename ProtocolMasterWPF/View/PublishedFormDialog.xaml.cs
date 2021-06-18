using MaterialDesignThemes.Wpf;
using ProtocolMasterWPF.Model;
using ProtocolMasterWPF.ViewModel;
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
    /// Interaction logic for PublishedFormDialog.xaml
    /// </summary>
    public partial class PublishedFormDialog : UserControl
    {
        PublishedProtocolDataViewModel viewModel;
        public PublishedFormDialog()
        {
            viewModel = new PublishedProtocolDataViewModel();
            DataContext = viewModel;
            InitializeComponent();
        }
        public static void ShowDialog()
        {
            DialogHost.Show(new PublishedFormDialog(), "PublishedFormHost");
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            PublishedFileStore.Instance.Add(viewModel.Label, viewModel.URL);
            MaterialDesignThemes.Wpf.DialogHost.Close("PublishedFormHost");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.DialogHost.Close("PublishedFormHost");
        }
    }
}
