using ProtocolMasterCore.Protocol;
using ProtocolMasterCore.Utility;
using ProtocolMasterWPF.Helpers;
using ProtocolMasterWPF.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace ProtocolMasterWPF.Model
{
    internal enum SessionState
    {
        NotReady,
        Selecting,
        Ready,
        Running,
        Viewing,
    }
    public delegate void SessionActionCallback();
    internal class Session : Observable
    {
        public InterpretAndDriveProtocol Protocol { get; private set; }
        private string extensionDir;
        CancellationTokenSource CancelSource { get; set; }
        CancellationToken CancelToken { get; set; }
        public List<IExtensionMeta> InterpreterOptions { get => _interpreterOptions; private set { _interpreterOptions = value; NotifyProperty(); } }
        private List<IExtensionMeta> _interpreterOptions;
        public IExtensionMeta SelectedInterpreter
        {
            get => Protocol.InterpreterManager.Selected;
            set
            {
                if (value == null) return;
                Protocol.InterpreterManager.Selected = value;
                NotifyProperty();
                if (Settings.Default.InterpreterMeta != value)
                {
                    Settings.Default.InterpreterMeta = new ExtensionMetaSetting(value);
                    Settings.Default.Save();
                }
            }
        }
        public List<IExtensionMeta> DriverOptions { get => _driverOptions; private set { _driverOptions = value; NotifyProperty(); } }
        private List<IExtensionMeta> _driverOptions;
        public IExtensionMeta SelectedDriver
        {
            get => Protocol.DriverManager.Selected;
            set
            {
                if (value == null) return;
                Protocol.DriverManager.Selected = value;
                NotifyProperty();
                if (Settings.Default.DriverMeta != value)
                {
                    Settings.Default.DriverMeta = new ExtensionMetaSetting(value);
                    Settings.Default.Save();
                }
            }
        }
        public ClockAnimator Animator { get; private set; }
        public MediaProperties Cam { get; private set; }
        SessionState State { get => _state; set { _state = value; NotifyStateProperties(); } }
        SessionState _state = SessionState.NotReady;
        public Streamer Selection
        {
            get => _selection;
            private set
            {
                _selection = value;
                // Check validity and set state!
                if (_selection.GetType() != typeof(NoFileSelection)) State = SessionState.Ready;
                else State = SessionState.NotReady;
                NotifyProperty();
                NotifyProperty("SelectionObject");
            }
        }
        public Streamer _selection;
        public object SelectionObject { get { if (Selection != null) return (object)Selection; else return "No Protocol Selected"; } }
        public bool CanStart { get => State == SessionState.Ready; }
        public bool CanStop { get => State == SessionState.Running; }
        public bool CanPreview { get => State == SessionState.Ready; }
        public bool CanReset { get => State == SessionState.Viewing; }
        public bool CanSelect { get => State == SessionState.Ready || State == SessionState.NotReady; }
        public bool IsSelecting { get => State == SessionState.Selecting; }
        public SessionActionCallback OnStart, OnStop, OnReset, OnPreview, OnRun;
        public Session()
        {
            AppEnvironment.TryAddLocationAssembly("Extensions", "Extensions", out extensionDir);
            Protocol = new InterpretAndDriveProtocol(extensionDir);
            Protocol.InterpreterManager.OnOptionsLoaded += LoadInterpreterOptions;
            Protocol.DriverManager.OnOptionsLoaded += LoadDriverOptions;
            Animator = new ClockAnimator();
            Protocol.InterpreterManager.OnEventsLoaded += Animator.FindMaxTime;
            Protocol.DriverManager.OnProtocolStart += Animator.StartAnimatorNow;
            Protocol.DriverManager.OnProtocolEnd += Animator.StopAnimator;

            InitDefaultExtensions();


            Cam = new MediaProperties();
            Protocol.InterpreterManager.OnEventsLoaded += Cam.SetLabel;
            Protocol.InterpreterManager.OnEventsLoaded += this.LogLabel;
            Protocol.DriverManager.OnProtocolStart += Cam.StartRecord;
            Protocol.DriverManager.OnProtocolStart += this.LogStart;
            Protocol.DriverManager.OnProtocolEnd += this.LogEnd;
            OnStop += Cam.StopRecord;
        }
        public void LogLabel(List<ProtocolEvent> events, string label) => Log.Out($"Loaded Protocol: {label}");
        public void LogStart() => Log.Out($"Protocol Begin Execution");
        public void LogEnd() => Log.Out($"Protocol End Execution");
        public void InitDefaultExtensions()
        {
            Protocol.LoadExtensions();
            try
            {
                SelectedDriver = Settings.Default.DriverMeta;
            }
            catch (ArgumentException e)
            {
                Log.Error($"Default Driver could not be seleted, exception: {e}");
                SelectedDriver = Protocol.DriverManager.Selected;
            }
            catch (NullReferenceException)
            {
                SelectedDriver = Protocol.DriverManager.Selected;
            }
            try
            {
                SelectedInterpreter = Settings.Default.InterpreterMeta;
            }
            catch (ArgumentException e)
            {
                Log.Error($"Default Interpreter could not be seleted, exception: {e}");
                SelectedInterpreter = Protocol.InterpreterManager.Selected;
            }
            catch (NullReferenceException)
            {
                SelectedDriver = Protocol.DriverManager.Selected;
            }
        }
        private void LoadInterpreterOptions(List<IExtensionMeta> options) => InterpreterOptions = options;
        private void LoadDriverOptions(List<IExtensionMeta> options) => DriverOptions = options;
        private void NotifyStateProperties()
        {
            NotifyProperty("CanStart");
            NotifyProperty("CanStop");
            NotifyProperty("CanPreview");
            NotifyProperty("CanReset");
            NotifyProperty("CanSelect");
            NotifyProperty("IsSelecting");
        }
        public void Start(bool overrideCheck = false)
        {
            if (CanStart || overrideCheck)
            {
                OnStart?.Invoke();
                State = SessionState.Running;
                CancelSource = new CancellationTokenSource();
                CancelToken = CancelSource.Token;
                Task SessionTask = new Task(() =>
                {
                    CancelToken.Register(() =>
                    {
                        Protocol.Cancel();
                    });
                    Protocol.Interpret(Selection.StartStream(), Selection.ToString());
                    if (!CancelToken.IsCancellationRequested)
                    {
                        OnRun?.Invoke();
                        Protocol.Run();
                    }
                    if (!CancelToken.IsCancellationRequested)
                        Stop();
                }, CancelSource.Token);
                SessionTask.Start();
            }
            else throw new Exception($"Cannot start in state {State}");
        }
        public void Stop(bool overrideCheck = false)
        {
            if (CanStop || overrideCheck)
            {
                CancelSource.Cancel();
                OnStop?.Invoke();
                State = SessionState.Viewing;
            }
            else throw new Exception($"Cannot stop in state {State}");
        }
        public void Reset(bool overrideCheck = false)
        {
            if (CanReset || overrideCheck)
            {
                Protocol.Reset();
                OnReset?.Invoke();
                if (Selection != null)
                    State = SessionState.Ready;
                else
                    State = SessionState.NotReady;
            }
            else throw new Exception($"Cannot reset in state {State}");
        }
        public void Preview(bool overrideCheck = false)
        {
            if (CanPreview || overrideCheck)
            {
                CancelSource = new CancellationTokenSource();
                CancelToken = CancelSource.Token;
                Task SessionTask = new Task(() =>
                {
                    CancelToken.Register(() =>
                    {
                        Protocol.Cancel();
                    });
                    Protocol.Interpret(Selection.StartStream(), Selection.ToString());
                }, CancelSource.Token);
                SessionTask.Start();
                OnPreview?.Invoke();
                State = SessionState.Viewing;
            }
            else throw new Exception($"Cannot preview in state {State}");
        }
        public void OpenSelection(bool overrideCheck = false)
        {
            if (CanSelect || overrideCheck)
            {
                State = SessionState.Selecting;
            }
            else throw new Exception($"Cannot open selection in state {State}");
        }
        public void MakeSelection(object select)
        {
            if (typeof(Streamer).IsAssignableFrom(select.GetType()))
            {
                Selection = (Streamer)select;
                Log.Error($"Making selection: {Selection}");
                Reset(true);
            }
            else
            {
                Log.Error($"Object {Selection} not of type {typeof(Streamer)}");
                CancelSelection();
            }
        }
        public void CancelSelection()
        {
            Log.Error($"Cancelling selection");
        }
    }
}
