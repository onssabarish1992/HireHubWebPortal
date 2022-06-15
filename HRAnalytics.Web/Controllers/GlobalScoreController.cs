using System;
using HRAnalytics.Entities;
using HRAnalytics.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRAnalytics.Web.Controllers
{
	public class GlobalScoreController : HRAnalyticsBaseController
    {
        public GlobalScoreController(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<IActionResult> Index()
        {
            var candidatecollection = await GetGlobalScore();

            List<GlobalScoreViewModel> lst_global = ConvertEntityToViewModel(candidatecollection);

            return View(lst_global);
        }



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

        private List<GlobalScoreViewModel> ConvertEntityToViewModel(CandidateCollection candidates)
        {
            List<GlobalScoreViewModel> lst_score = new List<GlobalScoreViewModel>();

            GlobalScoreViewModel scoreViewModel = null;

            foreach (var item in candidates)
            {
                scoreViewModel = new GlobalScoreViewModel();
                scoreViewModel.CandidateName = item.CandidateName;
                scoreViewModel.JobName = item.JobName;
                scoreViewModel.IsHired = item.IsHired;
<<<<<<< Updated upstream
=======
                scoreViewModel.Recommendation = "Hire";
>>>>>>> Stashed changes
                scoreViewModel.ProposedCompensation = item.ProposedCompensation;
                scoreViewModel.ActualCompensation = item.ActualCompensation;

                lst_score.Add(scoreViewModel);
            }

            return lst_score;
        }
    }
}

