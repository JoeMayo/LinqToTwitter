using System;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows report spam demos
    /// </summary>
    public class ReportSpamDemos
    {
        /// <summary>
        /// Run all report spam related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            //ReportSpamDemo(twitterCtx);
        }

        #region Report Spam Demos

        /// <summary>
        /// Shows multiple ways to report spammers
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ReportSpamDemo(TwitterContext twitterCtx)
        {
            var spammer = twitterCtx.ReportSpam(null, null, "Greer_105");
            Console.WriteLine("Spammer \"{0}\" Zapped! He he :)", spammer.Name);

            // after the first one, subsequent calls won't report spam to Twitter
            // but hopefully you can see my enthusiasm for this API;
            // besides, a couple extra examples might be helpful - Joe

            spammer = twitterCtx.ReportSpam("84705854", null, null);
            Console.WriteLine("Spammer \"{0}\" Zapped again! Ha Ha :)", spammer.Name);

            spammer = twitterCtx.ReportSpam(null, "84705854", null);
            Console.WriteLine("Spammer \"{0}\" is so gone! ... and don't come back! :)", spammer.Name);
        }

        #endregion
    }
}
