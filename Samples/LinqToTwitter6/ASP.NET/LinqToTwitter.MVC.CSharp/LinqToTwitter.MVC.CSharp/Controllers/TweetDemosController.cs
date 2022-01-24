using LinqToTwitter;
using LinqToTwitter.Common;
using LinqToTwitter.MVC.CSharp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcDemo.Controllers
{
    public class TweetDemosController : Controller
    {
        readonly IWebHostEnvironment webHostEnvironment;

        public TweetDemosController(IWebHostEnvironment webHostEnvironment)
        {
            this.webHostEnvironment = webHostEnvironment;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tweet()
        {
            var sendTweetVM = new SendTweetViewModel
            {
                Text = "Testing async LINQ to Twitter in MVC - " + DateTime.Now.ToString()
            };

            return View(sendTweetVM);
        }

        [HttpPost]
        [ActionName("Tweet")]
        public async Task<ActionResult> TweetAsync(SendTweetViewModel tweet)
        {
            var auth = new MvcOAuth2Authorizer
            {
                CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
            };

            var ctx = new TwitterContext(auth);

            Tweet responseTweet = await ctx.TweetAsync(tweet.Text);

            var responseTweetVM = new SendTweetViewModel
            {
                Text = "Testing async LINQ to Twitter in MVC - " + DateTime.Now.ToString(),
                Response = "Tweet successful! Response from Twitter: " + responseTweet.Text
            };

            return View(responseTweetVM);
        }

        [ActionName("TweetTimeline")]
        public async Task<ActionResult> TweetTimelineAsync()
        {
            var auth = new MvcOAuth2Authorizer
            {
                CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
            };

            var ctx = new TwitterContext(auth);

            var userQuery =
                await
                (from usr in ctx.TwitterUser
                 where usr.Type == UserType.UsernameLookup &&
                       usr.Usernames == "Linq2Twitr" &&
                       usr.UserFields == UserField.ProfileImageUrl
                 select usr)
                .SingleOrDefaultAsync();

            TwitterUser user = userQuery.Users.FirstOrDefault();

            var tweetQuery =
                await
                (from tweet in ctx.Tweets
                 where tweet.Type == TweetType.TweetsTimeline &&
                       tweet.ID == user.ID.ToString()
                 select tweet)
                .SingleOrDefaultAsync();
            
            var tweets =
                (from tweet in tweetQuery.Tweets
                 select new TweetViewModel
                 {
                    ImageUrl = user.ProfileImageUrl,
                    ScreenName = user.Name,
                    Text = tweet.Text
                 })
                .ToList();

            return View(tweets);
        }

        [ActionName("UploadImage")]
        public async Task<ActionResult> UploadImageAsync()
        {
            var auth = new MvcOAuth2Authorizer
            {
                CredentialStore = new OAuth2SessionCredentialStore(HttpContext.Session)
            };

            var ctx = new TwitterContext(auth);

            string status = $"Testing multi-image tweet #Linq2Twitter £ {DateTime.Now}";
            string mediaCategory = "tweet_image";

            string path = 
                Path.Combine(
                    webHostEnvironment.WebRootPath, 
                    "images", 
                    "200xColor_2.png");
            var imageUploadTasks =
                new List<Task<Media>>
                {
                    ctx.UploadMediaAsync(System.IO.File.ReadAllBytes(path), "image/jpg", mediaCategory),
                };

            await Task.WhenAll(imageUploadTasks);

            List<string> mediaIds =
                (from tsk in imageUploadTasks
                 select tsk.Result.MediaID.ToString())
                .ToList();

            Tweet tweet = await ctx.TweetMediaAsync(status, mediaIds);

            return View(
                new MediaViewModel
                {
                    MediaUrl = tweet.Entities.Urls.FirstOrDefault()?.Url,
                    Description = tweet.Entities.Urls.FirstOrDefault()?.Description,
                    Text = tweet.Text
                });
        }
    }
}