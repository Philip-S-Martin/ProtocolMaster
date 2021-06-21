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
            App.Current.Dispatcher.Invoke(() =>
            {
                Exception ex = e.Exception.InnerException;
                Log.Error(ex);
                Log.Error(ex.StackTrace);
                Log.Flush();
                MessageBoxResult res = MessageBox.Show(App.Current.MainWindow, $"An exception occurred in a task:\n\n{ex.Message}\n\nWould you like to see additional data?", "Unhandled Exception in Task", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                    MessageBox.Show(App.Current.MainWindow, $"Message:\n\n{ex.Message}\n\nStack Trace:\n\n{ex.StackTrace}", "Exception Information", MessageBoxButton.OK);
                MessageBox.Show(App.Current.MainWindow, $"The application will now continue.\n\nPlease consider restarting the application if you believe that it may prevent this error from occuring again..", "Application will Continue", MessageBoxButton.OK);
            });
        }
        void AppDispatcherUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Log.Error(ex.Message);
            Log.Error(ex.StackTrace);
            Log.Flush();
            MessageBoxResult res = MessageBox.Show(App.Current.MainWindow, $"A terminating exception occurred:\n\n{ex.Message}\n\nWould you like to see additional data?", "Unhandled Exception", MessageBoxButton.YesNo);
            if (res == MessageBoxResult.Yes)
                MessageBox.Show(App.Current.MainWindow, $"Message:\n\n{ex.Message}\n\nStack Trace:\n\n{ex.StackTrace}", "Exception Information", MessageBoxButton.OK);
            MessageBox.Show(App.Current.MainWindow, $"The application will now close.", "Application closing", MessageBoxButton.OK);
            App.Current.MainWindow.Close();
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
