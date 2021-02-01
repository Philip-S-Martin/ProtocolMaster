using ProtocolMasterWPF.ViewModel;
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
            DataContext = new TitleBarViewModel();
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
    }
}
