using LinqToTwitter;
using System;

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
            PerformHelpTest(twitterCtx);
        }

        #region Help Demos

        /// <summary>
        /// shows how to perform a help test
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void PerformHelpTest(TwitterContext twitterCtx)
        {
            var helpResult = twitterCtx.HelpTest();

            Console.WriteLine("Test Result: " + helpResult);
        }

        #endregion
    }
}
