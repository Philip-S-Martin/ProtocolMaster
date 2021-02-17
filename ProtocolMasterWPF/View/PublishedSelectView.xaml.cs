using ProtocolMasterWPF.Model;
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
    /// Interaction logic for PublishedSelectView.xaml
    /// </summary>
    public partial class PublishedSelectView : UserControl, ISelectView
    {
        public PublishedSelectView()
        {
            InitializeComponent();
        }

        public ListBox SelectList => SelectListBox;

        private void DeleteButton_Click(object sender, RoutedEventArgs e) => PublishedFileStore.Instance.Remove(SelectListBox.SelectedValue as PublishedFileStreamer);
        private void AddButton_Click(object sender, RoutedEventArgs e) => PublishedFileStore.Instance.Add(NewNameText.Text, NewURLText.Text);
    }
}
