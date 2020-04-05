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
using ProtocolMaster.Component;
using ProtocolMaster.Component.Google;

namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class DrivePane : Page
    {
        Dictionary<string, TreeViewItem> idDictionary;

        public DrivePane()
        {
            idDictionary = new Dictionary<string, TreeViewItem>();
            InitializeComponent();
        }


        // Account Sign-in/Sign-out
        private async void Account_Click(object sender, RoutedEventArgs e)
        {
            if (App.Instance.LoggedIn)
            {
                AccountButton.IsEnabled = false;
                AccountButton.ToolTip = "Signing-Out";
                await App.Instance.LogOut();
                AccountButton.Header = "Sign-In";
                AccountButton.ToolTip = "Google Drive";
                AccountButton.IsEnabled = true;
            }
            else
            {
                AccountButton.IsEnabled = false;
                AccountButton.ToolTip = "Opening Browser";
                try
                {
                    await App.Instance.LogIn();
                }
                catch (OperationCanceledException)
                {
                    AccountButton.Header = "Sign-In";
                    AccountButton.IsEnabled = true;
                    return;
                }
                AccountButton.Header = "Sign-Out";
                AccountButton.ToolTip = "Google Drive";
                AccountButton.IsEnabled = true;
            }
            Refresh();
        }

        public void Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {
            FileTree.Items.Clear();
            idDictionary.Clear();
            
            MarkupCallback callback = Display_Tree_Child;
            Drive.Instance.Refresh(callback);
        }

        public void Folder_Click(object sender, RoutedEventArgs e)
        {
            Drive.Instance.CreateFolder("New Folder").Callback(Display_Tree_Child);
        }

        public void Display_Tree_Child(string parentName, string name, string header)
        {
            TreeViewItem child = new TreeViewItem();
            idDictionary.Add(name, child);
            child.Header = header;

            if (parentName != null)
            {
                TreeViewItem parent;
                idDictionary.TryGetValue(parentName, out parent);
                if(parent != null)
                    parent.Items.Add(child);
                else
                    FileTree.Items.Add(child);
            }
            else
            {
                FileTree.Items.Add(child);
            }
        }
    }

    
}
