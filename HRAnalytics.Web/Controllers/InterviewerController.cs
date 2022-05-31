using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.Web.Controllers
{
    public class InterviewerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
