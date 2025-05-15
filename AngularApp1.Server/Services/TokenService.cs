using AngularApp1.Server.Data;
using AngularApp1.Server.Entities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AngularApp1.Server.Services;

public class TokenService(IConfiguration configuration, TokenStore tokenStore)
{
    private readonly string? _secretKey = configuration["Jwt:Key"];
    private readonly string? _issuer = configuration["Jwt:Issuer"];
    private readonly string? _audience = configuration["Jwt:Audience"];
    private readonly string? _salesforceClientId = configuration["Salesforce:ClientId"];
    private readonly string? _salesforceUserName = configuration["Salesforce:UserName"];
    private readonly string? _salesforceUrl = configuration["Salesforce:Url"];

    private readonly string? _salesforceClientSecret = configuration["Pfx"];
    private readonly string? _salesforceCertificatePw = configuration["Login:Password"];

    private readonly string? _salesforceCertificatePath = configuration["Pfx"];

    public async Task RetrieveAndStoreTokensAsync()
    {
        // Retrieve the JWT token
        var jwtToken = GetJwtToken();

        // Retrieve the access token using the JWT token
        var accessToken = await GetSalesforceAccessTokenAsync(jwtToken);

        if (string.IsNullOrEmpty(accessToken))
            throw new Exception("Access token retrieval failed.");

        // Store the tokens in TokenStore
        tokenStore.SalesforceJWTToken = jwtToken;
        tokenStore.SalesforceAccessToken = accessToken;
    }

    public string GenerateToken(string username, ProfileType role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role.ToString()) // Convert the enum to string for the role claim
        };

        if (_secretKey != null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        return string.Empty;
    }


    private async Task<string?> GetSalesforceAccessTokenAsync(string jwtToken)
    {
        var client = new HttpClient();

        try
        {
            var content = CreateJwtBearerTokenContent(jwtToken);

            var response = await client.PostAsync($"{configuration["Salesforce:RequestUrl"]}token", content);

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                dynamic tokenResponse = JsonConvert.DeserializeObject(responseString)!;

                tokenStore.SalesforceInstanceUrl = tokenResponse.instance_url;

                return tokenResponse.access_token;
            }

            Console.WriteLine("Error: " + responseString);
            return null;

        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            throw;
        }

    }

    private string GetJwtToken()
    {
        const string header = "{\"alg\":\"RS256\"}";
        const string claimTemplate = "{{\"iss\": \"{0}\", \"sub\": \"{1}\", \"aud\": \"{2}\", \"exp\": \"{3}\", \"jti\": \"{4}\"}}";

        var token = new StringBuilder();

        token.Append(Base64UrlEncode(Encoding.UTF8.GetBytes(header)));
        token.Append('.');

        // Create the JWT Claims Object
        var claimArray = new object?[5];
        claimArray[0] = _salesforceClientId ?? string.Empty;
        claimArray[1] = _salesforceUserName ?? string.Empty;
        claimArray[2] = _salesforceUrl ?? string.Empty;
        claimArray[3] = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 300).ToString();
        claimArray[4] = Guid.NewGuid().ToString();

        var payload = string.Format(claimTemplate, claimArray);
        token.Append(Base64UrlEncode(Encoding.UTF8.GetBytes(payload)));

        var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        store.Open(OpenFlags.ReadOnly);

        if (_salesforceClientSecret != null)
        {
            var certBytes = Convert.FromBase64String(_salesforceClientSecret);

            var cert = new X509Certificate2(certBytes, _salesforceCertificatePw,
                X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
            var privateKey = cert.GetRSAPrivateKey();

            // Sign the JWT Header + "." + JWT Claims Object
            var dataToSign = Encoding.UTF8.GetBytes(token.ToString());
            var signature = privateKey?.SignData(dataToSign, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            token.Append('.');
            if (signature != null) token.Append(Base64UrlEncode(signature));
        }

        return token.ToString();
    }

    private FormUrlEncodedContent CreateJwtBearerTokenContent(string jwtToken)
    {
        return new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
            { "assertion", jwtToken }
        });
    }

    private string Base64UrlEncode(byte[] input)
    {
        var base64 = Convert.ToBase64String(input);
        base64 = base64.Split('=')[0];
        base64 = base64.Replace('+', '-');
        base64 = base64.Replace('/', '_');
        return base64;
    }
}