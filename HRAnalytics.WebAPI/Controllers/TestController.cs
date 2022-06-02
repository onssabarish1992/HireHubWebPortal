using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("HealthCheck")]
        public IActionResult CheckHealth()
        {
            return Ok("Hii...System is up...");
        }
    }
}
