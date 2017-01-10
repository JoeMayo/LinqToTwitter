using LinqToTwitter;
using Microsoft.AspNetCore.Mvc;
using MvcCoreDemo.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MvcCoreDemo.Controllers
{
    public class StatusDemosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Tweet()
        {
            var sendTweetVM = new SendTweetViewModel
            {
                Text = $"Testing async LINQ to Twitter in MVC - {DateTime.Now.ToString()}"
            };

            return View(sendTweetVM);
        }

        [HttpPost]
        [ActionName("Tweet")]
        public async Task<ActionResult> TweetAsync(SendTweetViewModel tweet)
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(HttpContext.Session)
            };

            var ctx = new TwitterContext(auth);

            Status responseTweet = await ctx.TweetAsync(tweet.Text);

            var responseTweetVM = new SendTweetViewModel
            {
                Text = $"Testing async LINQ to Twitter in MVC Core - {DateTime.Now.ToString()}",
                Response = $"Tweet successful! Response from Twitter: {responseTweet.Text}"
            };

            return View(responseTweetVM);
        }

        [ActionName("HomeTimeline")]
        public async Task<ActionResult> HomeTimelineAsync()
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(HttpContext.Session)
            };

            var ctx = new TwitterContext(auth);

            var tweets =
                await
                (from tweet in ctx.Status
                 where tweet.Type == StatusType.Home
                 select new TweetViewModel
                 {
                     ImageUrl = tweet.User.ProfileImageUrl,
                     ScreenName = tweet.User.ScreenNameResponse,
                     Text = tweet.Text
                 })
                .ToListAsync();

            return View(tweets);
        }
    }
}
