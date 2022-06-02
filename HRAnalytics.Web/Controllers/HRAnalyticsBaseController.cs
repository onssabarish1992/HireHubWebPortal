using HRAnalytics.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace HRAnalytics.Web.Controllers
{
    public class HRAnalyticsBaseController : Controller
    {
        private readonly IConfiguration _configuration;
        HttpClient client;
        private readonly string apiBaseURL;
        public HRAnalyticsBaseController(IConfiguration configuration)
        {
            _configuration = configuration;
            apiBaseURL = _configuration.GetValue<string>(
               "APIBaseURL");
            client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseURL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Get UserID of logged in user
        /// </summary>
        /// <returns></returns>
        protected string GetLoggedInUserID()
        {
            string loggedInUserID;
            try
            {
                loggedInUserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            catch (Exception)
            {
                throw;
            }
            return loggedInUserID;
        }

        /// <summary>
        /// Get Job Master data from APIs
        /// </summary>
        /// <returns></returns>
        public async Task<JobCollection> GetJobs()
        {
            #region Declarations
            JobCollection l_JobCollection = new();
            string l_jobURL = string.Empty;
            #endregion
            try
            {
                l_jobURL = apiBaseURL + "api/Job/GetJobs";
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
    }
}
