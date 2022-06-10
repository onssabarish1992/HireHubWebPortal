using System;
using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneratePairsController : ControllerBase
    {
        private readonly TelemetryClient _telemetryClient;
        IJobBL _jobBL;

        public GeneratePairsController(TelemetryClient telemetryClient, IJobBL jobBL)
        {
            _telemetryClient = telemetryClient;
            _jobBL = jobBL;
        }

        [HttpGet]
        [Route("GeneratePairs")]
        public IActionResult GeneratePairs(string argType)
        {
            #region Declarations
            JobCollection l_jobCollection;
            List<int> Role = new List<int>();
            #endregion
            try
            {
                    l_jobCollection = _jobBL.GetRoles();
                
                if (l_jobCollection == null || l_jobCollection.Count == 0)
                {
                    return NotFound();
                }

                for (var i = 0; i<l_jobCollection.Count;i++){
                    Role.Add(l_jobCollection[i].JobCriteriaID);
                }
                
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught while GeneratePairController model...");
                _telemetryClient.TrackException(ex);
                throw;
            }

            return Ok(Role);

            
        }
    }
}

