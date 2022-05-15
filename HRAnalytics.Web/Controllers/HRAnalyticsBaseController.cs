using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRAnalytics.Web.Controllers
{
    public class HRAnalyticsBaseController : Controller
    {
   

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
    }
}
