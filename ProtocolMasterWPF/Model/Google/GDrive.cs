using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using ProtocolMasterCore.Utility;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using File = Google.Apis.Drive.v3.Data.File;

namespace ProtocolMasterWPF.Model.Google
{
    public sealed class GDrive : IService
    {
        private static readonly GDrive instance = new GDrive();
        static GDrive()
        {
        }
        private GDrive()
        {
            AvailableFiles = new ObservableCollection<object>();
        }
        public static GDrive Instance
        {
            get
            {
                return instance;
            }
        }
        public Stream StreamFile(string fileID)
        {
            if (fileID != null)
                return service.Files.Export(fileID, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet").ExecuteAsStream();
            else return null;
        }
        public void Publish(string fileID)
        {
            service.Revisions.Update(new Revision()
            {
                Published = true,
                PublishAuto = true,
            },
            fileID,
            "1").Execute();
        }

        // Core Drive Functionality. 
        // This all needs to be abstracted somewhere else (Model)
        const string ROOT_FOLDER_NAME = "ProtocolMaster";
        public ObservableCollection<object> AvailableFiles { get; private set; }
        FileList driveFiles;
        private DriveService service;
        #region
        private void LoadAvailable(bool clear = false)
        {
            AvailableFiles.Clear();
            FileList result;
            do
            {
                FilesResource.ListRequest listRequest = service.Files.List();
                listRequest.PageSize = 20;
                listRequest.Fields = "nextPageToken, files(id, name, mimeType, trashed)";
                listRequest.Q =
                    "mimeType = 'application/vnd.google-apps.spreadsheet' and " +
                    "trashed = false";
                result = listRequest.Execute();

                foreach (File file in result.Files)
                    AvailableFiles.Add(new GFileStreamer(file));
            } while (result.IncompleteSearch.HasValue && result.IncompleteSearch.Equals(true));
        }
        public void RefreshAvailable()
        {
            LoadAvailable(true);
        }
        #endregion

        // IService Implementation
        #region
        private static readonly string[] serviceTokens =
        {
            DriveService.Scope.DriveFile,
            "https://www.googleapis.com/auth/drive.install"
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
            LoadAvailable();
        }
        #endregion
    }
}
