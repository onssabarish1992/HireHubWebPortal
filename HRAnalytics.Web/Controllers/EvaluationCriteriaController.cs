using HRAnalytics.Entities;
using HRAnalytics.Utilities;
using HRAnalytics.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace HRAnalytics.Web.Controllers
{
    [Authorize(Roles = "HR")]
    public class EvaluationCriteriaController : HRAnalyticsBaseController
    {
        public EvaluationCriteriaController(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Load evaluation criteria page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            CriteriaViewModel criteriaViewMdel = new();
            await PopulateDropdownValues();

            var criteriasCreated = await GetSavedCriterias();
            if (criteriasCreated != null)
            {
                //Convert the Jobs Into View Model
                criteriaViewMdel.CriteriasCreated = ExtractRequiredCriterias(criteriasCreated);
            }
            if (TempData[HRAnalyticsConstants.C_SETJOBSUBCRITERIAINSERTSUCCESS_CONST] != null)
            {
                ViewBag.SuccessMessage =  "Criteria Created Successfully!!";
            }
            return View(criteriaViewMdel);
        }

        /// <summary>
        /// Get the required criterias
        /// </summary>
        /// <param name="criteriasCreated"></param>
        /// <returns></returns>
        private List<SubcriteriaViewModel> ExtractRequiredCriterias(JobCollection criteriasCreated)
        {
            #region Declarations
            List<SubcriteriaViewModel> subCriteriasCreated = new();
            SubcriteriaViewModel subCriteria;
            #endregion
            try
            {
                foreach (var role in criteriasCreated)
                {
                    subCriteria = new SubcriteriaViewModel();
                    subCriteria.JobName = role.JobName;
                    subCriteria.CriteriaName = role.CriteriaName;
                    subCriteria.CriteriaDescription = role.SubCriteriaDescription;
                    subCriteria.SubCriteriaID = role.SubCriteriaId;
                    subCriteriasCreated.Add(subCriteria);
                }

                subCriteriasCreated = subCriteriasCreated.OrderBy(x => x.JobName).ToList();

            }
            catch (Exception)
            {
                throw;
            }

            return subCriteriasCreated;
        }

        /// <summary>
        /// This method is used to retrieve saved criterias from database
        /// </summary>
        /// <returns></returns>
        public async Task<JobCollection> GetSavedCriterias()
        {
            #region Declarations
            JobCollection l_JobCollection = new();
            string l_jobURL = string.Empty;
            #endregion
            try
            {
                l_jobURL = apiBaseURL + "api/Job/GetEvaluationCriteria";
                HttpResponseMessage l_jobData = await client.GetAsync(l_jobURL);

                    if (l_jobData != null && l_jobData.IsSuccessStatusCode)
                    {
                        var l_JobResponse = l_jobData.Content.ReadAsStringAsync().Result;
                        l_JobCollection = JsonConvert.DeserializeObject<JobCollection>(l_JobResponse);
                    }

            }
            catch (Exception)
            {
                throw;
            }
            return l_JobCollection;
        }

        /// <summary>
        /// Populate dropdown values 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> PopulateDropdownValues()
        {
            bool l_Executed;
            try
            {
                //This API call which fetch only the list of job roles created in set criteria screen 
                var JobsCreated = await GetJobCriterias();

                ViewBag.Citeria = Enumerable.Empty<SelectListItem>();
                ViewBag.Jobs = new SelectList(JobsCreated, "JobId", "JobName");

                l_Executed = true;
            }
            catch (Exception)
            {
                throw;
            }
            return l_Executed;
        }

        /// <summary>
        /// Get the criterias based on job role
        /// </summary>
        [HttpGet]
        public async Task<JobCollection> GetCriteriaForJobs(int argJobId)
        {
            #region Declaration
            JobCollection jobCollection = new();
            string l_jobURL = string.Empty;
            #endregion
            try
            {
                l_jobURL = apiBaseURL + "api/Job/GetCriteriaForJob?argJobId="+ argJobId;
                HttpResponseMessage l_jobData = await client.GetAsync(l_jobURL);
                if (l_jobData != null && l_jobData.IsSuccessStatusCode)
                {
                    var l_JobResponse = l_jobData.Content.ReadAsStringAsync().Result;
                    jobCollection = JsonConvert.DeserializeObject<JobCollection>(l_JobResponse);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return jobCollection;
        }

        /// <summary>
        /// Post Method for save
        /// </summary>
        /// <param name="argSuCriteria"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveJobCriteria(CriteriaViewModel argSuCriteria)
        {
            #region Declarations
            HttpResponseMessage l_Message;
            #endregion

            if (ModelState.IsValid)
            {
                if (argSuCriteria != null)
                {
                    argSuCriteria.Criteria.Mode = "Create";
                    l_Message = await SaveSubCriteria(argSuCriteria.Criteria);

                    if (l_Message.IsSuccessStatusCode)
                    {
                        TempData[HRAnalyticsConstants.C_SETJOBSUBCRITERIAINSERTSUCCESS_CONST] = HRAnalyticsConstants.C_SUCCESS_CONST;

                        return RedirectToAction("Index", "EvaluationCriteria");
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
            return RedirectToAction("Index", "EvaluationCriteria");
        }

        /// <summary>
        /// API call to save subcriteria
        /// </summary>
        /// <param name="argSubCriteria"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SaveSubCriteria(SubcriteriaViewModel argSubCriteria)
        {
            #region Declarations
            HttpResponseMessage l_Response = new HttpResponseMessage();
            Job l_Job;
            string l_SaveSubCriteriaURL = apiBaseURL + "api/Job/SaveSubCriteria?LoggedInUser=" + GetLoggedInUserID();
            #endregion
            try
            {
                if (argSubCriteria != null)
                {
                    l_Job = ConvertSubCriteriaViewModeltoEntity(argSubCriteria);
                    if (l_Job != null)
                    {
                        l_Response = await client.PostAsJsonAsync(l_SaveSubCriteriaURL, l_Job);
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
        /// Convert view model to entity
        /// </summary>
        /// <param name="argSubCriteria"></param>
        /// <returns></returns>
        private Job ConvertSubCriteriaViewModeltoEntity(SubcriteriaViewModel argSubCriteria)
        {
            #region Declaration
            Job l_job = new();
            #endregion
            try
            {
                if (argSubCriteria != null)
                {
                    l_job.JobId = argSubCriteria.JobId;
                    l_job.CriteriaID = argSubCriteria.CriteriaId;
                    l_job.SubCriteriaDescription = argSubCriteria.CriteriaDescription;
                    l_job.Mode = argSubCriteria.Mode;
                    l_job.SubCriteriaId = argSubCriteria.SubCriteriaID;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return l_job;
        }

        /// <summary>
        /// Method to delete job criteria
        /// </summary>
        /// <param name="argJobCriteria"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<HttpResponseMessage> DeleteJobCriteria(SubcriteriaViewModel argJobCriteria)
        {
            #region Declarations
            HttpResponseMessage l_Response = new HttpResponseMessage();
            // Job l_Job;
            Job l_job = new();
            string l_SaveJobRoleURL = apiBaseURL + "api/Job/SaveSubCriteria?LoggedInUser=" + GetLoggedInUserID();



            #endregion
            try
            {
                if (argJobCriteria != null)
                {
                    l_job.JobId = argJobCriteria.JobId;
                    l_job.CriteriaID = argJobCriteria.CriteriaId;
                    l_job.SubCriteriaDescription = argJobCriteria.SubCriteriaDescription;
                    l_job.SubCriteriaId = argJobCriteria.SubCriteriaID;
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
        /// Convert view model to Entity
        /// </summary>
        /// <param name="argJobCriteria"></param>
        /// <returns></returns>
        private Job ConvertJobCriteriaViewModeltoEntity(SubcriteriaViewModel argJobCriteria)
        {
            Job l_job = new();
            try
            {
                if (argJobCriteria != null)
                {
                    l_job.JobId = argJobCriteria.JobId;
                    l_job.CriteriaID = argJobCriteria.CriteriaId;
                    l_job.SubCriteriaDescription = argJobCriteria.SubCriteriaDescription;
                    l_job.SubCriteriaId = argJobCriteria.SubCriteriaID;
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
        /// Delete criteria method
        /// </summary>
        /// <param name="argID"></param>
        /// <returns></returns>
        public async Task<ActionResult> DeleteItem(int argID)
        {
            SubcriteriaViewModel l_vwmodel = new();
            l_vwmodel.Mode = "Delete";
            l_vwmodel.SubCriteriaID = argID;

            await DeleteJobCriteria(l_vwmodel);

            return RedirectToAction("Index");
        }
    }
}
