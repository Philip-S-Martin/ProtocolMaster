using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ProtocolMaster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static App Instance { get { return (App)Application.Current; } }
        public static View.MainWindow Window { get { return (View.MainWindow)Application.Current.MainWindow; } }
        public bool LoggedIn => Model.Google.GAuth.Instance.IsAuthenticated();

        internal Model.Protocol.ExtensionSystem ExtensionSystem { get; private set; }
        void App_Startup(object sender, StartupEventArgs e)
        {
            ExtensionSystem = new Model.Protocol.ExtensionSystem();

            Model.Debug.Log.Error("ProtocolMaster Starting up");
            MainWindow = new View.MainWindow();
            MainWindow.Show();
            Model.Debug.Log.Out("Application Data: " + Model.Debug.Log.Instance.AppData);

            ExtensionSystem.LoadExtensions();
        }

        // Full Login Routine
        public async Task LogIn()
        {
            await Model.Google.GAuth.Instance.Authenticate(Model.Google.GDrive.Instance, Model.Google.GSheets.Instance);
        }

        public async Task LogOut()
        {
            await Model.Google.GAuth.Instance.DeAuthenticate();
        }

        public async void Window_Closed()
        {
            ExtensionSystem.Cancel();
            await Model.Google.GAuth.Instance.DeAuthenticate();


            Model.Debug.Log.Error("ProtocolMaster Mainwindow Exited Gracefully");
            Model.Debug.Log.Instance.WriteFiles();
        }
    }
}
