using AngularApp1.Server.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class SalesforceController : ControllerBase
    {
        private readonly ISalesforceService _salesforceService;

        public SalesforceController(ISalesforceService salesforceService)
        {
            _salesforceService = salesforceService;
        }

        [HttpGet("store")]
        public async Task<IActionResult> GetStore()
        {
            try
            {
                var data = await _salesforceService.GetSalesforceDataAsync();
                return Ok(JsonDocument.Parse(data));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
