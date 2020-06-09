using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using ProtocolMaster.Model.Debug;

namespace ProtocolMaster.Model.Google
{
    public sealed class GSheets : IService
    {
        private static readonly GSheets instance = new GSheets();
        static GSheets()
        {
        }
        private GSheets()
        {
        }
        public static GSheets Instance
        {
            get
            {
                return instance;
            }
        }
        // IService Implementation
        private static readonly string[] serviceTokens =
        {
            "https://www.googleapis.com/auth/drive.install"
        };
        string[] IService.ServiceTokens() { return serviceTokens; }
        private SheetsService service;
        void IService.CreateService(UserCredential credential)
        {
            // Create the drive service.
            Log.Error("CreateService(): CREATING SHEETS SERVICE");
            // Create the drive service.
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Protocol Master",
            });
        }
    }
}
