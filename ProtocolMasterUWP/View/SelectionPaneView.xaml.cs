using ProtocolMasterUWP.Observable;
using ProtocolMasterUWP.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ProtocolMasterUWP.View
{
    public sealed partial class SelectionPaneView : UserControl
    {
        ObservableCollection<TreeItem> DataSource;
        internal SessionControlViewModel SessionControl { get; set; }

        public SelectionPaneView()
        {
            this.InitializeComponent();
            DataSource = PopulateChildren();

        }
        private ObservableCollection<TreeItem> PopulateChildren()
        {
            var list = new ObservableCollection<TreeItem>();
            list.Add(new TreeItem { Name = "Tree0", Children = new ObservableCollection<TreeItem> { new TreeItem { Name = "A" }, new TreeItem { Name = "B" }, new TreeItem { Name = "C" } } });
            list.Add(new TreeItem { Name = "Tree1", Children = new ObservableCollection<TreeItem> { new TreeItem { Name = "A" }, new TreeItem { Name = "B" }, new TreeItem { Name = "C" } } });
            list.Add(new TreeItem { Name = "Tree2", Children = new ObservableCollection<TreeItem> { new TreeItem { Name = "A" }, new TreeItem { Name = "B" }, new TreeItem { Name = "C" } } });
            list.Add(new TreeItem { Name = "Tree3", Children = new ObservableCollection<TreeItem> { new TreeItem { Name = "A" }, new TreeItem { Name = "B" }, new TreeItem { Name = "C" } } });
            list.Add(new TreeItem { Name = "Tree4", Children = new ObservableCollection<TreeItem> { new TreeItem { Name = "A" }, new TreeItem { Name = "B" }, new TreeItem { Name = "C" } } });
            list.Add(new TreeItem { Name = "Tree5", Children = new ObservableCollection<TreeItem> { new TreeItem { Name = "A" }, new TreeItem { Name = "B" }, new TreeItem { Name = "C" } } });
            return list;
        }
        private void FileSelect_Click(object sender, RoutedEventArgs e) => SessionControl.MakeSelection();
        private void CancelSelect_Click(object sender, RoutedEventArgs e) => SessionControl.CancelSelection();
    }
}
