using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
