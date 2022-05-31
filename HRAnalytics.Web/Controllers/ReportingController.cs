using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.Web.Controllers
{
    public class ReportingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
