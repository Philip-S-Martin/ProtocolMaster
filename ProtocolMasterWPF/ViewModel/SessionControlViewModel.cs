using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtocolMasterWPF.ViewModel
{
    internal enum SessionState
    {
        NotReady,
        Selecting,
        Ready,
        Running,
        Viewing,
    }
    internal class SessionControlViewModel : ViewModelBase
    {
        private Object selection;
        SessionState state = SessionState.NotReady;
        SessionState State { get => state; set { state = value; NotifyStateProperties(); } }
        private Object Selection
        {
            get => selection;
            set
            {
                selection = value;
                // Check validity and set state!
                if (selection != null) State = SessionState.Ready;
                else State = SessionState.NotReady;
                OnPropertyChanged();
            }
        }
        public bool CanStart { get => State == SessionState.Ready; }
        public bool CanStop { get => State == SessionState.Running; }
        public bool CanPreview { get => State == SessionState.Ready; }
        public bool CanReset { get => State == SessionState.Viewing; }
        public bool CanSelect { get => State == SessionState.Ready || State == SessionState.NotReady; }
        public bool IsSelecting { get => State == SessionState.Selecting; }

        public SessionControlViewModel()
        {
            Selection = new object();
        }
        private void NotifyStateProperties()
        {
            OnPropertyChanged("CanStart");
            OnPropertyChanged("CanStop");
            OnPropertyChanged("CanPreview");
            OnPropertyChanged("CanReset");
            OnPropertyChanged("CanSelect");
            OnPropertyChanged("IsSelecting");
        }
        public void Start(bool overrideCheck = false)
        {
            if (CanStart || overrideCheck)
            {
                State = SessionState.Running;
            }
            else throw new Exception("Cannot start in state " + State.ToString());
        }
        public void Stop(bool overrideCheck = false)
        {
            if (CanStop || overrideCheck)
            {
                State = SessionState.Viewing;
            }
            else throw new Exception("Cannot stop in state " + State.ToString());
        }
        public void Reset(bool overrideCheck = false)
        {
            if (CanReset || overrideCheck)
            {
                if(Selection != null)
                    State = SessionState.Ready;
                else
                    State = SessionState.NotReady;
            }
            else throw new Exception("Cannot reset in state " + State.ToString());
        }
        public void Preview(bool overrideCheck = false)
        {
            if (CanPreview || overrideCheck)
            {
                State = SessionState.Viewing;
            }
            else throw new Exception("Cannot preview in state " + State.ToString());
        }
        public void OpenSelection(bool overrideCheck = false)
        {
            if (CanSelect || overrideCheck)
            {
                State = SessionState.Selecting;
            }
            else throw new Exception("Cannot open selection in state " + State.ToString());
        }
        public void MakeSelection()
        {
            Selection = new Object();
            Reset(true);
        }
        public void CancelSelection()
        {
            Selection = null;
        }
    }
}
