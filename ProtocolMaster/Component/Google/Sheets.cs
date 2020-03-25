using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Auth.OAuth2;


namespace ProtocolMaster.Component.Google
{
    public sealed class Sheets : IService
    {
        // IService Implementation
        private static string[] serviceTokens =
        {
            
        };
        string[] IService.ServiceTokens() { return serviceTokens; }
        void IService.CreateService(UserCredential credential)
        {
            // Create the drive service.
            throw new NotImplementedException();
        }
    }
}
