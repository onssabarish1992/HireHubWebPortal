using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.Web.Controllers
{
    public class CandidateController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
