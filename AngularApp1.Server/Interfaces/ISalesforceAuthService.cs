namespace AngularApp1.Server.Interfaces
{
    public interface ISalesforceAuthService
    {
        Task<string> QueryUserPassword(string logInEmail);
    }
}
