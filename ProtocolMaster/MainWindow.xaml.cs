using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using ProtocolMasterLib;
using AForge.Video;
using AForge.Video.DirectShow;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;



namespace ProtocolMasterGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        App app;
        public MainWindow()
        {
            InitializeComponent();
            app = (App)App.Current;
        }

        // Account Sign-in/Sign-out
        private async void Account_Click(object sender, RoutedEventArgs e)
        {
            if (app.LoggedIn)
            {
                AccountButton.IsEnabled = false;
                AccountButton.Content = "Signing-Out";
                await app.Logout();
                AccountButton.Content = "Sign-In";
                AccountButton.IsEnabled = true;
            }
            else
            {
                AccountButton.IsEnabled = false;
                AccountButton.Content = "Opening Browser";
                try
                {
                    await app.Login();
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



        // Window Closing Operations
        private void Window_Closed(object sender, EventArgs e)
        {
            app.Window_Closed();
        }
    }
}
