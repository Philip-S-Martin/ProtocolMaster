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
        public DrivePane Drive { get; private set; }
        public LogPane Log { get; private set; }
        public PropertiesPane Properties { get; private set; }
        public TimelinePane Timeline { get; private set; }
        public VideoPane Video { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Drive = new DrivePane();
            Log = new LogPane();
            Properties = new PropertiesPane();
            Timeline = new TimelinePane();
            Video = new VideoPane();
            DriveView.Navigate(Drive);
            LogView.Navigate(Log);
            //PropertiesView.Navigate(Properties);
            TimelineView.Navigate(Timeline);
            VideoView.Navigate(Video);
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
            ProtocolMaster.Component.Debug.Log.Instance.OpenFolder();
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
