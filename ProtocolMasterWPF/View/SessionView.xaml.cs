using ProtocolMasterCore.Prompt;
using ProtocolMasterWPF.Model;
using System.Windows.Controls;
using ProtocolMasterWPF;
using ProtocolMasterWPF.ViewModel;

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
            SessionControl = new Session();

            PromptTargetStore promptTargets = new PromptTargetStore();
            promptTargets.UserSelect = DropdownDialog.DropdownUserSelect;
            promptTargets.UserNumber = DropdownDialog.DropdownUserNumber;

            SessionControl.Protocol.DriverManager.PromptTargets = promptTargets;
            SessionControl.Protocol.InterpreterManager.PromptTargets = promptTargets;

            InitializeComponent();
            ControlBar.SessionControl = SessionControl;
            CamView.DataContext = new CameraViewModel(SessionControl.Cam);
            SessionControl.Protocol.InterpreterManager.OnEventsLoaded += Timeline.LoadPlotDataInUIThread;
            SessionControl.OnStart += Timeline.StartTime;
            SessionControl.OnStop += Timeline.StopTime;
            SessionControl.OnReset += Timeline.ResetPlot;
            SessionControl.OnReset += ControlBar.ResetTime;
            SessionControl.Animator.OnUpdate += Timeline.UpdateTime;
            SessionControl.Animator.OnUpdate += ControlBar.UpdateTime;

        }
    }
}
