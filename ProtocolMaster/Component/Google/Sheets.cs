using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using ProtocolMaster.Component.Debug;

namespace ProtocolMaster.Component.Google
{
    public sealed class Sheets : IService
    {
        private static readonly Sheets instance = new Sheets();
        static Sheets()
        {
        }
        private Sheets()
        {
        }
        public static Sheets Instance
        {
            get
            {
                return instance;
            }
        }
        // IService Implementation
        private static string[] serviceTokens =
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
