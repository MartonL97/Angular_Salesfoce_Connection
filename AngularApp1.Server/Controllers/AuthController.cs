using Microsoft.AspNetCore.Mvc;
using AngularApp1.Server.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        // Generate token if authentication is successful
        var token = _tokenService.GenerateToken(loginRequest.Username);
        return Ok(new { Token = token });

    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}