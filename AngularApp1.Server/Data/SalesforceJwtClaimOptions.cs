namespace AngularApp1.Server.Data
{
    public class SalesforceJwtClaimOptions(IConfiguration configuration)
    {
        public string? JwtKey { get; set; } = configuration["JwtKey"];

        // Salesforce specific claims
        public string? SalesForceUserName { get; set; } = configuration["SalesforceUserName"];

        // Login related
        public string? CertificateIdPrefix { get; set; } = configuration["CertificateIdPrefix"];
        public string? SecretKey { get; set; } = configuration["SecretKey"];
    }
}
