using System.Diagnostics;
using System.Net;
using LinqToTwitter;
using System;
using System.Linq;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows help demos
    /// </summary>
    public class HelpDemos
    {
        /// <summary>
        /// Run all help related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            GetHelpConfiguration(twitterCtx);
            //GetHelpLanguages(twitterCtx);
            //GetHelpRateLimits(twitterCtx);
            //ExceedRateLimitDemo(twitterCtx);
        }

        /// <summary>
        /// shows how to get configuration info
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetHelpConfiguration(TwitterContext twitterCtx)
        {
            var helpResult =
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Configuration
                 select test)
                .SingleOrDefault();

            Configuration cfg = helpResult.Configuration;

            Console.WriteLine("Short URL Length: " + cfg.ShortUrlLength);
            Console.WriteLine("Short URL HTTPS Length: " + cfg.ShortUrlLengthHttps);
            Console.WriteLine("Non-UserName Paths: ");
            foreach (var name in cfg.NonUserNamePaths)
            {
                Console.WriteLine("\t" + name);
            }
            Console.WriteLine("Photo Size Limit: " + cfg.PhotoSizeLimit);
            Console.WriteLine("Max Media Per Upload: " + cfg.MaxMediaPerUpload);
            Console.WriteLine("Characters Reserved Per Media: " + cfg.CharactersReservedPerMedia);
            Console.WriteLine("Photo Sizes");
            foreach (var photo in cfg.PhotoSizes)
            {
                Console.WriteLine("\t" + photo.Type);
                Console.WriteLine("\t\t" + photo.Width);
                Console.WriteLine("\t\t" + photo.Height);
                Console.WriteLine("\t\t" + photo.Resize);
            }
        }

        /// <summary>
        /// shows how to perform a help test
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetHelpLanguages(TwitterContext twitterCtx)
        {
            var helpResult =
                (from test in twitterCtx.Help
                 where test.Type == HelpType.Languages
                 select test)
                .SingleOrDefault();

            foreach (var lang in helpResult.Languages)
            {
                Console.WriteLine("{0}({1}): {2}", lang.Name, lang.Code, lang.Status);
            }
        }

        static void GetHelpRateLimits(TwitterContext twitterCtx)
        {
            var helpResult =
                (from help in twitterCtx.Help
                 where help.Type == HelpType.RateLimits //&&
                       //help.Resources == "search,users"
                 select help)
                .SingleOrDefault();

            foreach (var category in helpResult.RateLimits)
            {
                Console.WriteLine("\nCategory: {0}", category.Key);

                foreach (var limit in category.Value)
                {
                    Console.WriteLine(
                        "\n  Resource: {0}\n    Remaining: {1}\n    Reset: {2}\n    Limit: {3}",
                        limit.Resource, limit.Remaining, limit.Reset, limit.Limit);
                }
            }
        }

        /// <summary>
        /// Intentionally exceeds rate limits so you can see and know how to handle the Twitter error.
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ExceedRateLimitDemo(TwitterContext twitterCtx)
        {
            try
            {
                while (true)
                {
                    var statusResponse =
                        (from status in twitterCtx.Status
                         where status.Type == StatusType.Home
                         select status)
                        .ToList();
                } 
            }
            catch (TwitterQueryException tqEx)
            {
                const int TooManyRequests = 429;

                var webEx = tqEx.InnerException as WebException;
                if (webEx != null && (int)((HttpWebResponse)webEx.Response).StatusCode == TooManyRequests)
                    Console.WriteLine("Rate Limit Exceeded: " + tqEx.ToString());
                else
                    Console.WriteLine("Some other exception occurred: " + tqEx.ToString());
            }
        }

    }
}
