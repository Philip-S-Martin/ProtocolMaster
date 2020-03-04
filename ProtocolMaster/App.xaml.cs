using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using System.Collections.Generic;

namespace ProtocolMasterGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        UserCredential credential;
        private DriveService service;
        private FileDataStore store;

        public bool LoggedIn
        {
            get
            {
                return credential != null;
            }
        }


        public async Task Login()
        {
            store = new FileDataStore("ProtocolMaster/Auth");
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(40000);

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
               new ClientSecrets
               {
                   ClientId = "749421427281-78b926aib5kcphq6qu09jpvgj265quoc.apps.googleusercontent.com",
                   ClientSecret = "A7UFlilq-VQTLgyKyfYKVcCY"
               },
               new[] { DriveService.Scope.DriveFile },
               "user", cts.Token, store);

            // Create the drive service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Drive API Sample",
            });
        }

        public async Task Logout()
        {
            credential = null;
            service = null;
            await store.ClearAsync();

        }

        public string[] test()
        {
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 20;
            listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.Q =
                "mimeType = 'application/vnd.google-apps.folder' and " +
                "'1GjWe_A_uB53MwC-xVp5KFpMXdPS38iJ7' in parents";


            FileList files = listRequest.Execute();

            List<String> res = new List<string>();
            Console.WriteLine("Files:");

            if (files.Files.Count > 0)
            {
                foreach (var file in files.Files)
                {
                    res.Add(file.Name);
                }
            }
            else
            {
                res.Add("No files found.");
            }
            return res.ToArray();
        }
        public void Window_Closed()
        {
            //store.ClearAsync();
        }
    }
}
