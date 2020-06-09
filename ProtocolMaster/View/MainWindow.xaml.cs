using MahApps.Metro.Controls;
using System;
using System.IO;

namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Click_Documentation(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://sites.google.com/view/protocolmaster/docs");
        }

        private void Click_Extension_Folder(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "C:\\Users\\phili\\source\\repos\\Philip-S-Martin\\ProtocolMaster\\ProtocolMaster\\bin\\Debug\\net48\\Extensions",
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void Click_Protocol_Folder(object sender, EventArgs e)
        {
            string target = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\ProtocolMaster\\Protocols";
            Directory.CreateDirectory(target);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = target,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void Click_Log_Folder(object sender, EventArgs e)
        {
            ProtocolMaster.Model.Debug.Log.Instance.OpenFolder();
        }

        private void OnLoad(object sender, EventArgs e)
        {

        }

        // Window Closing Operations
        private void Window_Closed(object sender, EventArgs e)
        {
            App.Instance.Window_Closed();
        }
    }
}
