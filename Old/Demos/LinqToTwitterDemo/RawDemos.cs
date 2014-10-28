using System;
using System.Collections.Generic;
using System.Linq;
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
            //AccountTotalsDemo(twitterCtx);
            //AccountSettingsDemo(twitterCtx);
            //CategoryStatusDemo(twitterCtx);
            //RetweetedToUserDemo(twitterCtx);
            SearchDemo(twitterCtx);
            //UpdateStatusDemo(twitterCtx);
            //CreateFavoriteDemo(twitterCtx);
            //RelatedResultsDemo(twitterCtx);
        }

        /// <summary>
        /// Gets account totals
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void AccountTotalsDemo(TwitterContext twitterCtx)
        {
            var rawResult =
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == "account/totals.json"
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
                 where raw.QueryString == "account/settings.json"
                 select raw.Result)
                .FirstOrDefault();

            Console.WriteLine("Response from Twitter: \n\n" + rawResult);
        }

        /// <summary>
        /// Gets tweets of users in a suggested category
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void CategoryStatusDemo(TwitterContext twitterCtx)
        {
            var rawResult =
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == "users/suggestions/technology/members.json"
                 select raw.Result)
                .FirstOrDefault();

            Console.WriteLine("Response from Twitter: \n\n" + rawResult);
        }

        /// <summary>
        /// Gets tweets retweeted by a user
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void RetweetedToUserDemo(TwitterContext twitterCtx)
        {
            var rawResult =
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == "statuses/retweeted_to_user.json?screen_name=twitterapi"
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
            string unencodedStatus = "LINQ to Twitter";
            string encodedStatus = Uri.EscapeDataString(unencodedStatus);
            string queryString = "search/tweets.json?q=" + encodedStatus;

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

            string queryString = "/statuses/update.json";

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

            string queryString = "/favorites/create/25786742388.json";

            string result = twitterCtx.ExecuteRaw(queryString, parameters);

            Console.WriteLine("Result from create favorite: \n\n" + result);
        }
    }
}
