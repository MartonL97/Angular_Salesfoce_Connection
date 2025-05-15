using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IConfiguration configuration) : ControllerBase
    {
        [HttpGet(Name = "GetLogInSettings")]
        public IActionResult Get()
        {
            var _salesforceCertificatePath = configuration["My:Hierarchical:Config:Data"];

            var configDictionary = new Dictionary<string, string>();
            foreach (var kvp in configuration.AsEnumerable())
            {
                configDictionary[kvp.Key] = kvp.Value ?? "";
            }

            var response = new
            {
                SalesforceCertificatePath = _salesforceCertificatePath,
                Configuration = configDictionary
            };

            return Ok(response); // Returns structured JSON
        }
    }

}
