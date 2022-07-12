using HRAnalytics.Entities;
using HRAnalytics.Utilities;
using HRAnalytics.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HRAnalytics.Web.Controllers
{
    [Authorize(Roles = "HR")]
    public class RoleCriteriaController : HRAnalyticsBaseController
    {

        public RoleCriteriaController(IConfiguration configuration):base(configuration)
        {
        }

        /// <summary>
        /// Default page for job role
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            #region Declarations
            JobRoleViewModel l_jobRoleModel = new();
            #endregion

            await PopulateDropdownValues();

            //Get existing job criterias created
            var JobsCreated = await GetJobCriterias();

            if (JobsCreated != null)
            {
                //Convert the Jobs Into View Model
                l_jobRoleModel.CiiteriasCreated = ExtractRequiredCriterias(JobsCreated);
            }

            if (TempData[HRAnalyticsConstants.C_SETJOBCRITERIAINSERTSUCCESS_CONST] != null)
            {
                ViewBag.SuccessMessage = "Job Criteria Created Successfully!!";
            }

            return View(l_jobRoleModel);
        }

        /// <summary>
        /// Get required criterias and form the entity
        /// </summary>
        /// <param name="jobsCreated"></param>
        /// <returns></returns>
        private List<JobCriteriaViewModel> ExtractRequiredCriterias(JobCollection jobsCreated)
        {
            #region Declarations
            List<JobCriteriaViewModel> rolesCreated = new();
            JobCriteriaViewModel jobRole;
            #endregion
            try
            {
                foreach (var role in jobsCreated)
                {
                    jobRole = new JobCriteriaViewModel();
                    jobRole.JobName = role.JobName;
                    jobRole.Position = role.Position;
                    jobRole.Compensation = role.Compensation;
                    jobRole.JobDescription = role.JobDescription;
                    jobRole.JobCriteriaID = role.JobCriteriaId;

                    rolesCreated.Add(jobRole);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return rolesCreated;
        }


        /// <summary>
        /// Populate Job Role Dropdown values from database
        /// </summary>
        private async Task<bool> PopulateDropdownValues()
        {
            bool l_Executed;
            try
            {
                var Jobs = await GetJobs();

                ViewBag.Jobs = new SelectList(Jobs, "JobId", "JobName");

                l_Executed = true;
            }
            catch (Exception)
            {
                throw;
            }

            return l_Executed;
        }

        /// <summary>
        /// Post request to save job role
        /// </summary>
        /// <param name="argJobCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveJobRole(JobRoleViewModel argJobCriteria)
        {
            #region Declarations
            HttpResponseMessage l_Message;
            #endregion

            if (ModelState.IsValid)
            {
                if (argJobCriteria != null)
                {
                    l_Message = await SaveJobCriteria(argJobCriteria);

                    if (l_Message.IsSuccessStatusCode)
                    {
                        TempData[HRAnalyticsConstants.C_SETJOBCRITERIAINSERTSUCCESS_CONST] = HRAnalyticsConstants.C_SUCCESS_CONST;

                        return RedirectToAction("Index", "RoleCriteria");
                    }
                    else
                    {
                        await PopulateDropdownValues();

                        ViewBag.ErrorMessage = "Some Error In Application!";
                        return View("Index");
                    }
                }
            }
            else
            {
                await PopulateDropdownValues();

                ViewBag.ErrorMessage = "Some Error In Application!";
                return View("Index");
            }
            return RedirectToAction("Index", "Candidate");
        }

        /// <summary>
        /// API call for job role
        /// </summary>
        /// <param name="argJobCriteria"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SaveJobCriteria(JobRoleViewModel argJobCriteria)
        {
            #region Declarations
            HttpResponseMessage l_Response = new HttpResponseMessage();
            Job l_Job;
            string l_SaveJobRoleURL = apiBaseURL + "api/Job/SaveRole?LoggedInUser=" + GetLoggedInUserID();
            #endregion
            try
            {
                if (argJobCriteria != null)
                {
                    argJobCriteria.JobCriteriaViewModel.Mode = "Create";
                    l_Job = ConvertJobCriteriaViewModeltoEntity(argJobCriteria.JobCriteriaViewModel);
                    if (l_Job != null)
                    {
                        l_Response = await client.PostAsJsonAsync(l_SaveJobRoleURL, l_Job);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }



            return l_Response;
        }

        /// <summary>
        /// Save Job Criteria using API call
        /// </summary>
        /// <param name="argJobCriteria"></param>
        /// <returns></returns>s
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteJobCriteria(JobCriteriaViewModel argJobCriteria)
        {
            #region Declarations
            HttpResponseMessage l_Response = new HttpResponseMessage();
            // Job l_Job;
            Job l_job = new();
            string l_SaveJobRoleURL = apiBaseURL + "api/Job/SaveRole?LoggedInUser=" + GetLoggedInUserID();

           

            #endregion
            try
            {
                if(argJobCriteria!=null)
                {
                    l_job.JobId = argJobCriteria.JobId;
                    l_job.Position = argJobCriteria.Position;
                    l_job.Compensation = argJobCriteria.Compensation;
                    l_job.JobDescription = argJobCriteria.JobDescription;
                    l_job.ClosingDate = argJobCriteria.ClosingDate.HasValue ? argJobCriteria.ClosingDate : null;
                    l_job.JobCriteriaId = argJobCriteria.JobCriteriaID;
                    l_job.Mode = argJobCriteria.Mode;
                    l_job = ConvertJobCriteriaViewModeltoEntity(argJobCriteria);

                    if (l_job != null)
                    {
                        l_Response = await client.PostAsJsonAsync(l_SaveJobRoleURL, l_job);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_Response;
        }

        /// <summary>
        /// Save record to database by API call
        /// </summary>
        /// <param name="argJobCriteria"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Job ConvertJobCriteriaViewModeltoEntity(JobCriteriaViewModel argJobCriteria)
        {
            Job l_job = new();
            try
            {
                if (argJobCriteria != null)
                {
                    l_job.JobId = argJobCriteria.JobId;
                    l_job.Position = argJobCriteria.Position;
                    l_job.Compensation = argJobCriteria.Compensation;
                    l_job.JobDescription = argJobCriteria.JobDescription;
                    l_job.ClosingDate = argJobCriteria.ClosingDate.HasValue? argJobCriteria.ClosingDate:null;
                    l_job.JobCriteriaId = argJobCriteria.JobCriteriaID;
                    l_job.Mode = argJobCriteria.Mode;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return l_job;
        }

        /// <summary>
        /// Delete job role option
        /// </summary>
        /// <param name="argID"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteItem(int argID)
        {
            JobCriteriaViewModel l_vwmodel = new();
            l_vwmodel.Mode = "Delete";
            l_vwmodel.JobCriteriaID = argID;

            await DeleteJobCriteria(l_vwmodel);

            return RedirectToAction("Index");
        }
    }
}
