using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;


using File = Google.Apis.Drive.v3.Data.File;

namespace ProtocolMaster.Component.Google
{
    public sealed class Auth
    {
        private static readonly Auth instance = new Auth();
        static Auth()
        {
        }
        private Auth()
        {
        }
        public static Auth Instance
        {
            get
            {
                return instance;
            }
        }

        UserCredential credential;
        private FileDataStore userStore;

        public async Task Authenticate(params IService[] services)
        {
            Log.Error("Authenticate(): AUTHENTICATING");

            userStore = new FileDataStore(Log.Instance.AppData + "\\Auth", true);
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(40000);

            Log.Error("Authenticate(): Launching OAuth");

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
               new ClientSecrets
               {
                   ClientId = "911699926501-6ici7to17is1fet4mr6jn864lrmrsi0g.apps.googleusercontent.com",
                   ClientSecret = "LbVHZSa-rtPBMl9odwMBkJi_"
               },
               CombineServiceTokens(services),
               "user", cts.Token, userStore);
            CreateServices(services);
            Log.Error("Authenticate(): User Fully Authenticated");
        }
        private void CreateServices(IService[] services)
        {
            foreach(IService service in services)
            {
                service.CreateService(credential);
            }
        }
        
        private string[] CombineServiceTokens(IService[] services)
        {
            string[] builderPrev;
            string[] builderNext = services[0].ServiceTokens();
            int merged = 1;
            while (merged < services.Length)
            {
                builderPrev = builderNext;
                builderNext = new string[builderPrev.Length + services[merged].ServiceTokens().Length];
                builderPrev.CopyTo(builderNext, 0);
                services[merged].ServiceTokens().CopyTo(builderNext, builderNext.Length);
                merged++;
            }
            
            return builderNext;
        }

        public async Task DeAuthenticate()
        {
            if (!isAuthenticated()) return;
            Log.Error("DeAuthenticate(): DEAUTHENTICATING");
            credential = null;
            await userStore.ClearAsync();
            Log.Error("Authenticate(): User Fully Deauthenticated");
        }

        public bool isAuthenticated()
        {
            return credential != null;
        }
    }
}
