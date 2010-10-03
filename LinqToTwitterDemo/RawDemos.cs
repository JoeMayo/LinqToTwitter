using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    public class RawDemos
    {
        /// <summary>
        /// Run all raw query demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            //PublicTimelineDemo(twitterCtx);
            AccountTotalsDemo(twitterCtx);
            //AccountSettingsDemo(twitterCtx);
            //RetweetedToUserDemo(twitterCtx);
            //SearchDemo(twitterCtx);
            //UpdateStatusDemo(twitterCtx);
            //CreateFavoriteDemo(twitterCtx);
        }

        #region Raw Demos

        /// <summary>
        /// Requests the public timeline
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void PublicTimelineDemo(TwitterContext twitterCtx)
        {
            var rawResult =
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == "statuses/public_timeline.xml"
                 select raw)
                .FirstOrDefault();

            Console.WriteLine("Response from Twitter: \n\n" + rawResult.Result);
        }

        /// <summary>
        /// Gets account totals
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void AccountTotalsDemo(TwitterContext twitterCtx)
        {
            var rawResult =
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == "account/totals.xml"
                 select raw)
                .FirstOrDefault();

            Console.WriteLine("Response from Twitter: \n\n" + rawResult.Result);
        }

        /// <summary>
        /// Gets account settings
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void AccountSettingsDemo(TwitterContext twitterCtx)
        {
            var rawResult =
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == "account/settings.xml"
                 select raw)
                .FirstOrDefault();

            Console.WriteLine("Response from Twitter: \n\n" + rawResult.Result);
        }

        /// <summary>
        /// Gets tweets retweeted by a user
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void RetweetedToUserDemo(TwitterContext twitterCtx)
        {
            var rawResult =
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == "statuses/retweeted_to_user.xml?screen_name=twitterapi"
                 select raw)
                .FirstOrDefault();

            Console.WriteLine("Response from Twitter: \n\n" + rawResult.Result);
        }

        /// <summary>
        /// Shows how to encode a parameter
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchDemo(TwitterContext twitterCtx)
        {
            twitterCtx.BaseUrl = "http://search.twitter.com";
            string unencodedStatus = "LINQ to Twitter";
            string encodedStatus = Uri.EscapeDataString(unencodedStatus);
            string queryString = "search.atom?q=" + encodedStatus;

            var rawResult =
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == queryString
                 select raw)
                .FirstOrDefault();

            Console.WriteLine("Response from Twitter: \n\n" + rawResult.Result);
        }

        /// <summary>
        /// Perform update status side-effect with raw data
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusDemo(TwitterContext twitterCtx)
        {
            string status = "Testing LINQ to Twitter Raw Interface: " + DateTime.Now.ToString();
            var parameters = new Dictionary<string, string>
            {
                { "status", status }
            };

            string queryString = "/statuses/update.xml";

            string result = twitterCtx.ExecuteRaw(queryString, parameters);

            Console.WriteLine("Result from update status: \n\n" + result);
        }

        /// <summary>
        /// Perform create favorite side-effect with raw data
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void CreateFavoriteDemo(TwitterContext twitterCtx)
        {
            string status = "Testing LINQ to Twitter Raw Interface: " + DateTime.Now.ToString();
            var parameters = new Dictionary<string, string>();

            string queryString = "/favorites/create/25786742388.xml";

            string result = twitterCtx.ExecuteRaw(queryString, parameters);

            Console.WriteLine("Result from create favorite: \n\n" + result);
        }

        #endregion
    }
}
