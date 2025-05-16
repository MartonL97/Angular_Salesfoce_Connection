using Microsoft.AspNetCore.Mvc;

namespace AngularApp1.Server.Controllers
{
    //Just for debugging in Azure
    [ApiController]
    [Route("[controller]")]
    public class UserController(IConfiguration configuration) : ControllerBase
    {
        public IActionResult Get()
        {
            var configDictionary = new Dictionary<string, string>();
            foreach (var kvp in configuration.AsEnumerable())
            {
                configDictionary[kvp.Key] = kvp.Value ?? "";
            }

            var response = new
            {
                Configuration = configDictionary
            };

            return Ok(response);
        }
    }

}
