using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

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
        const string ROOT_FOLDER_NAME = "ProtocolMaster";
        private TreeFile root;
        private DriveService service;

        // Core Drive Functionality
        #region
        private void FindRoot()
        {
            Log.Error("FindRoot(): FINDIND ROOT FOLDER");
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 20;
            listRequest.Fields = "nextPageToken, files(id, name)";
            listRequest.Q =
                "mimeType = 'application/vnd.google-apps.folder' and " +
                "name = '" + ROOT_FOLDER_NAME + "'";

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
            root = new TreeFile(CreateFolder(ROOT_FOLDER_NAME));
        }
        public File CreateFolder(string name, TreeFile parent = null)
        {
            // Prepare the file
            File newRoot = new File();
            newRoot.Name = name;
            newRoot.MimeType = "application/vnd.google-apps.folder";
            if (parent != null)
            {
                newRoot.Parents = new List<string>() { parent.Id };
            }
            // Create the file on Google Server
            FilesResource.CreateRequest create = service.Files.Create(newRoot);
            create.Fields = "id";
            return new TreeFile(create.Execute(), parent);
        }
        public List<File> GetChildren(File parent)
        {
            List<File> list = new List<File>();
            FileList result;
            do
            {
                FilesResource.ListRequest listRequest = service.Files.List();
                listRequest.PageSize = 20;
                listRequest.Fields = "nextPageToken, files(id, name)";
                listRequest.Q =
                    "mimeType = 'application/vnd.google-apps.folder' and " +
                    "'" + parent.Id + "' in parents";
                result = listRequest.Execute();
                list.AddRange(result.Files);
            } while (result.IncompleteSearch.HasValue && result.IncompleteSearch.Equals(true));
            return list;
        }

        public void CallbackPreBF(MarkupCallback callback)
        {
            root.CallbackPreBF(callback);
        }
        #endregion

        // IService Implementation
        #region
        private static string[] serviceTokens =
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
