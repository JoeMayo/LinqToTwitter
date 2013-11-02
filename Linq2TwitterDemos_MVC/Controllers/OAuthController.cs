using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LinqToTwitter;

namespace Linq2TwitterDemos_MVC.Controllers
{
    public class OAuthController : AsyncController
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> BeginAsync()
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"]
                }
            };

            string twitterCallbackUrl = Request.Url.ToString().Replace("Begin", "Complete");
            return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl));
        }

        public async Task<ActionResult> CompleteAsync()
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore()
            };

            await auth.CompleteAuthorizeAsync(Request.Url);

            return RedirectToAction("Index", "Home");
        }
    }
}