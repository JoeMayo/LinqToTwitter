using LinqToTwitter;
using System;
using System.Linq;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows related results demos
    /// </summary>
    public class RelatedResultsDemos
    {
        /// <summary>
        /// Run all related results demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            ShowRelatedResultsDemo(twitterCtx);
        }

        /// <summary>
        /// shows how to get related results of a tweet
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ShowRelatedResultsDemo(TwitterContext twitterCtx)
        {
            var results =
                (from result in twitterCtx.RelatedResults
                 where result.Type == RelatedResultsType.Show &&
                       result.StatusID == 195992821411495936ul
                 select result)
                .ToList();

            results.ForEach(result => 
                Console.WriteLine("Name: {0}\nTweet: {1}\n", result.User.Identifier.ScreenName, result.Text));
        }
    }
}
