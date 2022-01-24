using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

namespace LinqToTwitter.MVC.CSharp.Controllers
{
    public class OAuthController : Controller
    {
        private readonly ILogger<OAuthController> logger;

        public OAuthController(ILogger<OAuthController> logger)
        {
            this.logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> BeginAsync()
        {
            //var auth = new MvcSignInAuthorizer
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(HttpContext.Session)
                {
                    ConsumerKey = Environment.GetEnvironmentVariable("TwitterConsumerKey"),
                    ConsumerSecret = Environment.GetEnvironmentVariable("TwitterConsumerSecret")
                }
            };

            // Available in v5.1.0 - you can pass parameters that you can read in Complete(), via Request.QueryString, when Twitter returns
            //var parameters = new Dictionary<string, string> { { "my_custom_param", "val" } };
            //string twitterCallbackUrl = Request.GetDisplayUrl().Replace("Begin", "Complete");
            //return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl), parameters);
            await auth.CredentialStore.ClearAsync();
            string twitterCallbackUrl = Request.GetDisplayUrl().Replace("Begin", "Complete");
            return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl));
        }

        public async Task<ActionResult> CompleteAsync()
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
