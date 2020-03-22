using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;
using ProtocolMaster.Component;
using ProtocolMaster.Component.Google;

namespace ProtocolMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
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
                catch(OperationCanceledException)
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

        }

        // Log Functions
        public void Log(string logString)
        {
            Paragraph addLog = new Paragraph();
            addLog.Inlines.Add(logString);
            LogDocument.Blocks.Add(addLog);
            
            addLog.BringIntoView();
        }

        public void Log_Folder_Click(object sender, RoutedEventArgs e)
        {
            ProtocolMaster.Component.Log.Instance.OpenFolder();
        }
        public void Log_Test(object sender, RoutedEventArgs e)
        {
            ProtocolMaster.Component.Log.Error("Test");
        }

        // Window Closing Operations
        private void Window_Closed(object sender, EventArgs e)
        {
            App.Instance.Window_Closed();
        }
    }
}
