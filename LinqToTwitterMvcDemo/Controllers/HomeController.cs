using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using LinqToTwitter;
using LinqToTwitterMvcDemo.Models;

namespace LinqToTwitterMvcDemo.Controllers
{
    public class HomeController : Controller
    {
        readonly IOAuthCredentials credentials = new SessionStateCredentials();

        MvcAuthorizer auth;
        TwitterContext twitterCtx;

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

            // internally, this doesn't execute if BeginAuthorization hasn't been called yet
            //  but it will execute after the user authorizes your application
            auth.CompleteAuthorization(Request.Url);

            // this will only execute if we don't have all 4 keys, which is what IsAuthorized checks
            if (!auth.IsAuthorized)
            {
                Uri specialUri = new Uri(Request.Url.ToString());

                // url param is optional, it lets you specify the page Twitter redirects to.
                // You can use it to complete the OAuth process on another action/controller - in which
                // case you would move auth.CompleteAuthorization to that action/controller.
                return auth.BeginAuthorization(specialUri);
            }

            twitterCtx = new TwitterContext(auth);

            List<TweetViewModel> friendTweets = 
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.User &&
                       tweet.ScreenName == "JoeMayo"
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
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
