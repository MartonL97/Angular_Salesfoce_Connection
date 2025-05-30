﻿using AngularApp1.Server.Data;
using AngularApp1.Server.Interfaces;
using AngularApp1.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace AngularApp1.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(TokenService tokenService, TokenStore tokenStore, ISalesforceAuthService salesforceAuthService, IConfiguration configuration, ILogger<AuthController> logger)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        logger.LogWarning("Test-> Login request received at {Time}", DateTime.UtcNow);

        var salesforceClaims = new SalesforceJwtClaimOptions(configuration);

        try
        {
            await tokenService.RetrieveAndStoreTokensAsync(salesforceClaims);

            var isValid = await IsSalesforceTokenValid(tokenStore.SalesforceAccessToken);

            if (!isValid)
                return Unauthorized("Invalid Salesforce token");

            if (request.SalesforceUserName != null)
            {
                var credentials = await salesforceAuthService.QueryUserPassword(request.SalesforceUserName);

                if (request.Password != null)
                {
                    if (credentials.Salt != null)
                    {
                        var hashedPassword = HashWithPbkdf2(request.Password, credentials.Salt);

                        if (credentials.PasswordHash != null && !SecureEquals(credentials.PasswordHash, hashedPassword))
                            return BadRequest("Unauthorized");
                    }
                }
                
                if (credentials.Email != null)
                    tokenStore.SalesforceRefreshToken = tokenService.GenerateToken(credentials.Email, credentials.ProfileType, salesforceClaims.JwtKey);

                logger.LogWarning("Test-> Login successful at {Time}", DateTime.UtcNow);

                return Ok(new { Token = tokenStore.SalesforceRefreshToken });
            }

            logger.LogWarning("Test-> Login successful at {Time}", DateTime.UtcNow);
            return BadRequest("Invalid Login Request");
        }
        catch(Exception exception)
        {
            return NotFound($"Failed to login in with error: {exception.Message}");
        }
    }

    private async Task<bool> IsSalesforceTokenValid(string? token)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await httpClient.GetAsync($"{configuration["Salesforce:RequestUrl"]}userinfo");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private string HashWithPbkdf2(string password, string salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 100_000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hash);
    }

    private bool SecureEquals(string a, string b)
    {
        if (a.Length != b.Length) return false;

        var result = 0;
        for (var i = 0; i < a.Length; i++)
            result |= a[i] ^ b[i];

        return result == 0;
    }


    public class LoginRequest
    {
        public string? SalesforceUserName { get; set; }

        public string? Password { get; set; } = null;
    }

}