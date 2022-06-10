﻿using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateResultController : ControllerBase
    {
        private ICandidateResult _candidateResult;
        private readonly TelemetryClient _telemetryClient;

        public CandidateResultController(ICandidateResult argCandidate, TelemetryClient argTelemetryClient)
        {
            _candidateResult = argCandidate;
            _telemetryClient = argTelemetryClient;
        }

        [HttpGet]
        [Route("GetCandidateResult")]
        public IActionResult GetCandidateResult()
        {
            #region Declarations
            List<Candidate> l_CandidateCollection;
            #endregion
            try
            {
                l_CandidateCollection = _candidateResult.GetCandidateResult();

                if (l_CandidateCollection == null || l_CandidateCollection.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught within GetCandidateResult method...");
                _telemetryClient.TrackException(ex);
                throw;
            }

            return Ok(l_CandidateCollection);
        }
    }
}
