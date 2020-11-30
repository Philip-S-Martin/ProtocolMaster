using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

        #region Single Process Management
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        // Pinvoke declaration for ShowWindow
        private const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        Mutex pmMutex;
        #endregion
        void App_Startup(object sender, StartupEventArgs e)
        {
            bool isNewInstance = false;
            pmMutex = new Mutex(true, "ProtocolMaster", out isNewInstance);
            if (!isNewInstance)
            {
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        ShowWindow(process.MainWindowHandle, SW_SHOWMAXIMIZED);
                        SetForegroundWindow(process.MainWindowHandle);
                        break;
                    }
                }
                App.Current.Shutdown();
            }
            RegisterUriScheme();

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
            //ExtensionSystem.Terminate();
            await Model.Google.GAuth.Instance.DeAuthenticate();

            Model.Debug.Log.Error("ProtocolMaster Mainwindow Exited Gracefully");
            Model.Debug.Log.Instance.WriteFiles();
        }

        const string UriScheme = "protocolmaster";
        const string FriendlyName = "Protocol master";

        public static void RegisterUriScheme()
        {
            if (!RegistryUriExists())
            {
                using (var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + UriScheme))
                {
                    // Replace typeof(App) by the class that contains the Main method or any class located in the project that produces the exe.
                    // or replace typeof(App).Assembly.Location by anything that gives the full path to the exe
                    string applicationLocation = typeof(App).Assembly.Location;

                    key.SetValue("", "URL:" + FriendlyName);
                    key.SetValue("URL Protocol", "");

                    using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                    {
                        defaultIcon.SetValue("", applicationLocation + ",1");
                    }

                    using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                    {
                        commandKey.SetValue("", "\"" + applicationLocation + "\" \"%1\"");
                    }
                }
            }
        }

        public static bool RegistryUriExists()
        {
            string pmSubkey = "SOFTWARE\\Classes\\" + UriScheme;
            foreach (string subkey in Registry.CurrentUser.GetSubKeyNames())
            {
                if (subkey == pmSubkey)
                    return true;
            }
            return false;
        }
    }
}
