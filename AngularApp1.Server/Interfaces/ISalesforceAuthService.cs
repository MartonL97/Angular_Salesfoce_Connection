using AngularApp1.Server.Entities;

namespace AngularApp1.Server.Interfaces
{
    public interface ISalesforceAuthService
    {
        Task<ProfileCredentials> QueryUserPassword(string logInEmail);
    }
}
