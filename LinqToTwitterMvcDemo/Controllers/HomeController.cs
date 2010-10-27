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
        private MvcAuthorizer auth;
        private TwitterContext twitterCtx;

        public ActionResult Index()
        {
            auth = Session["MvcAuthorizer"] as MvcAuthorizer;

            if (auth == null)
            {
                string consumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"];
                string consumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"];

                auth = new MvcAuthorizer
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                };

                Session["MvcAuthorizer"] = auth;
            }

            auth.CompleteAuthorization();

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
