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
        #region Page level declarations
        HttpClient client;
        private readonly IConfiguration _configuration;
        private readonly string apiBaseURL;
        #endregion

        public CandidateController(IConfiguration configuration):base(configuration)
        {
            _configuration = configuration;
            apiBaseURL = _configuration.GetValue<string>(
                "APIBaseURL");
            client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

        public async Task<IActionResult> Candidate(int argScheduleID)
        {
            var candidateScoreInfo = await GetCandidateScore(argScheduleID);
            CandidateScoreViewModel l_candidateViewModel = ConvertCandidateScoreToViewModel(candidateScoreInfo);

            ViewBag.RatingScale = CreateRatingList();

            return View(l_candidateViewModel);
        }


        /// <summary>
        /// Create dropdown for rating scale
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> CreateRatingList()
        {
            List<SelectListItem> ratingScale = new List<SelectListItem>() {
                new SelectListItem {
                    Text = "1", Value = "1"
                },
                new SelectListItem {
                    Text = "2", Value = "2"
                },
                new SelectListItem {
                    Text = "3", Value = "3"
                },
                new SelectListItem {
                    Text = "4", Value = "4"
                },
                new SelectListItem {
                    Text = "5", Value = "5"
                },
            };

            return ratingScale;

        }



        /// <summary>
        /// Cpnvert to view model
        /// </summary>
        /// <param name="candidateScoreInfo"></param>
        /// <returns></returns>
        private CandidateScoreViewModel ConvertCandidateScoreToViewModel(CandidateScore candidateScoreInfo)
        {
            CandidateScoreViewModel l_CandidateScoreViewModel = new();
            //InterviewCandidateViewModel l_InterviewCandidateViewModel = new();
            List<CandidateRatingViewModel> l_CandidateRatings = new();
            CandidateRatingViewModel l_CandidateRatingViewModel;
            try
            {
                if(candidateScoreInfo!=null && candidateScoreInfo.Candidate!=null)
                {
                    InterviewCandidateViewModel l_InterviewCandidateViewModel = new InterviewCandidateViewModel{ 
                        CandidateId = candidateScoreInfo.Candidate.CandidateID ,
                        CandidateName = candidateScoreInfo.Candidate.CandidateName,
                        RoleName = candidateScoreInfo.Candidate.JobName,
                        ScheduleID = candidateScoreInfo.Candidate.ScheduleID
                    };

                    l_CandidateScoreViewModel.candidateDetail = l_InterviewCandidateViewModel;

                    if (candidateScoreInfo.Evaluation!=null && candidateScoreInfo.Evaluation.Count > 0)
                    {
                        foreach (var rating in candidateScoreInfo.Evaluation)
                        {
                            l_CandidateRatingViewModel = new CandidateRatingViewModel();
                            l_CandidateRatingViewModel.CriteriaId = rating.CriteriaId;
                            l_CandidateRatingViewModel.CriteriaName = rating.CriteriaName;
                            l_CandidateRatingViewModel.CriteriaDescription = rating.CriteriaDescription;
                            l_CandidateRatingViewModel.Rating = rating.CriteriaScore;
                            l_CandidateRatingViewModel.Comments = rating.CriteriaComments;
                            l_CandidateRatingViewModel.JobId = rating.JobId;


                            l_CandidateRatings.Add(l_CandidateRatingViewModel);
                        }

                        l_CandidateScoreViewModel.candidateRatings = l_CandidateRatings;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return l_CandidateScoreViewModel;
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
            


        /// <summary>
        /// API call to get the candidate score
        /// </summary>
        /// <param name="argScheduleID"></param>
        /// <returns></returns>
        public async Task<CandidateScore> GetCandidateScore(int argScheduleID)
        {
            #region Declarations
            CandidateScore l_candidateScore = new();
            string l_UserURL = string.Empty;
            #endregion
            try
            {
                l_UserURL = apiBaseURL + "api/CandidateScore/GetCandidateScore?argScheduleID=" + argScheduleID;

                HttpResponseMessage l_UserData = await client.GetAsync(l_UserURL);

                if (l_UserData != null && l_UserData.IsSuccessStatusCode)
                {
                    var l_UserResponse = l_UserData.Content.ReadAsStringAsync().Result;
                    l_candidateScore = JsonConvert.DeserializeObject<CandidateScore>(l_UserResponse);
                }

            }
            catch (Exception)
            {
                throw;
            }
            return l_candidateScore;
        }
    }
}
