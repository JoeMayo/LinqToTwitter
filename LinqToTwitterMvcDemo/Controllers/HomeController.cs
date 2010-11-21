using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LinqToTwitter;
using LinqToTwitterMvcDemo.Models;
using System.Configuration;

namespace LinqToTwitterMvcDemo.Controllers
{
    public class HomeController : Controller
    {
        private const string OAuthCredentialsKey = "OAuthCredentialsKey";

        private MvcAuthorizer auth;
        private TwitterContext twitterCtx;

        public ActionResult Index()
        {
            string authString = Session[OAuthCredentialsKey] as string;

            if (authString == null)
            {
                string consumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
                string consumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];

                auth = new MvcAuthorizer
                {
                    Credentials = new InMemoryCredentials
                    {
                        ConsumerKey = consumerKey,
                        ConsumerSecret = consumerSecret
                    }
                };

                Session[OAuthCredentialsKey] = auth.ToString();
            }
            else
            {
                var credentials = new InMemoryCredentials();
                credentials.Load(authString);
                auth.Credentials = credentials;
            }

            auth.CompleteAuthorization(Request.Url);

            if (!auth.IsAuthorized)
            {
                return auth.BeginAuthorization();
            }

            twitterCtx = new TwitterContext(auth);

            var friendTweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Friends
                 select new TweetViewModel
                 {
                     ImageUrl = tweet.User.ProfileImageUrl,
                     ScreenName = tweet.User.Identifier.ScreenName,
                     Tweet = tweet.Text
                 })
                .ToList();

            return View(friendTweets);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
