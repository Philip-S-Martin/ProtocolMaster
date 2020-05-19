using ProtocolMaster.Component.Debug;
using ProtocolMaster.Component.Google;
using ProtocolMaster.Component.Model;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ProtocolMaster.View;

namespace ProtocolMaster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App Instance { get { return (App)Application.Current; } }
        public static MainWindow Window { get { return (MainWindow)Application.Current.MainWindow; } }
        public bool LoggedIn => Auth.Instance.IsAuthenticated();

        internal ExtensionSystem Extensions { get; private set; }
        void App_Startup(object sender, StartupEventArgs e)
        {
            Log.Error("ProtocolMaster Starting up");
            MainWindow = new MainWindow();
            MainWindow.Show();
            Log.Out("Application Data: " + Log.Instance.AppData);

            Extensions = new ExtensionSystem();
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
