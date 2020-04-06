using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CoreDemo.Models;
using LinqToTwitter;

namespace CoreDemo.Controllers
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
