using System;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoreDemo.Controllers
{
    public class OAuthController : Controller
    {
        readonly IConfiguration configuration;

        public OAuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Begin()
        {
            //var auth = new MvcSignInAuthorizer
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(HttpContext.Session)
                {
                    ConsumerKey = configuration["Twitter:ConsumerKey"],
                    ConsumerSecret = configuration["Twitter:ConsumerSecret"]
                }
            };

            string twitterCallbackUrl = Request.GetDisplayUrl().Replace("Begin", "Complete");
            return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl));
        }

        public async Task<ActionResult> Complete()
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(HttpContext.Session)
            };

            await auth.CompleteAuthorizeAsync(new Uri(Request.GetDisplayUrl()));

            // This is how you access credentials after authorization.
            // The oauthToken and oauthTokenSecret do not expire.
            // You can use the userID to associate the credentials with the user.
            // You can save credentials any way you want - database, 
            //   isolated storage, etc. - it's up to you.
            // You can retrieve and load all 4 credentials on subsequent 
            //   queries to avoid the need to re-authorize.
            // When you've loaded all 4 credentials, LINQ to Twitter will let 
            //   you make queries without re-authorizing.
            //
            //var credentials = auth.CredentialStore;
            //string oauthToken = credentials.OAuthToken;
            //string oauthTokenSecret = credentials.OAuthTokenSecret;
            //string screenName = credentials.ScreenName;
            //ulong userID = credentials.UserID;
            //

            return RedirectToAction("Index", "Home");
        }
    }
}