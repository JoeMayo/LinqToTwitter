using LinqToTwitter;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MvcDemo.Controllers
{
    public class OAuthController : AsyncController
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> BeginAsync()
        {
            //var auth = new MvcSignInAuthorizer
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore
                {
                    ConsumerKey = Environment.GetEnvironmentVariable("TwitterConsumerKey"),
                    ConsumerSecret = Environment.GetEnvironmentVariable("TwitterConsumerSecret")
                }
            };

            // Available in v5.1.0 - you can pass parameters that you can read in Complete(), via Request.QueryString, when Twitter returns
            //var parameters = new Dictionary<string, string> { { "my_custom_param", "val" } };
            //string twitterCallbackUrl = Request.GetDisplayUrl().Replace("Begin", "Complete");
            //return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl), parameters);

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