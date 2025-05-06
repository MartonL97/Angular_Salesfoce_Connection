using AngularApp1.Server.Data;
using AngularApp1.Server.Interfaces;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace AngularApp1.Server.Services
{
    public class AuthService(TokenStore tokenStore) : ISalesforceAuthService
    {
        public async Task<string> QueryUserPassword(string logInEmail)
        {
            if (string.IsNullOrEmpty(tokenStore.SalesforceAccessToken))
                throw new Exception("Access token retrieval failed.");

            var response = await QueryUserPassword(tokenStore.SalesforceAccessToken, logInEmail);
            return response;
        }

        private async Task<string> QueryUserPassword(string accessToken, string logInEmail)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var soql = $"SELECT Email__c, Password__c FROM Player__c WHERE Email__c = '{logInEmail}' LIMIT 1";
            var queryUrl = $"{tokenStore.SalesforceInstanceUrl}/services/data/v60.0/query?q={Uri.EscapeDataString(soql)}";

            var response = await client.GetAsync(queryUrl);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
