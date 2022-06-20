using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;

namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateScoreController : ControllerBase

    {
        private readonly TelemetryClient _telemetryClient;

        ICandidateScore _candiadateScoreBL;

        ICandidateOptimization _candidateOptimization;


        public CandidateScoreController(TelemetryClient telemetryClient, 
                                        ICandidateScore candiadateScoreBL,
                                        ICandidateOptimization candidateOptimization)

        {
            _telemetryClient = telemetryClient;
            _candiadateScoreBL = candiadateScoreBL;
            _candidateOptimization = candidateOptimization;
        }

     

        [HttpGet]
        [Route("GetCandidateScore")]
        public IActionResult GetCandidateScore(int argScheduleID)
        {
            #region Declarations
            CandidateScore l_candidateScore;
            #endregion
            try
            {
                l_candidateScore = _candiadateScoreBL.GetCandidateScore(argScheduleID);

                if (l_candidateScore == null || l_candidateScore.Evaluation==null || l_candidateScore.Evaluation.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught while CandidateScoreController model...");
                _telemetryClient.TrackException(ex);

                throw;
            }

            return Ok(l_candidateScore);
        }

        [HttpPost]
        [Route("SaveRatings")]
        public IActionResult SaveRatings(string argLoggedInUser, int argScheduleID, [FromBody] List<CandidateEvaluation> argCandidateEvaluations)
        {
            try
            {
                _candiadateScoreBL.SaveInterviewRatings(argLoggedInUser, argScheduleID, argCandidateEvaluations);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught while CandidateScoreController model...");
                _telemetryClient.TrackException(ex);
                throw;
            }

            return Ok();
        }


        [HttpGet]
        [Route("GetAllCandidateScore")]
        public IActionResult GetAllCandidateScore()
        {
            #region Declarations
            List<CandidateEvaluation> l_scoreEvaluation=new();
            #endregion
            try
            {
                l_scoreEvaluation = _candiadateScoreBL.GetAllCandidateScores();

                if (l_scoreEvaluation == null || l_scoreEvaluation.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(l_scoreEvaluation);
        }

        [HttpPost]
        [Route("SaveScores")]
        public IActionResult SaveScores(string argLoggedInUser)
        {
            try
            {
                List<Candidate> l_GlobalScores = _candidateOptimization.GetOptimumResult();
                _candiadateScoreBL.SaveGlobalScores(argLoggedInUser, l_GlobalScores);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();

        }

        [HttpPost]
        [Route("SaveResult")]
        public IActionResult SaveResult([FromBody]Candidate argCandidate)
        {
            try
            {
                
                _candiadateScoreBL.SaveResult(argCandidate);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();

        }
    }
}
