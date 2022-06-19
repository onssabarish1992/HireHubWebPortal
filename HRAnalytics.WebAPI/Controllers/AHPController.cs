using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
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
        public IActionResult SavePairs(string argLoggedInUser, int argEntityID)
        {
            try
            {
                _ahpBL.SavePairs(argEntityID, argLoggedInUser);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }

        [HttpGet]
        [Route("GetAHPPairs")]
        public IActionResult GetAHPPairs(int argEntityID, int? argParentEntityID)
        {
            #region Declarations
            List<AHPPair> l_pairCollection;
            #endregion
            try
            {
                l_pairCollection = _ahpBL.GetAHPPairs(argEntityID, argParentEntityID);

                if (l_pairCollection == null || l_pairCollection.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught in GetAHPPairs model...");
                _telemetryClient.TrackException(ex);
                throw;
            }

            return Ok(l_pairCollection);
        }

        [HttpPost]
        [Route("SaveAHPWeightage")]
        public IActionResult SaveAHPWeightage(string argLoggedInUserID, [FromBody]List<AHPPair> argAHPPairs)
        {
            try
            {
                _ahpBL.SavAHPWeightage(argLoggedInUserID, argAHPPairs);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught in SaveAHPWeightage method...");
                _telemetryClient.TrackException(ex);
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpPost]
        [Route("SaveAHPFinalScores")]
        public IActionResult SaveAHPFinalScores(string argLoggedInUserID, int argEntityID, int? argJobId)
        {
            try
            {
                _ahpBL.SavAHPFinalScores(argLoggedInUserID, argEntityID, argJobId);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught in SaveAHPFinalScores method...");
                _telemetryClient.TrackException(ex);
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
