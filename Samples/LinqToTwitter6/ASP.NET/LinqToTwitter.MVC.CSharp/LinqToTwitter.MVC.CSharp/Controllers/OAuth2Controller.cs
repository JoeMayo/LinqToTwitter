using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using LinqToTwitter.OAuth;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace LinqToTwitter.MVC.CSharp.Controllers
{
    public class OAuth2Controller : Controller
    {
        private readonly ILogger<OAuth2Controller> logger;

        public OAuth2Controller(ILogger<OAuth2Controller> logger)
        {
            this.logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> BeginAsync()
        {
            string twitterCallbackUrl = Request.GetDisplayUrl().Replace("Begin", "Complete");

            var auth = new MvcOAuth2Authorizer
            {
                CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
                {
                    ClientID = Environment.GetEnvironmentVariable(OAuthKeys.TwitterClientID),
                    ClientSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterClientSecret),
                    Scopes = new List<string>
                    {
                        "tweet.read",
                        "tweet.write",
                        "tweet.moderate.write",
                        "users.read",
                        "follows.read",
                        "follows.write",
                        "offline.access",
                        "space.read",
                        "mute.read",
                        "mute.write",
                        "like.read",
                        "like.write",
                        "block.read",
                        "block.write"
                    },
                    RedirectUri = twitterCallbackUrl,
                }
            };

            return await auth.BeginAuthorizeAsync("MyState");
        }

        public async Task<ActionResult> CompleteAsync()
        {
            var auth = new MvcOAuth2Authorizer
            {
                CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
            };

            Request.Query.TryGetValue("code", out StringValues code);
            Request.Query.TryGetValue("state", out StringValues state);

            await auth.CompleteAuthorizeAsync(code, state);

            return RedirectToAction("Index", "Home");
        }
    }
}
