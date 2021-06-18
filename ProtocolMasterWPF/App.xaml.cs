using ProtocolMasterCore.Utility;
using ProtocolMasterWPF.Model;
using ProtocolMasterWPF.Model.Google;
using ProtocolMasterWPF.ViewModel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ProtocolMasterWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static LogViewModel LogVM { get; private set; }
        static App()
        {
            InitializeLog();
        }
        private static void InitializeLog()
        {
            LogVM = new LogViewModel();
            Log.ErrorPrinter += LogVM.AddLog;
            Log.OutputPrinter += LogVM.AddLog;
            Log.Out("Output Log Running");
            Log.Error("Error Log Running");
            Task logThreadTask = new Task(() => Log.Error("Log working in parallel thread"));
            logThreadTask.Start();
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += AppDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            GAuth.Instance.PostAuthentication += AuthenticationRefocus;
            LocalFileStore.Instance.RefreshFiles();
        }
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Error(e);
            Log.Error(e.Exception.StackTrace);
            Log.Flush();
            MessageBoxResult res = MessageBox.Show(e.Exception.Message, " ", MessageBoxButton.YesNo);
        }

        void AppDispatcherUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e);
            Log.Error(((Exception)e.ExceptionObject).StackTrace);
            Log.Flush();
            MessageBoxResult res = MessageBox.Show(((Exception)e.ExceptionObject).Message, "Unhandled Exception", MessageBoxButton.YesNo);
        }
        private void AuthenticationRefocus(object sender, EventArgs e)
        {
            MainWindow.Activate();
        }

        public async void GoogleAuthenticate()
        {
            await GAuth.Instance.Authenticate(GDrive.Instance);
        }
        public async void GoogleDeauthenticate()
        {
            await GAuth.Instance.DeAuthenticate();
        }

        public static void TryOpenURI(object sender, params string[] uris)
        {
            for (int i = 0; i < uris.Length; i++)
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = uris[i],
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                    return;
                }
                catch (Exception e) { Log.Error($"Failed to open {(sender as DependencyObject).GetValue(FrameworkElement.NameProperty)} URI. Exception:{e}"); }
            }
            Log.Error($"Failed to open any URIs for {(sender as DependencyObject).GetValue(FrameworkElement.NameProperty)}, please contact the developer.");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Log.Flush();
        }
    }
}
