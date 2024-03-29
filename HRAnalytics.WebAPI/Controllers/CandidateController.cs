﻿using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;

namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        private readonly TelemetryClient _telemetryClient;

        private ICandidateBL _candiadateBL;

        public CandidateController(TelemetryClient telemetryClient, ICandidateBL candidateBL)
        {
            _telemetryClient = telemetryClient;
            _candiadateBL = candidateBL;
        }

        [HttpPost]
        [Route("SaveInterviewSchecule")]
        public IActionResult SaveInterviewSchecule(string LoggedInUser, [FromBody] Candidate Candidate)
        {
            try
            {
                _candiadateBL.SaveCandidateInterviewSchedule(LoggedInUser, Candidate);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught while CandidateController model...");
                _telemetryClient.TrackException(ex);
                throw;
            }

            return Ok();
        }

        [HttpGet]
        [Route("GetInterviewers")]
        public IActionResult GetInterviewers(string argUserType)
        {
            #region Declarations
            UserCollection l_userCollection;
            #endregion
            try
            {
                l_userCollection = _candiadateBL.GetAllInterviewers(argUserType);

                if (l_userCollection == null || l_userCollection.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught while CandidateController model...");
                _telemetryClient.TrackException(ex);
                throw;
            }

            return Ok(l_userCollection);
        }

        [HttpGet]
        [Route("GetCandidateDetails")]
        public IActionResult GetCandidateDetails(int argCandidateID, int argScheduleID)
        {
            #region Declarations
            Candidate l_Candidate;
            #endregion
            try
            {
                l_Candidate = _candiadateBL.GetCandidateInformation(argCandidateID, argScheduleID);

                if (l_Candidate == null || l_Candidate.CandidateID == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught while CandidateController model...");
                _telemetryClient.TrackException(ex);
                throw;
            }

            return Ok(l_Candidate);
        }
        [HttpGet]
        [Route("GetCandidateForInterviewer")]
        public IActionResult GetCandidateForInterviewer(string argInterviewerID)
        {
            #region Declarations
            CandidateCollection l_CandidateCollection;
            #endregion
            try
            {
                l_CandidateCollection = _candiadateBL.GetCandidateForInterviewer(argInterviewerID);

                if (l_CandidateCollection == null || l_CandidateCollection.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught while CandidateController model...");
                _telemetryClient.TrackException(ex);
                throw;
            }

            return Ok(l_CandidateCollection);
        }

        [HttpGet]
        [Route("GetGlobalScores")]
        public IActionResult GetGlobalScores()
        {
            #region Declarations
            CandidateCollection l_GlobalScore;
            #endregion
            try
            {
                l_GlobalScore = _candiadateBL.GetGlobalScores();

                if (l_GlobalScore == null || l_GlobalScore.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught while CandidateController model...");
                _telemetryClient.TrackException(ex);
                throw;
            }

            return Ok(l_GlobalScore);
        }
    }
}
