using MahApps.Metro.Controls;
using System;


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
