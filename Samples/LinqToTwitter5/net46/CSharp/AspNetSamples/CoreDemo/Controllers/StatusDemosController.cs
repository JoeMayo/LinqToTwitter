using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreDemo.Models;
using LinqToTwitter;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace CoreDemo.Controllers
{
    public class StatusDemosController : Controller
    {
        readonly IHostingEnvironment env;

        public StatusDemosController(IHostingEnvironment env)
        {
            this.env = env;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tweet()
        {
            var sendTweetVM = new SendTweetViewModel
            {
                Text = "Testing LINQ to Twitter in ASP.NET Core - " + DateTime.Now.ToString()
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
                Text = tweet.Text,
                Response = "Tweet successful! Response from Twitter: " + responseTweet.Text
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
                 where tweet.Type == StatusType.Home &&
                       tweet.TweetMode == TweetMode.Extended
                 select new TweetViewModel
                 {
                     ImageUrl = tweet.User.ProfileImageUrl,
                     ScreenName = tweet.User.ScreenNameResponse,
                     Text = tweet.FullText
                 })
                .ToListAsync();

            return View(tweets);
        }

        [ActionName("UploadImage")]
        public async Task<ActionResult> UploadImageAsync()
        {
            var auth = new MvcAuthorizer
            {
                CredentialStore = new SessionStateCredentialStore(HttpContext.Session)
            };

            var twitterCtx = new TwitterContext(auth);

            string status = $"Testing multi-image tweet #Linq2Twitter £ {DateTime.Now}";
            string mediaCategory = "tweet_image";

            string path = Path.Combine(env.WebRootPath, "images\\200xColor_2.png");
            var imageUploadTasks =
                new List<Task<Media>>
                {
                    twitterCtx.UploadMediaAsync(System.IO.File.ReadAllBytes(path), "image/jpg", mediaCategory),
                };

            await Task.WhenAll(imageUploadTasks);

            List<ulong> mediaIds =
                (from tsk in imageUploadTasks
                 select tsk.Result.MediaID)
                .ToList();

            Status tweet = await twitterCtx.TweetAsync(status, mediaIds);

            return View(
                new TweetViewModel
                {
                    ImageUrl = tweet.User.ProfileImageUrl,
                    ScreenName = tweet.User.ScreenNameResponse,
                    Text = tweet.Text
                });
        }
    }
}