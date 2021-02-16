using MaterialDesignThemes.Wpf;
using ProtocolMasterCore.Protocol;
using ProtocolMasterWPF.Model;
using ProtocolMasterWPF.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using Windows.Devices.Enumeration;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for SessionControlBarView.xaml
    /// </summary>
    public partial class SessionControlBarView : UserControl
    {
        internal Session SessionControl { get => _sessionControl; set { _sessionControl = value; DataContext = _sessionControl; InitDefaults(); } }
        private Session _sessionControl;
        public SessionControlBarView()
        {
            InitializeComponent();
        }
        private void InitDefaults()
        {
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
        private void SelectButton_Click(object sender, RoutedEventArgs e) => DialogHost.Show(new ProtocolSelectView(), "SessionDialog", SelectDialog_OnDialogClosing);
        public void UpdateTime(double elapsed, double duration) => App.Current.Dispatcher.Invoke(() => UpdateTimeLocal(elapsed, duration));
        private void UpdateTimeLocal(double elapsed, double duration)
        {
            ElapsedLabel.Text = DateTime.FromOADate(elapsed).ToString("HH:mm:ss");
            DurationLabel.Text = DateTime.FromOADate(duration).ToString("HH:mm:ss");
            TimeProgressBar.Value = 100f * elapsed / duration;
        }

    }
}
