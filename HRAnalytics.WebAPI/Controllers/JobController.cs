using HRAnalytics.BL.Interfaces;
using HRAnalytics.Entities;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    
    {
        private readonly TelemetryClient _telemetryClient;

        IJobBL _jobBL;



        public JobController(TelemetryClient telemetryClient, IJobBL jobBL)
        {
            _telemetryClient = telemetryClient;
            _jobBL = jobBL;
        }


        [HttpGet]
        [Route("GetJobs")]
        public IActionResult GetJobs()
        {
            #region Declarations
            JobCollection l_jobCollection = new();
            #endregion
            try
            {
                l_jobCollection = _jobBL.GetAllJobs();

                if (l_jobCollection == null || l_jobCollection.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackTrace("Exception caught GetEvaluationCriteria method...");
                _telemetryClient.TrackException(ex);
            }
            

            return Ok(l_jobCollection);
        }

        [HttpGet]
        [Route("GetEvaluationCriteria")]
        public IActionResult GetEvaluationCriteria()
        {
            #region Declarations
            JobCollection l_jobCollection=new();
            #endregion
            try
            {
                l_jobCollection = _jobBL.GetEvaluationCriteria();

                if (l_jobCollection == null || l_jobCollection.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                
               _telemetryClient.TrackTrace("Exception caught in GetEvaluationCriteria method...");
               _telemetryClient.TrackException(ex);
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
            catch (Exception ex)
            {
                
                _telemetryClient.TrackTrace("Exception caught GetEvaluationCriteria method...");
                _telemetryClient.TrackException(ex);
                
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
            catch (Exception ex)
            {
                
               _telemetryClient.TrackTrace("Exception caught GetEvaluationCriteria method...");
               _telemetryClient.TrackException(ex);
                
                throw;
            }

            return Ok(l_jobCollection);
        }

        [HttpGet]
        [Route("GetRoles")]
        public IActionResult GetRoles()
        {
            #region Declarations
            JobCollection l_jobCollection;
            #endregion
            try
            {
                l_jobCollection = _jobBL.GetRoles();

                if (l_jobCollection == null || l_jobCollection.Count == 0)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                
                _telemetryClient.TrackTrace("Exception caught GetEvaluationCriteria method...");
                _telemetryClient.TrackException(ex);
                
                throw;
            }

            return Ok(l_jobCollection);
        }

        [HttpPost]
        [Route("SaveRole")]
        public IActionResult SaveRole(string LoggedInUser, [FromBody] Job job)
        {
            try
            {
                _jobBL.SaveRole(LoggedInUser, job);
            }
            catch (Exception ex)

            {
                
               _telemetryClient.TrackTrace("Exception caught GetEvaluationCriteria method...");
               _telemetryClient.TrackException(ex);
                
                BadRequest();
            }

            return Ok();
        }
    }

}
