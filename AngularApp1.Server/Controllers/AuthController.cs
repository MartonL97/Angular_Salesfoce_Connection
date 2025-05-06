using System.Net.Http.Headers;
using AngularApp1.Server.Data;
using AngularApp1.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp1.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(TokenService tokenService, TokenStore tokenStore, IConfiguration configuration)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request.JwtKey != configuration["Jwt:Key"] || request.SalesforceUserName != configuration["Salesforce:UserName"])
            return BadRequest("Unauthorized");

        // Validate user credentials (this is a simplified version)
        var isValid = await IsSalesforceTokenValid(tokenStore.SalesforceAccessToken);

        if (isValid)
        {
            // Generate token if authentication is successful
            tokenStore.SalesforceRefreshToken = tokenService.GenerateToken(tokenStore.SalesforceJWTToken);
            return Ok(new { Token = tokenStore.SalesforceRefreshToken });
        }
        return Unauthorized("Invalid Salesforce token");
    }

    private async Task<bool> IsSalesforceTokenValid(string token)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            // A lightweight request that requires auth (e.g., identity endpoint)
            var response = await httpClient.GetAsync("https://login.salesforce.com/services/oauth2/userinfo");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public class LoginRequest
    {
        public string SalesforceUserName { get; set; }

        public string? JwtKey { get; set; } = null;
    }

}