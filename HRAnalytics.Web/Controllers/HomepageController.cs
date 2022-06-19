using Microsoft.AspNetCore.Mvc;


namespace HRAnalytics.Web.Controllers
{
    public class HomepageController : Controller

    {
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
