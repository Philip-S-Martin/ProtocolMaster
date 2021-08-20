using MaterialDesignThemes.Wpf;
using ProtocolMasterWPF.Model;
using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for SessionSettingsDialog.xaml
    /// </summary>
    public partial class SessionSettingsDialog : UserControl
    {
        Session session;
        internal SessionSettingsDialog(Session sessionControl)
        {
            DataContext = sessionControl;
            session = sessionControl;
            InitializeComponent();
        }
        internal static void ShowDialog(Session SessionControl) => DialogHost.Show(new SessionSettingsDialog(SessionControl), "SessionSettingsHost");

        private void DoneButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialDesignThemes.Wpf.DialogHost.Close("SessionSettingsHost");
        }

        private void ToggleCamera_Checked(object sender, RoutedEventArgs e)
        {
            session.Cam.StartRecord();
        }

        private void ToggleCamera_Unchecked(object sender, RoutedEventArgs e)
        {
            session.Cam.StopRecord();
        }
    }
}
