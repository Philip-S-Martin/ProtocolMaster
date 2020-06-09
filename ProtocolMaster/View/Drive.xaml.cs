using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for Drive.xaml
    /// </summary>
    public partial class Drive : UserControl
    {
        readonly Dictionary<string, TreeViewItem> idDictionary;

        public Drive()
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

            // Refresh Google Drive Private
            if (App.Instance.LoggedIn)
            {
                Model.Google.MarkupCallback callback = Display_Tree_Child;
                Model.Google.GDrive.Instance.Refresh(callback);
            }
        }

        public void Folder_Click(object sender, RoutedEventArgs e)
        {
            Model.Google.GDrive.Instance.CreateFolder("New Folder").Callback(Display_Tree_Child);
        }

        public void Display_Tree_Child(string parentName, string name, string header)
        {
            TreeViewItem child = new TreeViewItem();
            idDictionary.Add(name, child);
            child.Header = header;

            if (parentName != null)
            {
                idDictionary.TryGetValue(parentName, out TreeViewItem parent);
                if (parent != null)
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

