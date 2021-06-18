using ProtocolMasterWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for PublishedSelectView.xaml
    /// </summary>
    public partial class PublishedSelectView : UserControl, ISelectView
    {
        public PublishedSelectView()
        {
            InitializeComponent();
        }

        public ListBox SelectList => SelectListBox;
        internal ObservableCollection<PublishedFileStreamer> FileStore => PublishedFileStore.Instance.PublishedFiles;

        private void DeleteButton_Click(object sender, RoutedEventArgs e) => PublishedFileStore.Instance.Remove(SelectListBox.SelectedValue as PublishedFileStreamer);
        private void AddButton_Click(object sender, RoutedEventArgs e) => PublishedFormDialog.ShowDialog();

        private void CopyLinkButton_Click(object sender, RoutedEventArgs e)
        {
            if(SelectList.SelectedItem != null) Clipboard.SetText(((PublishedFileStreamer)SelectList.SelectedItem).URL);
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectList.SelectedItem != null) PublishedEditFormDialog.ShowDialog((PublishedFileStreamer)SelectList.SelectedItem, SelectList);
        }

        private void DownloadLocalButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectList.SelectedItem != null) ((PublishedFileStreamer)SelectList.SelectedItem).DownloadToLocal("xlsx");
        }
    }
}
