using AngularApp1.Server.Data;
using AngularApp1.Server.Entities;
using AngularApp1.Server.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AngularApp1.Server.Services
{
    public class SalesforceAuthorizationService(TokenStore tokenStore, IConfiguration configuration) : ISalesforceAuthService
    {
        public async Task<ProfileCredentials> QueryUserPassword(string logInEmail)
        {
            if (string.IsNullOrEmpty(tokenStore.SalesforceAccessToken))
                throw new Exception("Access token retrieval failed.");

            var response = await QueryUserPassword(tokenStore.SalesforceAccessToken, logInEmail);
            var credentials = GetProfileCredentials(response);

            return credentials;
        }

        private async Task<string> QueryUserPassword(string accessToken, string logInEmail)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var soql = $"SELECT Email__c, PasswordHash__c, Salt__c, ProfileType__c FROM Profiles__c WHERE Email__c = '{logInEmail}' LIMIT 1";
            var queryUrl = $"{tokenStore.SalesforceInstanceUrl}/services/data/v60.0/query?q={Uri.EscapeDataString(soql)}";

            var response = await client.GetAsync(queryUrl);
            return await response.Content.ReadAsStringAsync();
        }

        private ProfileCredentials GetProfileCredentials(string salesforceCredentialsJson)
        {
            var jsonDocument = JsonDocument.Parse(salesforceCredentialsJson);
            var record = jsonDocument.RootElement.GetProperty("records")[0];


            var credentials = new ProfileCredentials
            {
                Email = record.GetProperty("Email__c").GetString(),
                PasswordHash = record.GetProperty("PasswordHash__c").GetString(),
                Salt = record.GetProperty("Salt__c").GetString(),
                ProfileType = record.GetProperty("ProfileType__c").GetString() switch
                {
                    "Player" => ProfileType.Player,
                    "Coach" => ProfileType.Coach,
                    _ => throw new ArgumentOutOfRangeException(nameof(ProfileCredentials.ProfileType))
                }
            };

            return credentials;
        }
    }
}
