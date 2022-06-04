using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateScoreController : ControllerBase
    {
        ICandidateScore _candiadateScoreBL;

        public CandidateScoreController(ICandidateScore candidateScore)
        {
            _candiadateScoreBL = candidateScore;
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
            catch (Exception)
            {
                throw;
            }

            return Ok(l_candidateScore);
        }
    }
}
