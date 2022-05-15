using HRAnalytics.Entities;
using Microsoft.AspNetCore.Mvc;
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

        public CandidateController(IConfiguration configuration)
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
            var interviewers = await GetInterviewers();
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<UserCollection> GetInterviewers()
        {
            #region Declarations
            UserCollection l_UserCollection = new();
            string l_UserURL = string.Empty;
            string l_userType = "HR";
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
    }
}
