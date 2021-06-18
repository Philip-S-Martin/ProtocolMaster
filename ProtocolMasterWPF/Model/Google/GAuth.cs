using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using ProtocolMasterCore.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ProtocolMasterWPF.Model.Google
{
    public enum GAuthState
    {
        PreAuth,
        DoAuth,
        PostAuth
    }
    public sealed class GAuth : INotifyPropertyChanged
    {
        private static readonly GAuth instance;
        private static readonly string authDir;
        public event EventHandler PostAuthentication, PreAuthentication;
        private UserCredential credential;
        private UserCredential Credential { get => credential; set => credential = value; }
        private FileDataStore userStore;
        private GAuthState _state;
        private GAuthState State { get => _state; set { _state = value; OnPropertyChanged("IsPreAuth"); OnPropertyChanged("IsDoAuth"); OnPropertyChanged("IsPostAuth"); } }
        public bool IsPreAuth { get => State == GAuthState.PreAuth; }
        public bool IsDoAuth { get => State == GAuthState.DoAuth; }
        public bool IsPostAuth { get => State == GAuthState.PostAuth; }

        static GAuth()
        {
            if (!AppEnvironment.TryAddLocationAppData("Auth", "Auth", out authDir))
            { throw new System.Exception("Could not create authenticaton directory"); }
            Log.Error($"AuthDir: {authDir}");
            instance = new GAuth();
        }
        private GAuth()
        {
            State = GAuthState.PreAuth;
            // Make sure there are no remaining credentials.
            DirectoryInfo directory = new DirectoryInfo(authDir);
            foreach (FileInfo file in directory.GetFiles())
                file.Delete();
        }
        public static GAuth Instance
        {
            get
            {
                return instance;
            }
        }
        public async Task Authenticate(params IService[] services)
        {
            if (PreAuthentication != null)
                PreAuthentication.Invoke(this, new EventArgs());
            State = GAuthState.DoAuth;
            userStore = new FileDataStore(authDir, true);

            Log.Error("Authenticate(): AUTHENTICATING");
            ICodeReceiver receiver = new GAuthReceiver();
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(40000);

            Log.Error("Authenticate():  Launching OAuth");

            try
            {
                Credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                     new ClientSecrets
                     {
                         ClientId = "911699926501-6ici7to17is1fet4mr6jn864lrmrsi0g.apps.googleusercontent.com",
                         ClientSecret = "LbVHZSa-rtPBMl9odwMBkJi_"
                     },
                     CombineServiceTokens(services),
                     "user", cts.Token, userStore, receiver);
                CreateServices(services);
            }
           catch (Exception e)
            {
                State = GAuthState.PreAuth;
                Log.Error($"Authentication Exception: {e.Message}");
                return;
            }
            if (cts.Token.IsCancellationRequested)
            {
                State = GAuthState.PreAuth;
                Log.Error("Authentication Failed");
            }
            else
            {
                State = GAuthState.PostAuth;
                Log.Error("Authenticate(): User Fully Authenticated");
                if (PostAuthentication != null)
                    PostAuthentication.Invoke(this, new EventArgs());
            }
        }
        private void CreateServices(IService[] services)
        {
            foreach (IService service in services)
            {
                service.CreateService(Credential);
            }
        }

        private string[] CombineServiceTokens(IService[] services)
        {
            List<string> builder = new List<string>();
            foreach (IService service in services)
            {
                foreach (string token in service.ServiceTokens())
                {
                    builder.Add(token);
                }
            }
            return builder.ToArray();
        }

        public async Task DeAuthenticate()
        {
            if (!IsPostAuth) return;
            Log.Error("DeAuthenticate(): DEAUTHENTICATING");
            Credential = null;
            await userStore.ClearAsync();
            State = GAuthState.PreAuth;
            Log.Error("Authenticate(): User Fully Deauthenticated");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
