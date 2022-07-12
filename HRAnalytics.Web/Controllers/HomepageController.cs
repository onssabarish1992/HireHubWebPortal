using Microsoft.AspNetCore.Mvc;


namespace HRAnalytics.Web.Controllers
{
    public class HomepageController : Controller

    {
        /// <summary>
        /// Custom home page to be loaded based on logged in user role
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            if (User.IsInRole("Interviewer"))
            {
                return RedirectToAction("Index", "Interviewer");
            }
            else if(User.IsInRole("HR"))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }


    }
}
