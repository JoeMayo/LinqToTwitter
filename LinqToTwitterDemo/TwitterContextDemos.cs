using System;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    public class TwitterContextDemos
    {
        /// <summary>
        /// Run demos that use TwitterContext members not covered by other API demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            ShowFeatureRateLimitHeadersViaResponseHeaders(twitterCtx);
        }

        /// <summary>
        /// Performs a search query and displays the X-Feature... headers from the response
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ShowFeatureRateLimitHeadersViaResponseHeaders(TwitterContext twitterCtx)
        {
            var user =
                (from usr in twitterCtx.User
                 where usr.Type == UserType.Search &&
                       usr.Query == "Joe Mayo"
                 select usr)
                 .FirstOrDefault();

            Console.WriteLine(
                "X-FeatureRateLimit-Limit: {0}\n" +
                "X-FeatureRateLimit-Remaining: {1}\n" +
                "X-FeatureRateLimit-Reset: {2}\n" +
                "X-FeatureRateLimit-Class: {3}\n",
                twitterCtx.ResponseHeaders["X-FeatureRateLimit-Limit"],
                twitterCtx.ResponseHeaders["X-FeatureRateLimit-Remaining"],
                twitterCtx.ResponseHeaders["X-FeatureRateLimit-Reset"],
                twitterCtx.ResponseHeaders["X-FeatureRateLimit-Class"]);
        }
    }
}
