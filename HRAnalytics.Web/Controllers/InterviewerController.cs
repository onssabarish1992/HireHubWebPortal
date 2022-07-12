using HRAnalytics.Entities;
using HRAnalytics.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HRAnalytics.Web.Controllers
{
    public class InterviewerController : HRAnalyticsBaseController
    {
        public InterviewerController(IConfiguration configuration) : base(configuration)
        {

        }

        /// <summary>
        /// Default landing page for interviewer
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var candidatecollection = await GetCandidateCollection();

            List<CandidateViewModel> lst_candidate = ConvertEntityToViewModel(candidatecollection);

            return View(lst_candidate);
        }


        /// <summary>
        /// API call to get list of candidates assigned to interviewer
        /// </summary>
        /// <returns></returns>
        public async Task<CandidateCollection> GetCandidateCollection()
        {
            #region Declarations
            CandidateCollection l_candidate = new();
            string l_CandidateURL = string.Empty;
            #endregion
            try
            {
                l_CandidateURL = apiBaseURL + "api/Candidate/GetCandidateForInterviewer?argInterviewerID=" + GetLoggedInUserID();

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
        /// Convert entity to view model
        /// </summary>
        /// <param name="candidates"></param>
        /// <returns></returns>
        private List<CandidateViewModel> ConvertEntityToViewModel(CandidateCollection candidates)
        {
            List<CandidateViewModel> lst_candidate = new List<CandidateViewModel>();

            CandidateViewModel candidateViewModel = null;

            foreach (var item in candidates)
            {
                candidateViewModel = new CandidateViewModel();
                candidateViewModel.CandidateName = item.CandidateName;
                candidateViewModel.JobName = item.JobName;
                candidateViewModel.InterviewSchedule = item.InterviewTimeStamp;
                candidateViewModel.ScheduleID = item.ScheduleID;
                candidateViewModel.Status = item.InterviewStatus;

                lst_candidate.Add(candidateViewModel);
            }

            return lst_candidate;
        }
    }
}
