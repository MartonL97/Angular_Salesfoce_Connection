using AngularApp1.Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Policy = "PlayerOnly")]
    public class SalesforceController(ISalesforceService salesforceService) : ControllerBase
    {
        [HttpGet("player")]
        public async Task<IActionResult> GetPlayer()
        {
            try
            {
                var data = await salesforceService.GetSalesforceDataAsync();
                return Ok(JsonDocument.Parse(data));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
