using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.Web.Controllers
{
    //Only HR has access to this page
    [Authorize(Roles = "HR")]
    public class AdminController : HRAnalyticsBaseController
    {
        public AdminController(IConfiguration configuration) : base(configuration)
        {
        }

        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// On click event of admin processing options (AHP, TOPSIS and Prescriptive analytics)
        /// </summary>
        /// <param name="argRequestType"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ProcessData(string argRequestType)
        {
            bool IsSuccess = false;
            HttpResponseMessage l_Message = new HttpResponseMessage();

            try
            {
                if(argRequestType == "SAVEAHPPAIR")
                {
                    l_Message = await SavePairs();
                }
                else if(argRequestType == "SAVEAHPSCORES")
                {
                    l_Message = await SaveAHPAllScores();
                }
                else if(argRequestType == "CALCULATERESULTS")
                {
                    l_Message = await SaveScores();
                }

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
        /// API call to save AHP Pairs
        /// </summary>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SavePairs()
        {
            #region Declarations
            HttpResponseMessage l_Response = new HttpResponseMessage();
            string l_SaveAHPRatingsURL = apiBaseURL + "api/AHP/SavePairs?argLoggedInUser=" + GetLoggedInUserID();
            #endregion
            try
            {
               l_Response = await client.PostAsync(l_SaveAHPRatingsURL, null); 
            }
            catch (Exception)
            {
                throw;
            }
            return l_Response;
        }


        /// <summary>
        /// API call to generate AHP Scores
        /// </summary>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SaveAHPAllScores()
        {
            #region Declarations
            HttpResponseMessage l_Response = new HttpResponseMessage();
            string l_SaveAHPRatingsURL = apiBaseURL + "api/AHP/SaveAHPAllScores?argLoggedInUserID=" + GetLoggedInUserID();
            #endregion
            try
            {
                l_Response = await client.PostAsync(l_SaveAHPRatingsURL, null);
            }
            catch (Exception)
            {
                throw;
            }
            return l_Response;
        }


        /// <summary>
        /// Calculate candidate rankings using AHP and set final results using Prescriptive analytics (MIP)
        /// </summary>
        /// <returns></returns>
        private async Task<HttpResponseMessage> SaveScores()
        {
            #region Declarations
            HttpResponseMessage l_Response = new HttpResponseMessage();
            string l_SaveAHPRatingsURL = apiBaseURL + "api/CandidateScore/SaveScores?argLoggedInUser=" + GetLoggedInUserID();
            #endregion
            try
            {
                l_Response = await client.PostAsync(l_SaveAHPRatingsURL, null);
            }
            catch (Exception)
            {
                throw;
            }
            return l_Response;
        }
    }
}
