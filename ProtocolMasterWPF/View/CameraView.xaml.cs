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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        
        public CameraView()
        {
            InitializeComponent();
            DataContext = new CameraViewModel();
        }

    }
}
