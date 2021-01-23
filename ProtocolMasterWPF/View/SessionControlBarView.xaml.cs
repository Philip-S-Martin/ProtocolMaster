using MaterialDesignThemes.Wpf;
using ProtocolMasterWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        private void Sample2_DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
            => ProtocolLabel.Text = eventArgs.Parameter == null ? ProtocolLabel.Text : eventArgs.Parameter.ToString();
        private void SelectButton_Click(object sender, RoutedEventArgs e) => DialogHost.Show(new ProtocolSelectView(), Sample2_DialogHost_OnDialogClosing);
    }
}
