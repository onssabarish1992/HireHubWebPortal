﻿using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidateController : ControllerBase
    {
        ICandidateBL _candiadateBL;

        public CandidateController(ICandidateBL candidateBL)
        {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
                throw;
            }

            return Ok(l_userCollection);
        }
    }
}
