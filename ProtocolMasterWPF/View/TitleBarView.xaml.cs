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

        int logCounter = 0;
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProtocolMasterCore.Utility.Log.Out($"Test: {logCounter}");
        }
    }
}
