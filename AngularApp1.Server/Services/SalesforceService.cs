using AngularApp1.Server.Data;
using AngularApp1.Server.Interfaces;
using System.Net.Http.Headers;

namespace AngularApp1.Server.Services
{
    public class SalesforceService(TokenStore tokenStore) : ISalesforceService
    {
        public async  Task<string> GetSalesforceDataAsync()
        {
            if (string.IsNullOrEmpty(tokenStore.SalesforceAccessToken))
                throw new Exception("Access token retrieval failed.");

            var response = await QuerySalesforceAsync(tokenStore.SalesforceAccessToken);
            return response;
        }

        private async Task<string> QuerySalesforceAsync(string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var soql = "SELECT FIELDS(All) FROM Player__c ORDER BY Name LIMIT 1";
            var queryUrl = $"{tokenStore.SalesforceInstanceUrl}/services/data/v60.0/query?q={Uri.EscapeDataString(soql)}";

            var response = await client.GetAsync(queryUrl);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
