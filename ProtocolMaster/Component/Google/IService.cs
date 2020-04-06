using Google.Apis.Auth.OAuth2;

namespace ProtocolMaster.Component.Google
{
    public interface IService
    {
        string[] ServiceTokens();
        void CreateService(UserCredential credential);
    }
}
