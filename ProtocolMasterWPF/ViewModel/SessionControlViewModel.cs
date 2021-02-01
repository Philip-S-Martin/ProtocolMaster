using ProtocolMasterCore.Utility;
using ProtocolMasterWPF.Model;
using System;

namespace ProtocolMasterWPF.ViewModel
{
    internal class SessionControlViewModel : ViewModelBase
    {
        Session ViewSession { get; set; }
        SessionState state = SessionState.NotReady;
        SessionState State { get => state; set { state = value; NotifyStateProperties(); } }
        public IStreamStarter _selection;
        public IStreamStarter Selection
        {
            get => _selection;
            private set
            {
                _selection = value;
                // Check validity and set state!
                if (_selection.GetType() != typeof(NoFileSelection)) State = SessionState.Ready;
                else State = SessionState.NotReady;
                OnPropertyChanged();
                OnPropertyChanged("SelectionObject");
            }
        }
        public object SelectionObject => (object)Selection;
        public bool CanStart { get => State == SessionState.Ready; }
        public bool CanStop { get => State == SessionState.Running; }
        public bool CanPreview { get => State == SessionState.Ready; }
        public bool CanReset { get => State == SessionState.Viewing; }
        public bool CanSelect { get => State == SessionState.Ready || State == SessionState.NotReady; }
        public bool IsSelecting { get => State == SessionState.Selecting; }

        public SessionControlViewModel(Session viewSession)
        {
            Selection = new NoFileSelection();
            ViewSession = viewSession;
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
                if (Selection != null)
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
        public void MakeSelection(object select)
        {
            if (typeof(IStreamStarter).IsAssignableFrom(select.GetType()))
            {
                Selection = (IStreamStarter)select;
                Log.Error($"Making selection: {Selection}");
                Reset(true);
            }
            else
            {
                Log.Error($"Object {Selection} not of type {typeof(IStreamStarter)}");
                CancelSelection();
            }
        }
        public void CancelSelection()
        {
            Log.Error($"Cancelling selection");
        }
    }
}
