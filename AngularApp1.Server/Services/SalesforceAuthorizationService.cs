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
        private readonly string? _salesforceClientSecret = configuration["Login:ServerPfx"];
        private readonly string? _salesforceCertificatePw = configuration["Login:Password"];

        public async Task<string> QueryUserPassword(string logInEmail)
        {
            if (string.IsNullOrEmpty(tokenStore.SalesforceAccessToken))
                throw new Exception("Access token retrieval failed.");

            var response = await QueryUserPassword(tokenStore.SalesforceAccessToken, logInEmail);
            var pw= DecryptPassword(GetPassword(response));

            return pw;
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

        private string GetPassword(string salesforceCredentialsJson)
        {
            var jsonDocument = JsonDocument.Parse(salesforceCredentialsJson);
            var record = jsonDocument.RootElement.GetProperty("records")[0];


            var credentials = new PlayerCredentials
            {
                Email = record.GetProperty("Email__c").GetString(),
                Password = record.GetProperty("Password__c").GetString()
            };
             
            return credentials.Password ?? string.Empty;
        }

        private string DecryptPassword(string encryptedPassword)
        {
            try
            {
                // Convert the encrypted password (string) to a byte array

                byte[] encryptedPasswordBytes = Base64UrlDecode(encryptedPassword);

                if (_salesforceClientSecret != null)
                {
                    var certBytes = Convert.FromBase64String(_salesforceClientSecret);

                    var certificate = new X509Certificate2(certBytes, _salesforceCertificatePw, X509KeyStorageFlags.Exportable);

                    using var rsa = certificate.GetRSAPrivateKey();

                    var decryptedBytes = rsa.Decrypt(encryptedPasswordBytes, RSAEncryptionPadding.OaepSHA1);

                    var decryptedPassword = Encoding.UTF8.GetString(decryptedBytes);

                    return decryptedPassword;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return string.Empty;
        }

        private byte[] Base64UrlDecode(string base64Url)
        {
            // Replace URL-safe characters with their Base64 counterparts
            base64Url = base64Url.Replace('-', '+').Replace('_', '/');

            // Add padding if necessary
            int padding = 4 - (base64Url.Length % 4);
            if (padding < 4)
            {
                base64Url = base64Url.PadRight(base64Url.Length + padding, '=');
            }

            // Convert the Base64Url string back to a byte array
            return Convert.FromBase64String(base64Url);
        }

    }
}
