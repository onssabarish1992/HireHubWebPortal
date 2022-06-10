using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;

namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TelemetryClient _telemetryClient;

        

        public TestController(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
            
        }

        [HttpGet]
        [Route("HealthCheck")]
        public IActionResult CheckHealth()
        {
            try
            {
                return Ok("Hii...System is up...");
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught while Testing model...");
                _telemetryClient.TrackException(ex);
            }

            return Ok("Hii...System is up after catch block...");

        }
    }
}
