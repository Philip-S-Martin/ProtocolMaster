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
using ProtocolMaster.Component;
using ProtocolMaster.Component.Google;

namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class DrivePane : Page
    {
        public DrivePane()
        {
            InitializeComponent();
        }

        // Account Sign-in/Sign-out
        private async void Account_Click(object sender, RoutedEventArgs e)
        {
            if (App.Instance.LoggedIn)
            {
                AccountButton.IsEnabled = false;
                AccountButton.Content = "Signing-Out";
                await App.Instance.LogOut();
                AccountButton.Content = "Sign-In";
                AccountButton.IsEnabled = true;
            }
            else
            {
                AccountButton.IsEnabled = false;
                AccountButton.Content = "Opening Browser";
                try
                {
                    await App.Instance.LogIn();
                }
                catch (OperationCanceledException)
                {
                    AccountButton.Content = "Sign-In";
                    AccountButton.IsEnabled = true;
                    return;
                }
                AccountButton.Content = "Sign-Out";
                AccountButton.IsEnabled = true;
            }
        }

        public void Refresh_Click(object sender, RoutedEventArgs e)
        {
            FileTree.Items.Clear();
            MarkupCallback callback = Display_Tree_Child;
            Drive.Instance.CallbackPreBF(callback);
        }

        public void Display_Tree_Child(string parentName, string name, string header)
        {
            TreeViewItem child = new TreeViewItem();
            child.Resources.Add(name, "");
            child.Header = header;

            if (parentName != null)
            {
                TreeViewItem parent;
                parent = (TreeViewItem)FileTree.FindResource(parentName);
                parent.Items.Add(child);
            }
            else
            {
                FileTree.Items.Add(child);
            }
        }
    }

    
}
