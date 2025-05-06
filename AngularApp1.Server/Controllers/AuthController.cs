using Microsoft.AspNetCore.Mvc;
using AngularApp1.Server.Services;
using AngularApp1.Server.Data;
using Microsoft.AspNetCore.Identity.Data;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly TokenStore _tokenStore;


    public AuthController(TokenService tokenService, TokenStore tokenStore, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _tokenStore = tokenStore;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        // Validate user credentials (this is a simplified version)
        if (true)
        {
            // Generate token if authentication is successful
            _tokenStore.SalesforceRefreshToken = _tokenService.GenerateToken(_tokenStore.SalesforceJWTToken);
            return Ok(new { Token = _tokenStore.SalesforceRefreshToken });
        }

        return Unauthorized("Invalid credentials");
    }
}

