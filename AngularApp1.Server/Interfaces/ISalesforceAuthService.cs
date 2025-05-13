using AngularApp1.Server.Entities;

namespace AngularApp1.Server.Interfaces
{
    public interface ISalesforceAuthService
    {
        Task<PlayerCredentials> QueryUserPassword(string logInEmail);
    }
}
