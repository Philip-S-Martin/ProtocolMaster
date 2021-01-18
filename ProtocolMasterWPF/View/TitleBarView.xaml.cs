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

        private void AppTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var window = App.Current.MainWindow;
            if (e.ChangedButton == MouseButton.Left)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    Point mousePos = Mouse.GetPosition(window);
                    double relativeX = mousePos.X / window.Width / 2;
                    double relativeY = mousePos.Y / window.Height / 2;
                    window.WindowState = WindowState.Normal;
                    window.Left = mousePos.X - window.Width*relativeX;
                    window.Top = mousePos.Y - window.Height*relativeY;
                }
                App.Current.MainWindow.DragMove();
            }
        }
    }
}
