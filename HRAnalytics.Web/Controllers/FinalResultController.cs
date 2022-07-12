using Microsoft.AspNetCore.Mvc;

namespace HRAnalytics.Web.Controllers
{
    public class FinalResultController : Controller
    {
        /// <summary>
        /// Mockup page to display final result
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
