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

        [HttpPost]
        [Route("SaveRatings")]
        public IActionResult SaveRatings(string argLoggedInUser, int argScheduleID, [FromBody] List<CandidateEvaluation> argCandidateEvaluations)
        {
            try
            {
                _candiadateScoreBL.SaveInterviewRatings(argLoggedInUser, argScheduleID, argCandidateEvaluations);
            }
            catch (Exception)
            {
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
        public IActionResult SaveScores(string argLoggedInUser, int argScheduleID, [FromBody] List<Candidate> argGlobalScores)
        {
            try
            {
                _candiadateScoreBL.SaveGlobalScores(argLoggedInUser, argScheduleID, argGlobalScores);

            }
            catch (Exception)
            {
                throw;
            }

            return Ok();

        }

        [HttpPost]
        [Route("SaveScores")]
        public IActionResult UpdateCompensation(string argLoggedInUser, int argGlobalScoreID, bool argIsHired, double argActualCompensation)
        {
            try
            {
                _candiadateScoreBL.UpdateCompensation(argLoggedInUser,argGlobalScoreID,  argIsHired,  argActualCompensation);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();

        }


    }
}
