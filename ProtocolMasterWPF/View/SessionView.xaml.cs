using ProtocolMasterCore.Prompt;
using ProtocolMasterWPF.Model;
using System.Windows.Controls;
using ProtocolMasterWPF;

namespace ProtocolMasterWPF.View
{
    /// <summary>
    /// Interaction logic for SessionView.xaml
    /// </summary>
    public partial class SessionView : UserControl
    {
        internal Session SessionControl { get => _sessionControl; set { _sessionControl = value; DataContext = _sessionControl; } }
        private Session _sessionControl;
        public SessionView()
        {
            InitializeComponent();
            SessionControl = new Session();
            ControlBar.SessionControl = SessionControl;
            SessionControl.Protocol.InterpreterManager.OnEventsLoaded += Timeline.LoadPlotDataInUIThread;
            SessionControl.Protocol.DriverManager.OnProtocolStart += Timeline.StartAnimatorUIThread;
            SessionControl.OnStop += Timeline.StopAnimatorUIThread;
            SessionControl.OnReset += Timeline.ResetPlot;
            PromptTargetStore promptTargets = new PromptTargetStore();
            promptTargets.UserSelect = DropdownDialog.DropdownUserSelect;
            
            SessionControl.Protocol.DriverManager.PromptTargets = promptTargets;
            SessionControl.Protocol.InterpreterManager.PromptTargets = promptTargets;
        }
    }
}
