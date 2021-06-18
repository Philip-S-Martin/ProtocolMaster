using ProtocolMasterWPF.Model;
using System;
using System.Collections.Generic;
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
        public static readonly DependencyProperty NewNameProperty =
            DependencyProperty.Register("NewName", typeof(string), typeof(PublishedSelectView));
        public string NewName
        {
            get { return (string)GetValue(NewNameProperty); }
            set { SetValue(NewNameProperty, value); }
        }
        public static readonly DependencyProperty NewURLProperty =
            DependencyProperty.Register("NewURL", typeof(string), typeof(PublishedSelectView));
        public string NewURL
        {
            get { return (string)GetValue(NewURLProperty); }
            set { SetValue(NewURLProperty, value); }
        }

        private int _numberOfValidationErrors = 0;
        public bool HasNoValidationErrors => _numberOfValidationErrors == 0;

        private void HandleValidationError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _numberOfValidationErrors++;
            else
                _numberOfValidationErrors--;
            AddButton.IsEnabled = HasNoValidationErrors;
        }
        public PublishedSelectView()
        {
            InitializeComponent();
        }

        public ListBox SelectList => SelectListBox;


        private void DeleteButton_Click(object sender, RoutedEventArgs e) => PublishedFileStore.Instance.Remove(SelectListBox.SelectedValue as PublishedFileStreamer);
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            PublishedFileStore.Instance.Add(NewName, NewURL);
            NewName = "";
            NewURL = "";
            AddPopup.IsPopupOpen = false;
        }
    }
}
