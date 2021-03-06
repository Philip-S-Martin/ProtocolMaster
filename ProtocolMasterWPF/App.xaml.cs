﻿using ProtocolMasterCore.Utility;
using ProtocolMasterWPF.Model.Google;
using ProtocolMasterWPF.ViewModel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace ProtocolMasterWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public LogViewModel LogVM { get; private set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InitializeLog();
            GAuth.Instance.PostAuthentication += AuthenticationRefocus;
        }
        private void InitializeLog()
        {
            LogVM = new LogViewModel();
            Log.ErrorPrinter += AddTextDispatched;
            Log.OutputPrinter += AddTextDispatched;
            Log.Out("Output Log Running");
            Log.Error("Error Log Running");
            Task logThreadTask = new Task(() => Log.Error("Log working in parallel thread"));
            logThreadTask.Start();
        }

        private void AddTextDispatched(string text)=>App.Current.Dispatcher.Invoke(() => { LogVM.LogText.Add(text); });

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
    }
}
