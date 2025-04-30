using AngularApp1.Server.Interfaces;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace AngularApp1.Server.Services
{
    public class SalesforceService(IConfiguration configuration) : ISalesforceService
    {
        private readonly IConfiguration _configuration = configuration;

        public async  Task<string> GetSalesforceDataAsync()
        {
            var jwt = GetJwtToken();
            var accessToken = await GetAccessTokenAsync(jwt);
            if (string.IsNullOrEmpty(accessToken))
                throw new Exception("Access token retrieval failed.");

            var response = await QuerySalesforceAsync(accessToken);
            return response;
        }

        private async Task<string> QuerySalesforceAsync(string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string instanceUrl = "https://orgfarm-e6c1f8885b-dev-ed.develop.my.salesforce.com"; // e.g., https://na123.salesforce.com
            string soql = "SELECT FIELDS(All) FROM Store__c ORDER BY Name LIMIT 1";
            string queryUrl = $"{instanceUrl}/services/data/v60.0/query?q={Uri.EscapeDataString(soql)}";

            var response = await client.GetAsync(queryUrl);
            return await response.Content.ReadAsStringAsync();
        }

        private async Task<string> GetAccessTokenAsync(string jwtToken)
        {
            var client = new HttpClient();

            // Prepare request content
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
            { "assertion", jwtToken }
        });

            try
            {
                var response = await client.PostAsync("https://login.salesforce.com/services/oauth2/token", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic tokenResponse = JsonConvert.DeserializeObject(responseString);

                    return tokenResponse.access_token;
                }

                Console.WriteLine("Error: " + responseString);
                return null;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private string Base64UrlEncode(byte[] input)
        {
            string base64 = Convert.ToBase64String(input);
            base64 = base64.Split('=')[0]; // Remove any trailing '=' characters
            base64 = base64.Replace('+', '-'); // Replace '+' with '-'
            base64 = base64.Replace('/', '_'); // Replace '/' with '_'
            return base64;
        }

        private string GetJwtToken()
        {
            string header = "{\"alg\":\"RS256\"}";
            string claimTemplate = "{{\"iss\": \"{0}\", \"sub\": \"{1}\", \"aud\": \"{2}\", \"exp\": \"{3}\", \"jti\": \"{4}\"}}";

            StringBuilder token = new StringBuilder();

            // Encode the JWT Header and add it to our string to sign
            token.Append(Base64UrlEncode(Encoding.UTF8.GetBytes(header)));

            // Separate with a period
            token.Append(".");

            // Create the JWT Claims Object
            string[] claimArray = new string[5];
            claimArray[0] = "3MVG9dAEux2v1sLsnNSYuh_c2dIUqNq8wcOdjOmg8DDRwwgaTzn6FM1tTVReo77uAehnAQN4J6uIZkzl6T5eq";
            claimArray[1] = "songyfalc97579@agentforce.com";
            claimArray[2] = "https://login.salesforce.com";
            claimArray[3] = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 300).ToString();
            claimArray[4] = GetJti(); // Replace with actual JTI

            // Format claims
            string payload = string.Format(claimTemplate, claimArray);

            // Add the encoded claims object
            token.Append(Base64UrlEncode(Encoding.UTF8.GetBytes(payload)));

            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Ensure base path is set
                .AddUserSecrets("6016f5ee-39de-4ff9-ab75-71b38b0e3603") // Manually specify the secrets ID
                .Build();

            var base64 = config["Certificate:ServerPfx"];
            var password = config["Certificate:Password"];

            var certBytes = Convert.FromBase64String(base64);

            var cert = new X509Certificate2(certBytes, password,
                X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

            Console.WriteLine($"✅ Loaded certificate: {cert.Subject}");


            RSA privateKey = cert.GetRSAPrivateKey();

            // Sign the JWT Header + "." + JWT Claims Object
            byte[] dataToSign = Encoding.UTF8.GetBytes(token.ToString());
            byte[] signature = privateKey.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Separate with a period
            token.Append(".");

            // Add the encoded signature
            token.Append(Base64UrlEncode(signature));

            return token.ToString();
        }

        private string GetJti()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
