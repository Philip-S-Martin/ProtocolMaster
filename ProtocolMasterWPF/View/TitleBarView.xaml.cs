using ProtocolMasterCore.Utility;
using ProtocolMasterWPF.ViewModel;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for TitleBarView.xaml
    /// </summary>
    public partial class TitleBarView : UserControl
    {
        public TitleBarView()
        {
            InitializeComponent();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            if (window.WindowState == WindowState.Normal) window.WindowState = WindowState.Maximized;
            else window.WindowState = WindowState.Normal;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Close();
        }
        private void LogTest_Click(object sender, RoutedEventArgs e)
        {
            ProtocolMasterCore.Utility.Log.Out($"Testing output log");
            ProtocolMasterCore.Utility.Log.Error($"Testing error log");
        }
        private void GoogleAuthButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).GoogleAuthenticate();
        }
        private void GoogleDeauthButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)App.Current).GoogleDeauthenticate();
        }
        private void OpenWebsiteHome_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, "https://protocolmaster.philipm.net/home", "https://sites.google.com/view/protocolmaster/home?authuser=0");
        private void OpenGithub_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, "https://github.com/Philip-S-Martin/ProtocolMaster");
        private void OpenReleaseNotes_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, "https://github.com/Philip-S-Martin/ProtocolMaster/releases");
        private void OpenLogFolder_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, AppEnvironment.GetLocation("Log"));
        private void OpenVideoFolder_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, AppEnvironment.GetLocation("Video"));
        private void OpenExtensionFolder_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, AppEnvironment.GetLocation("Extensions"));
        private void OpenWebsiteGettingStarted_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, "https://protocolmaster.philipm.net/docs/getting-started", "https://sites.google.com/view/protocolmaster/docs/getting-started");
        private void OpenWebsiteUserGuide_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, "https://protocolmaster.philipm.net/docs/user-guide", "https://sites.google.com/view/protocolmaster/docs/user-guide");
        private void OpenWiki_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, "https://github.com/Philip-S-Martin/ProtocolMaster/wiki");
        private void OpenProtocolsFolder_Click(object sender, RoutedEventArgs e) => App.TryOpenURI(sender, AppEnvironment.GetLocation("Protocols"));
    }
}
