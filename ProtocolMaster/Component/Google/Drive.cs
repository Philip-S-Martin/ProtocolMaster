using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using ProtocolMaster.Component.Debug;
using System.Collections.Generic;

namespace ProtocolMaster.Component.Google
{
    public sealed class Drive : IService
    {
        private static readonly Drive instance = new Drive();
        static Drive()
        {
        }
        private Drive()
        {
        }
        public static Drive Instance
        {
            get
            {
                return instance;
            }
        }


        // Core Drive Functionality. 
        // This all needs to be abstracted somewhere else (Model)
        const string ROOT_FOLDER_NAME = "ProtocolMaster";
        private TreeFile root;
        private DriveService service;
        #region
        private void FindRoot()
        {
            Log.Error("FindRoot(): FINDING ROOT FOLDER");
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 20;
            listRequest.Fields = "nextPageToken, files(id, name, mimeType)";
            listRequest.Q =
                "mimeType = 'application/vnd.google-apps.folder' and " +
                "name = '" + ROOT_FOLDER_NAME + "' and" +
                "trashed = false";

            FileList files = listRequest.Execute();

            Log.Error("FindRoot(): files.Files.Count = " + files.Files.Count);
            if (files.Files.Count == 0)
            {
                // This is an error. Could not find folder
                CreateRoot();
            }
            else if (files.Files.Count > 1)
            {
                // Also an error. Too many ProtocolMaster folders!
                root = new TreeFile(files.Files[0]);
            }
            else
            {
                root = new TreeFile(files.Files[0]);
            }
        }
        private void CreateRoot()
        {
            root = null;
            root = CreateFolder(ROOT_FOLDER_NAME);
        }
        public TreeFile CreateFolder(string name, TreeFile parent = null)
        {
            // Prepare the file
            File newFile = new File
            {
                Name = name,
                MimeType = "application/vnd.google-apps.folder"
            };
            if (parent == null)
            {
                parent = root;
            }
            if (parent != null)
            {
                newFile.Parents = new List<string>() { parent.File.Id };
            }
            // Create the file on Google Server
            FilesResource.CreateRequest create = service.Files.Create(newFile);
            create.Fields = "id, parents, mimeType, name";
            return parent.AddChild(create.Execute());
        }
        public List<File> GetChildren(File parent)
        {
            List<File> list = new List<File>();
            FileList result;
            do
            {
                FilesResource.ListRequest listRequest = service.Files.List();
                listRequest.PageSize = 20;
                listRequest.Fields = "nextPageToken, files(id, name, mimeType, parents)";
                listRequest.Q =
                    "mimeType = 'application/vnd.google-apps.spreadsheet' and " +
                    "trashed = false";
                result = listRequest.Execute();
                list.AddRange(result.Files);
            } while (result.IncompleteSearch.HasValue && result.IncompleteSearch.Equals(true));
            return list;
        }
        public void Refresh(MarkupCallback callback = null)
        {
            root = null;
            FindRoot();
            if (callback != null)
                CallbackPreBF(callback);
        }

        public void CallbackPreBF(MarkupCallback callback)
        {
            if (Auth.Instance.IsAuthenticated())
                root.CallbackPreBF(callback);
        }
        #endregion

        // IService Implementation
        #region
        private static readonly string[] serviceTokens =
        {
            DriveService.Scope.DriveFile
        };


        string[] IService.ServiceTokens() { return serviceTokens; }
        void IService.CreateService(UserCredential credential)
        {
            Log.Error("CreateService(): CREATING DRIVE SERVICE");
            // Create the drive service.
            service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Protocol Master",
            });

            // Find root folder for application.
            FindRoot();
        }
        #endregion
    }
}
