using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LinqToTwitter;
using Microsoft.Extensions.Options;
using MvcCoreDemo.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcCoreDemo.Controllers
{
    public class OAuthController : Controller
    {
        TwitterKeys twitterKeys;
        public OAuthController(IOptions<TwitterKeys> twitterKeys)
        {
            this.twitterKeys = twitterKeys.Value;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> BeginAsync()
        {
            //var auth = new MvcSignInAuthorizer
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(HttpContext.Session)
                {
                    ConsumerKey = twitterKeys.TwitterConsumerKey,
                    ConsumerSecret = twitterKeys.TwitterConsumerSecret
                }
            };

            string callingUrl = Url.Action(nameof(CompleteAsync), "OAuth", new { }, Request.Scheme);

            return await auth.BeginAuthorizationAsync(new Uri(callingUrl));
        }

        public async Task<IActionResult> CompleteAsync()
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(HttpContext.Session)
            };

            // It's important to make sure that the "oauth_verifier" is appended to the query string 
            // because CompleteAuthorizeAsync uses it to finish the OAuth sequence with Twitter.
            string callingUrl = Url.Action(nameof(CompleteAsync), "OAuth", new { oauth_verifier = Request.Query["oauth_verifier"] }, Request.Scheme);

            await auth.CompleteAuthorizeAsync(new Uri(callingUrl));

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

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
