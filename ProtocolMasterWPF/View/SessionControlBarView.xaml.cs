using MaterialDesignThemes.Wpf;
using ProtocolMasterWPF.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for SessionControlBarView.xaml
    /// </summary>
    public partial class SessionControlBarView : UserControl
    {
        private SessionControlViewModel _sessionControl;
        internal SessionControlViewModel SessionControl { get => _sessionControl; set { _sessionControl = value; DataContext = _sessionControl; } }
        public SessionControlBarView()
        {
            InitializeComponent();
            SessionControl = new SessionControlViewModel();
        }
        private void PreviewButton_Click(object sender, RoutedEventArgs e) => SessionControl.Preview();
        private void StartButton_Click(object sender, RoutedEventArgs e) => SessionControl.Start();
        private void StopButton_Click(object sender, RoutedEventArgs e) => SessionControl.Stop();
        private void ResetButton_Click(object sender, RoutedEventArgs e) => SessionControl.Reset();
        private void SelectDialog_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (eventArgs.Parameter == null) SessionControl.CancelSelection();
            else SessionControl.MakeSelection(eventArgs.Parameter);
        }
        private void SelectButton_Click(object sender, RoutedEventArgs e) => DialogHost.Show(new ProtocolSelectView(), SelectDialog_OnDialogClosing);
    }
}
