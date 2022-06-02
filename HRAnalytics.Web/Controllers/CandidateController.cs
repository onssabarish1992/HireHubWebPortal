using HRAnalytics.Entities;
using HRAnalytics.Utilities;
using HRAnalytics.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HRAnalytics.Web.Controllers
{
    public class CandidateController : HRAnalyticsBaseController
    {
        

        public CandidateController(IConfiguration configuration):base(configuration)
        {
        }

        public async Task<IActionResult> Index()
        {
            await PopulateDropdownValues();

            if (TempData[HRAnalyticsConstants.C_SCHEDULEINTERVIEW_CONST] != null)
            {
                ViewBag.SuccessMessage = "Candidate Interview Scheduled Successfully!!";
            }
            return View();
        }

        public async Task<IActionResult> Candidate()
        {

            return View();
        }

        /// <summary>
        /// Get interviewers for scheduling interview
        /// </summary>
        /// <returns></returns>
        public async Task<UserCollection> GetInterviewers()
        {
            #region Declarations
            UserCollection l_UserCollection = new();
            string l_UserURL = string.Empty;
            string l_userType = "INTERVIEWER";
            #endregion
            try
            {
                l_UserURL = apiBaseURL + "api/Candidate/GetInterviewers?argUserType=" + l_userType;
                HttpResponseMessage l_UserData = await client.GetAsync(l_UserURL);

                if (l_UserData != null && l_UserData.IsSuccessStatusCode)
                {
                    var l_UserResponse = l_UserData.Content.ReadAsStringAsync().Result;
                    l_UserCollection = JsonConvert.DeserializeObject<UserCollection>(l_UserResponse);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return l_UserCollection;
        }

        /// <summary>
        /// Post call handler from View
        /// </summary>
        /// <param name="argCandidate"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveCandidateSchedule(CandidateViewModel argCandidate)
        {
            HttpResponseMessage l_Message = new HttpResponseMessage();
            if (ModelState.IsValid)
            {
                if (argCandidate != null)
                {
                    l_Message = await SaveInterviewSchedule(argCandidate);

                    if (l_Message.IsSuccessStatusCode)
                    {
                        TempData[HRAnalyticsConstants.C_SCHEDULEINTERVIEW_CONST] = HRAnalyticsConstants.C_SUCCESS_CONST;

                        return RedirectToAction("Index", "Candidate");
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
        /// Function to populate interviewer and job details
        /// </summary>
        /// <returns></returns>
        private async Task<bool> PopulateDropdownValues()
        {
            bool l_Executed;
            try
            {
                var Interviewers = await GetInterviewers();
                var Jobs = await GetJobs();

                ViewBag.Interviewers = new SelectList(Interviewers, "UserID", "FullName");
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
        /// Save interview schedule API call
        /// </summary>
        /// <param name="argCandidateData"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SaveInterviewSchedule(CandidateViewModel argCandidateData)
        {
            #region Declarations
            HttpResponseMessage l_Response = new HttpResponseMessage();
            Candidate l_Candidate;
            string l_SaveCandidateScheduleURL = apiBaseURL + "api/Candidate/SaveInterviewSchecule?LoggedInUser=" + GetLoggedInUserID();
            #endregion

            try
            {
                l_Candidate = ConvertCandidateViewModeltoEntity(argCandidateData);
                if (l_Candidate != null)
                {
                    l_Response = await client.PostAsJsonAsync(l_SaveCandidateScheduleURL, l_Candidate);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_Response;
        }


        /// <summary>
        /// Convert view model to entit
        /// </summary>
        /// <param name="argCandidateViewModel"></param>
        /// <returns></returns>
        private Candidate ConvertCandidateViewModeltoEntity(CandidateViewModel argCandidateViewModel)
        {
            Candidate l_Candidate = new();
            try
            {
                if (argCandidateViewModel != null)
                {
                    l_Candidate.CandidateName = argCandidateViewModel.CandidateName;
                    l_Candidate.ProjectName = argCandidateViewModel.Project;
                    l_Candidate.InterviewerID = argCandidateViewModel.UserID;
                    l_Candidate.InterviewTimeStamp = argCandidateViewModel.InterviewSchedule;
                    l_Candidate.JobId = argCandidateViewModel.JobId;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return l_Candidate;
        }
    }
}
