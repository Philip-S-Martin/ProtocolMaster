using System.Windows.Controls;

namespace ProtocolMaster.View
{
    /// <summary>
    /// Interaction logic for TimelinePane.xaml
    /// </summary>
    public partial class TimelinePane : Page
    {
        public TimelinePane()
        {
            InitializeComponent();
        }

        public void ListDriver(string name)
        {
            MenuItem newDriver = new MenuItem();
            newDriver.Header = name;
            DriverDropdown.Items.Add(newDriver);
        }
    }
}
