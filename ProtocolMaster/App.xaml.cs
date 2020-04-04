using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ProtocolMaster.Component;
using ProtocolMaster.Component.Google;


namespace ProtocolMaster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App Instance { get { return (App)Application.Current; }}
        public static MainWindow Window { get { return (MainWindow)Application.Current.MainWindow; } }
        public bool LoggedIn => Auth.Instance.isAuthenticated();

        void App_Startup(object sender, StartupEventArgs e)
        {
            Log.Error("ProtocolMaster Starting up");
            MainWindow = new MainWindow();
            MainWindow.Show();
            Log.Out("Application Data: " + Log.Instance.AppData);
        }

        // Full Login Routine
        public async Task LogIn()
        {
            await Auth.Instance.Authenticate(Drive.Instance, Sheets.Instance);
        }

        public async Task LogOut()
        {
            await Auth.Instance.DeAuthenticate();
        }

        public async void Window_Closed()
        {
            await Auth.Instance.DeAuthenticate();

            Log.Error("ProtocolMaster Mainwindow Exited Gracefully");
            Log.Instance.WriteFiles();
        }
    }
}
