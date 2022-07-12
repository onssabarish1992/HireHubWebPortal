using System;
using HRAnalytics.Entities;
using HRAnalytics.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRAnalytics.Web.Controllers
{
    [Authorize(Roles = "HR")]
    public class GlobalScoreController : HRAnalyticsBaseController
    {
        public GlobalScoreController(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Controller for results page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var candidatecollection = await GetGlobalScore();

            List<GlobalScoreViewModel> lst_global = ConvertEntityToViewModel(candidatecollection);

            return View(lst_global);
        }

        /// <summary>
        /// API call to get global scores
        /// </summary>
        /// <returns></returns>
        public async Task<CandidateCollection> GetGlobalScore()
        {
            #region Declarations
            CandidateCollection l_candidate = new();
            string l_CandidateURL = string.Empty;
            #endregion
            try
            {
                l_CandidateURL = apiBaseURL + "api/Candidate/GetGlobalScores";

                HttpResponseMessage l_UserData = await client.GetAsync(l_CandidateURL);

                if (l_UserData != null && l_UserData.IsSuccessStatusCode)
                {
                    var l_UserResponse = l_UserData.Content.ReadAsStringAsync().Result;
                    l_candidate = JsonConvert.DeserializeObject<CandidateCollection>(l_UserResponse);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return l_candidate;
        }

        /// <summary>
        /// Convert entity to view model class
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns></returns>
        private List<GlobalScoreViewModel> ConvertEntityToViewModel(CandidateCollection candidates)
        {
            List<GlobalScoreViewModel> lst_score = new List<GlobalScoreViewModel>();
            GlobalScoreViewModel scoreViewModel;

            foreach (var item in candidates)
            {
                scoreViewModel = new GlobalScoreViewModel();
                scoreViewModel.CandidateName = item.CandidateName;
                scoreViewModel.JobName = item.JobName;
                scoreViewModel.IsHired = item.IsHired;
                scoreViewModel.Recommendation = item.IsRecommended.HasValue && item.IsRecommended.Value? "Hire" : "No Hire";
                scoreViewModel.ProposedCompensation = item.ProposedCompensation;
                scoreViewModel.ActualCompensation = item.ActualCompensation;
                scoreViewModel.ScheduleId = item.ScheduleID;
                scoreViewModel.GlobalScoreId = item.GlobalScoreId;
                lst_score.Add(scoreViewModel);
            }

            return lst_score;
        }

        /// <summary>
        /// Save compensation and actual hiring decision
        /// </summary>
        /// <param name="argIsHired"></param>
        /// <param name="argActualCompensation"></param>
        /// <param name="argGlobalScoreID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveCandidateHiringDetails(bool argIsHired, double argActualCompensation, int argGlobalScoreID)
        {
            bool IsSuccess = false;
            HttpResponseMessage l_Message = new HttpResponseMessage();
            Candidate l_Candidate = new Candidate();
            try
            {
                l_Candidate.GlobalScoreId = argGlobalScoreID;
                l_Candidate.ActualCompensation = argActualCompensation;
                l_Candidate.IsHired = argIsHired;
                l_Candidate.ProjectName = string.Empty;
                l_Candidate.CandidateName = string.Empty;
                l_Candidate.InterviewerID = string.Empty;


                l_Message = await SaveCandidateHiringDetails(l_Candidate);

                if (l_Message != null && l_Message.IsSuccessStatusCode)
                {
                    IsSuccess = true;
                }

            }
            catch (Exception)
            {
                throw;
            }

            return Json(IsSuccess);
        }

        /// <summary>
        /// API call to save the details
        /// </summary>
        /// <param name="argCandidate"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SaveCandidateHiringDetails(Candidate argCandidate)
        {
            #region Declarations
            HttpResponseMessage l_Response = new HttpResponseMessage();
            string l_SaveCandidateResultURL = apiBaseURL + "api/CandidateScore/SaveResult";
            #endregion
            try
            {
                l_Response = await client.PostAsJsonAsync(l_SaveCandidateResultURL, argCandidate);
            }
            catch (Exception)
            {
                throw;
            }
            return l_Response;
        }
    }
}

