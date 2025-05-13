using AngularApp1.Server.Data;
using AngularApp1.Server.Entities;
using AngularApp1.Server.Interfaces;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Buffers.Text;

namespace AngularApp1.Server.Services
{
    public class SalesforceAuthorizationService(TokenStore tokenStore, IConfiguration configuration) : ISalesforceAuthService
    {
        public async Task<PlayerCredentials> QueryUserPassword(string logInEmail)
        {
            if (string.IsNullOrEmpty(tokenStore.SalesforceAccessToken))
                throw new Exception("Access token retrieval failed.");

            var response = await QueryUserPassword(tokenStore.SalesforceAccessToken, logInEmail);
            var credentials= GetHashPassword(response);

            return credentials;
        }

        private async Task<string> QueryUserPassword(string accessToken, string logInEmail)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var soql = $"SELECT Email__c, PasswordHash__c, Salt__c FROM Player__c WHERE Email__c = '{logInEmail}' LIMIT 1";
            var queryUrl = $"{tokenStore.SalesforceInstanceUrl}/services/data/v60.0/query?q={Uri.EscapeDataString(soql)}";

            var response = await client.GetAsync(queryUrl);
            return await response.Content.ReadAsStringAsync();
        }

        private PlayerCredentials GetHashPassword(string salesforceCredentialsJson)
        {
            var jsonDocument = JsonDocument.Parse(salesforceCredentialsJson);
            var record = jsonDocument.RootElement.GetProperty("records")[0];


            var credentials = new PlayerCredentials
            {
                Email = record.GetProperty("Email__c").GetString(),
                PasswordHash = record.GetProperty("PasswordHash__c").GetString(),
                Salt = record.GetProperty("Salt__c").GetString()
            };

            return credentials;
        }
    }
}
