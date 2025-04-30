using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet(Name = "GetLogInSettings")]
        public IActionResult Get()
        {
            var response = new { message = "hello" }; // return an object with a property
            return Ok(response);  // This returns an object as JSON
        }
    }

}
