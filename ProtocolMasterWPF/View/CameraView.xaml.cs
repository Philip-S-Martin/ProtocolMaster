using ProtocolMasterWPF.ViewModel;
using System.Windows.Controls;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        CameraViewModel ViewModel { get; set; }
        public CameraView()
        {
            InitializeComponent();
            ViewModel = new CameraViewModel();
            DataContext = ViewModel;
        }
    }
}
