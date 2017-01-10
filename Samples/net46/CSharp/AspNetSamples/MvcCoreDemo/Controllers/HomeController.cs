using LinqToTwitter;
using Microsoft.AspNetCore.Mvc;

namespace MvcCoreDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (!new SessionStateCredentialStore(HttpContext.Session).HasAllCredentials())
                return RedirectToAction("Index", "OAuth");

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
