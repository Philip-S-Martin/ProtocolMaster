using Google.Apis.Auth.OAuth2;

namespace ProtocolMaster.Model.Google
{
    public interface IService
    {
        string[] ServiceTokens();
        void CreateService(UserCredential credential);
    }
}
