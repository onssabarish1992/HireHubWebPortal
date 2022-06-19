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
            return View();
        }


    }
}
