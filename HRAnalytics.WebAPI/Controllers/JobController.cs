using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        IJobBL _jobBL;

        public JobController(IJobBL jobBL)
        {
            _jobBL = jobBL;
        }

        [HttpGet]
        [Route("GetJobs")]
        public IActionResult GetJobs()
        {
            #region Declarations
            JobCollection l_jobCollection;
            #endregion
            try
            {
                l_jobCollection = _jobBL.GetAllJobs();

                if (l_jobCollection == null || l_jobCollection.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(l_jobCollection);
        }

        [HttpGet]
        [Route("GetEvaluationCriteria")]
        public IActionResult GetEvaluationCriteria()
        {
            #region Declarations
            JobCollection l_jobCollection;
            #endregion
            try
            {
                l_jobCollection = _jobBL.GetEvaluationCriteria();

                if (l_jobCollection == null || l_jobCollection.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(l_jobCollection);
        }
        [HttpPost]
        [Route("SaveSubCriteria")]
        public IActionResult SaveSubCriteria(string LoggedInUser, [FromBody] Job job)
        {
            try
            {
                _jobBL.SaveSubCriteria(LoggedInUser, job);
            }
            catch (Exception)
            {
                throw;
            }

            return Ok();
        }
        [HttpGet]
        [Route("GetCriteriaForJob")]
        public IActionResult GetCriteriaForJob(int argJobId)
        {
            #region Declarations
            JobCollection l_jobCollection;
            #endregion
            try
            {
                l_jobCollection = _jobBL.GetCriteriaForJob(argJobId);

                if (l_jobCollection == null || l_jobCollection.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return Ok(l_jobCollection);
        }
    }

}
