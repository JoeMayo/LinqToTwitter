using System;
using System.Linq;
using System.Net;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows account demos
    /// </summary>
    public class AccountDemos
    {
        /// <summary>
        /// Run all account related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            //VerifyAccountCredentials(twitterCtx);
            ViewRateLimitStatus(twitterCtx);
            ViewRateLimitStatusProjection(twitterCtx);
            ViewRateLimitResponseHeadersDemo(twitterCtx);
            ViewAccountTotalsDemo(twitterCtx);
            ViewAccountSettingsDemo(twitterCtx);
            //EndSession(twitterCtx); // really shouldn't do this at all!
            //UpdateAccountColors(twitterCtx);
            //UpdateAccountImage(twitterCtx);
            //UpdateAccountImageCallback(twitterCtx);
            //UpdateAccountBackgroundImage(twitterCtx);
            //UpdateAccountBackgroundImageBytes(twitterCtx);
            //UpdateAccountBackgroundImageAndTileDemo(twitterCtx);
            //UpdateAccountBackgroundImageAndTileButDontUseDemo(twitterCtx);
            //UpdateAccountBackgroundImageWithProgressUpdates(twitterCtx);
            //UpdateAccountInfoDemo(twitterCtx);
            //UpdateAccountSettingsDemo(twitterCtx);
        }

        /// <summary>
        /// Shows how to update account profile info
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountInfoDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountProfile(
                "LINQ to Tweeter",
                "http://linqtotwitter.codeplex.com",
                "Denver, CO",
                "Testing the Account Profile Update with LINQ to Twitter.");

            Console.WriteLine(
                "Name: {0}\nURL: {1}\nLocation: {2}\nDescription: {3}",
                user.Name, user.Url, user.Location, user.Description);
        }

        /// <summary>
        /// Shows how to update the background image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountBackgroundImage(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountBackgroundImage(@"..\..\images\200xColor_2.png", /*tile:*/ false, /*use:*/ true);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the background image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountBackgroundImageBytes(TwitterContext twitterCtx)
        {
            byte[] fileBytes = Utilities.GetFileBytes(@"..\..\images\200xColor_2.png");
            var user = twitterCtx.UpdateAccountBackgroundImage(fileBytes, "200xColor_2.png", "png", /*tile:*/ false, /*use:*/ true);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the background image in an account and tiles the image
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountBackgroundImageAndTileDemo(TwitterContext twitterCtx)
        {
            byte[] fileBytes = Utilities.GetFileBytes(@"..\..\images\200xColor_2.png");
            var user = twitterCtx.UpdateAccountBackgroundImage(fileBytes, "200xColor_2.png", "png", /*tile:*/ true, /*use:*/ true);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the background image in an account and tiles the image, but doesn't use the uploaded background
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountBackgroundImageAndTileButDontUseDemo(TwitterContext twitterCtx)
        {
            // TODO: Twitter doesn't let us upload and mark as not used
            byte[] fileBytes = Utilities.GetFileBytes(@"..\..\images\200xColor_2.png");
            var user = twitterCtx.UpdateAccountBackgroundImage(fileBytes, "200xColor_2.png", "png", /*tile:*/ true, /*use:*/ false);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the background image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountBackgroundImageWithProgressUpdates(TwitterContext twitterCtx)
        {
            twitterCtx.UploadProgressChanged +=
                (sender, e) =>
                {
                    Console.WriteLine("Progress: {0}%", e.PercentComplete);
                };
            byte[] fileBytes = Utilities.GetFileBytes(@"..\..\images\200xColor_2.png");
            var user = twitterCtx.UpdateAccountBackgroundImage(fileBytes, "200xColor_2.png", "png", /*tile:*/ false, /*use:*/ true);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountImage(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountImage(@"..\..\images\200xColor_2.png");

            Console.WriteLine("User Image: " + user.ProfileImageUrl);
        }

        /// <summary>
        /// Shows how to asynchronously update the image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountImageCallback(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountImage(
                @"..\..\images\200xColor_2.png",
                response =>
                {
                    Console.WriteLine("User Image: " + response.Status.ToString());
                });
        }

        /// <summary>
        /// Shows how to update Twitter colors
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void UpdateAccountColors(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountColors("9ae4e8", "#000000", "#0000ff", "#e0ff92", "#87bc44");

            Console.WriteLine("\nAccount Colors:\n");

            Console.WriteLine("Background:     " + user.ProfileBackgroundColor);
            Console.WriteLine("Text:           " + user.ProfileTextColor);
            Console.WriteLine("Link:           " + user.ProfileLinkColor);
            Console.WriteLine("Sidebar Fill:   " + user.ProfileSidebarFillColor);
            Console.WriteLine("Sidebar Border: " + user.ProfileSidebarBorderColor);
        }

        /// <summary>
        /// Shows how to end the session for the current account
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void EndSession(TwitterContext twitterCtx)
        {
            var endSessionStatus = twitterCtx.EndAccountSession();

            Console.WriteLine(
                "Request: {0}, Error: {1}",
                endSessionStatus.Request,
                endSessionStatus.Error);
        }

        private static void ViewAccountSettingsDemo(TwitterContext twitterCtx)
        {
            var accountSettings =
                (from acct in twitterCtx.Account
                 where acct.Type == AccountType.Settings
                 select acct.Settings)
                .SingleOrDefault();

            Console.WriteLine(
                "Trend Location: {0}\nGeo Enabled: {1}\nSleep Enabled: {2}",
                accountSettings.TrendLocation.Name,
                accountSettings.GeoEnabled,
                accountSettings.SleepTime.Enabled);
        }

        private static void ViewAccountTotalsDemo(TwitterContext twitterCtx)
        {
            var accountTotals =
                (from acct in twitterCtx.Account
                 where acct.Type == AccountType.Totals
                 select acct.Totals)
                .SingleOrDefault();

            Console.WriteLine(
                "Updates: {0}\nFriends: {1}\nFollowers: {2}\nFavorites: {3}",
                accountTotals.Updates,
                accountTotals.Friends,
                accountTotals.Followers,
                accountTotals.Favorites);
        }

        /// <summary>
        /// Shows how to extract rate limit info from response headers
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ViewRateLimitResponseHeadersDemo(TwitterContext twitterCtx)
        {
            var myMentions =
                from mention in twitterCtx.Status
                where mention.Type == StatusType.Mentions
                select mention;

            Console.WriteLine("\nAll rate limit results are either -1 or from the last query because this query hasn't executed yet. Look at results for this query *after* the query: \n");

            Console.WriteLine("Current Rate Limit: {0}", twitterCtx.RateLimitCurrent);
            Console.WriteLine("Remaining Rate Limit: {0}", twitterCtx.RateLimitRemaining);
            Console.WriteLine("Rate Limit Reset: {0}", twitterCtx.RateLimitReset);

            myMentions.ToList().ForEach(
                mention => Console.WriteLine(
                    "Name: {0}, Tweet: {1}\n",
                    mention.User.Name, mention.Text));

            Console.WriteLine("\nRate Limits from Query Response: \n");

            Console.WriteLine("Current Rate Limit: {0}", twitterCtx.RateLimitCurrent);
            Console.WriteLine("Remaining Rate Limit: {0}", twitterCtx.RateLimitRemaining);
            Console.WriteLine("Rate Limit Reset: {0}", twitterCtx.RateLimitReset);

            var resetTime =
                new DateTime(1970, 1, 1)
                .AddSeconds(twitterCtx.RateLimitReset)
                .ToLocalTime();

            Console.WriteLine("Rate Limit Reset in current time: {0}", resetTime);
        }

        /// <summary>
        /// Shows how to query an account's rate limit status info
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ViewRateLimitStatus(TwitterContext twitterCtx)
        {
            var accounts =
                from acct in twitterCtx.Account
                where acct.Type == AccountType.RateLimitStatus
                select acct;

            foreach (var account in accounts)
            {
                Console.WriteLine("\nRate Limit Status: \n");
                Console.WriteLine("Remaining Hits: {0}", account.RateLimitStatus.RemainingHits);
                Console.WriteLine("Hourly Limit: {0}", account.RateLimitStatus.HourlyLimit);
                Console.WriteLine("Reset Time: {0}", account.RateLimitStatus.ResetTime);
                Console.WriteLine("Reset Time in Seconds: {0}", account.RateLimitStatus.ResetTimeInSeconds);
            }
        }

        /// <summary>
        /// Shows how to query an account's rate limit status info with a custom projection
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ViewRateLimitStatusProjection(TwitterContext twitterCtx)
        {
            var rateLimitStatus =
                (from acct in twitterCtx.Account
                 where acct.Type == AccountType.RateLimitStatus
                 select acct.RateLimitStatus)
                .SingleOrDefault();

            Console.WriteLine("\nRate Limit Status: \n");
            Console.WriteLine("Remaining Hits: {0}", rateLimitStatus.RemainingHits);
            Console.WriteLine("Hourly Limit: {0}", rateLimitStatus.HourlyLimit);
            Console.WriteLine("Reset Time: {0}", rateLimitStatus.ResetTime);
            Console.WriteLine("Reset Time in Seconds: {0}", rateLimitStatus.ResetTimeInSeconds);
        }

        /// <summary>
        /// verifies that account credentials are correct
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void VerifyAccountCredentials(TwitterContext twitterCtx)
        {
            var accounts =
                from acct in twitterCtx.Account
                where acct.Type == AccountType.VerifyCredentials
                select acct;

            try
            {
                Account account = accounts.SingleOrDefault();
                User user = account.User;
                Status tweet = user.Status ?? new Status();
                Console.WriteLine("User (#" + user.Identifier.ID
                                    + "): " + user.Identifier.ScreenName
                                    + "\nTweet: " + tweet.Text
                                    + "\nTweet ID: " + tweet.StatusID + "\n");

                Console.WriteLine("Account credentials are verified.");
            }
            catch (WebException wex)
            {
                Console.WriteLine("Twitter did not recognize the credentials. Response from Twitter: " + wex.Message);
            }
        }

        private static void UpdateAccountSettingsDemo(TwitterContext twitterCtx)
        {
            Account acct = twitterCtx.UpdateAccountSettings(null, true, 20, 6, null, null);

            SleepTime sleep = acct.Settings.SleepTime;
            Console.WriteLine(
                "Enabled: {0}, Start: {1}, End: {2}",
                sleep.Enabled, sleep.StartHour, sleep.EndHour);
        }
    }
}
