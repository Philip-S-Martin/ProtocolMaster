using System;
using System.Collections.Generic;
using System.Text;
using Google.Apis.Auth.OAuth2;


namespace ProtocolMaster.Component.Google
{
    public interface IService
    {
        internal string[] ServiceTokens();
        internal void CreateService(UserCredential credential);
    }
}
