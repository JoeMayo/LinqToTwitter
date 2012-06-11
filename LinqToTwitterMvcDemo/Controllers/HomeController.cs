using System;
using LinqToTwitter;
using LinqToTwitterMvcDemo.Models;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace LinqToTwitterMvcDemo.Controllers
{
    public class HomeController : Controller
    {
        private IOAuthCredentials credentials = new SessionStateCredentials();

        private MvcAuthorizer auth;
        private TwitterContext twitterCtx;

        public ActionResult Index()
        {
            if (credentials.ConsumerKey == null || credentials.ConsumerSecret == null)
            {
                credentials.ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
                credentials.ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];
            }

            auth = new MvcAuthorizer
            {
                Credentials = credentials
            };

            auth.CompleteAuthorization(Request.Url);

            if (!auth.IsAuthorized)
            {
                Uri specialUri = new Uri(Request.Url.ToString());
                return auth.BeginAuthorization(specialUri);
            }

            twitterCtx = new TwitterContext(auth);

            var friendTweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.User &&
                       tweet.ScreenName == "AndreMayo1"                       
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
