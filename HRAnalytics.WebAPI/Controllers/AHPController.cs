using HRAnalytics.BL.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AHPController : ControllerBase
    {
        private readonly TelemetryClient _telemetryClient;

        IAHP _ahpBL;

        public AHPController(TelemetryClient telemetryClient,
                                        IAHP ahpBL)

        {
            _telemetryClient = telemetryClient;
            _ahpBL = ahpBL;
        }

        [HttpPost]
        [Route("SavePairs")]
        public IActionResult SavePairs(string argLoggedInUser, int argEntityID, int? argParentEntityID)
        {
            try
            {
                _ahpBL.SavePairs(argEntityID, argParentEntityID, argLoggedInUser);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();

        }
    }
}
