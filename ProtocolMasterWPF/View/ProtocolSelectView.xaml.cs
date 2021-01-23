using ProtocolMasterWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ProtocolSelectView.xaml
    /// </summary>
    public partial class ProtocolSelectView : UserControl
    {
        ProtocolSelectViewModel viewModel;
        public ProtocolSelectView()
        {
            viewModel = new ProtocolSelectViewModel();
            InitializeComponent();
            DataContext = viewModel;
        }
        private void DriveTab_Checked(object sender, RoutedEventArgs e)
        {
            SelectList.ItemsSource = viewModel.DriveOptions;
        }
        private void PublishedTab_Checked(object sender, RoutedEventArgs e)
        {
            SelectList.ItemsSource = viewModel.PublishedOptions;
        }
        private void LocalTab_Checked(object sender, RoutedEventArgs e)
        {
            SelectList.ItemsSource = viewModel.LocalOptions;
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.DialogHost.Close("SessionDialog",SelectList.SelectedItem);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.DialogHost.Close("SessionDialog");
        }
    }
}
