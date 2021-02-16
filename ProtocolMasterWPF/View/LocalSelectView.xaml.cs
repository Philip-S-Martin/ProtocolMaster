using ProtocolMasterCore.Utility;
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
    /// Interaction logic for LocalSelectView.xaml
    /// </summary>
    public partial class LocalSelectView : UserControl, ISelectView
    {
        public ListBox SelectList { get => SelectListBox; }
        public LocalSelectView()
        {
            InitializeComponent();
        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e) => LocalProtocolStore.Instance.RefreshFiles();
        private void OpenFolderButton_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, AppEnvironment.GetLocation("Protocols"));
    }
}
