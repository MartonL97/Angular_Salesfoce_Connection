namespace AngularApp1.Server.Data
{
    public class TokenStore
    {
        public string? SalesforceJwtToken { get; set; }

        public string? SalesforceAccessToken { get; set; }

        public string? SalesforceRefreshToken { get; set; }

        public string? SalesforceInstanceUrl { get; set; }
    }
}
