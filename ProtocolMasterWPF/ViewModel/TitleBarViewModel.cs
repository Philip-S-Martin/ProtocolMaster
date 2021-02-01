using ProtocolMasterWPF.Model.Google;

namespace ProtocolMasterWPF.ViewModel
{
    public class TitleBarViewModel
    {
        public GAuth Auth { get => GAuth.Instance; }
        public TitleBarViewModel()
        {
        }
    }
}
