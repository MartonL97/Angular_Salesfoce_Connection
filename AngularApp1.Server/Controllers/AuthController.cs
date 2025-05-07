using System.Net.Http.Headers;
using AngularApp1.Server.Data;
using AngularApp1.Server.Interfaces;
using AngularApp1.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp1.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(TokenService tokenService, TokenStore tokenStore, ISalesforceAuthService salesforceAuthService, IConfiguration configuration)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request.SalesforceUserName != null)
        {
            var requestUserPassword = await salesforceAuthService.QueryUserPassword(request.SalesforceUserName);

            if (requestUserPassword != request.Password)
                return BadRequest("Unauthorized");
        }
        else
            return BadRequest("Invalid Login Request");
        
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
            var response = await httpClient.GetAsync($"{configuration["Salesforce:RequestUrl"]}userinfo");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public class LoginRequest
    {
        public string? SalesforceUserName { get; set; }

        public string? Password { get; set; } = null;
    }

}