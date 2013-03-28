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
            VerifyAccountCredentials(twitterCtx);
            ViewRateLimitResponseHeadersDemo(twitterCtx);
            ViewAccountSettingsDemo(twitterCtx);
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
            //UpdateProfileBanner(twitterCtx);
            //RemoveProfileBanner(twitterCtx);
        }

        /// <summary>
        /// Shows how to update account profile info
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateAccountInfoDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountProfile("LINQ to Tweeter",
                "http://linqtotwitter.codeplex.com",
                "Denver, CO",
                "Testing the Account Profile Update with LINQ to Twitter.",
                true,
                true);

            Console.WriteLine(
                "Name: {0}\nURL: {1}\nLocation: {2}\nDescription: {3}",
                user.Name, user.Url, user.Location, user.Description);
        }

        /// <summary>
        /// Shows how to update the background image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateAccountBackgroundImage(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountBackgroundImage(@"..\..\images\200xColor_2.png", false, true, true, true);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the background image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateAccountBackgroundImageBytes(TwitterContext twitterCtx)
        {
            byte[] fileBytes = Utilities.GetFileBytes(@"..\..\images\200xColor_2.png");
            var user = twitterCtx.UpdateAccountBackgroundImage(fileBytes, "200xColor_2.png", "png", false, true, true, true);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the background image in an account and tiles the image
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateAccountBackgroundImageAndTileDemo(TwitterContext twitterCtx)
        {
            byte[] fileBytes = Utilities.GetFileBytes(@"..\..\images\200xColor_2.png");
            var user = twitterCtx.UpdateAccountBackgroundImage(fileBytes, "200xColor_2.png", "png", true, true, true, true);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the background image in an account and tiles the image, but doesn't use the uploaded background
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateAccountBackgroundImageAndTileButDontUseDemo(TwitterContext twitterCtx)
        {
            byte[] fileBytes = Utilities.GetFileBytes(@"..\..\images\200xColor_2.png");
            var user = twitterCtx.UpdateAccountBackgroundImage(fileBytes, "200xColor_2.png", "png", true, false, true, true);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the profile image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateProfileBanner(TwitterContext twitterCtx)
        {
            byte[] fileBytes = Utilities.GetFileBytes(@"..\..\images\WP_000003.jpg");
            var user = twitterCtx.UpdateProfileBanner(fileBytes, "WP_000003.jpg", "jpg", 1252, 626, 0, 0);

            Console.WriteLine("User Image: " + user.ProfileBannerUrl);
        }

        /// <summary>
        /// Shows how to remove the profile image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void RemoveProfileBanner(TwitterContext twitterCtx)
        {
            var user = twitterCtx.RemoveProfileBanner();
            Console.WriteLine("Profile Banner: " + user.ProfileBannerUrl ?? "None");
        }

        /// <summary>
        /// Shows how to update the background image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateAccountBackgroundImageWithProgressUpdates(TwitterContext twitterCtx)
        {
            twitterCtx.UploadProgressChanged +=
                (sender, e) =>
                {
                    Console.WriteLine("Progress: {0}%", e.PercentComplete);
                };
            byte[] fileBytes = Utilities.GetFileBytes(@"..\..\images\200xColor_2.png");
            var user = twitterCtx.UpdateAccountBackgroundImage(fileBytes, "200xColor_2.png", "png", false, true, true, true);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateAccountImage(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountImage(@"..\..\images\200xColor_2.png", true);

            Console.WriteLine("User Image: " + user.ProfileImageUrl);
        }

        /// <summary>
        /// Shows how to asynchronously update the image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateAccountImageCallback(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountImage(
                @"..\..\images\200xColor_2.png", true,
                response =>
                {
                    Console.WriteLine("User Image: " + response.Status.ToString());
                });
        }

        /// <summary>
        /// Shows how to update Twitter colors
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateAccountColors(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountColors("9ae4e8", "#000000", "#0000ff", "#e0ff92", "#87bc44", true, true);

            Console.WriteLine("\nAccount Colors:\n");

            Console.WriteLine("Background:     " + user.ProfileBackgroundColor);
            Console.WriteLine("Text:           " + user.ProfileTextColor);
            Console.WriteLine("Link:           " + user.ProfileLinkColor);
            Console.WriteLine("Sidebar Fill:   " + user.ProfileSidebarFillColor);
            Console.WriteLine("Sidebar Border: " + user.ProfileSidebarBorderColor);
        }

        /// <summary>
        /// Shows how to obtain account settings.
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void ViewAccountSettingsDemo(TwitterContext twitterCtx)
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

        /// <summary>
        /// Shows how to extract rate limit info from response headers
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void ViewRateLimitResponseHeadersDemo(TwitterContext twitterCtx)
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
        /// verifies that account credentials are correct
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void VerifyAccountCredentials(TwitterContext twitterCtx)
        {
            //var accounts =
            //    from acct in twitterCtx.Account
            //    where acct.Type == AccountType.VerifyCredentials
            //    select acct;

            try
            {
                //Account account = accounts.SingleOrDefault();
                Account account = twitterCtx.Account.Single(acct => acct.Type == AccountType.VerifyCredentials);
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

        /// <summary>
        /// Shows how to update account settings.
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void UpdateAccountSettingsDemo(TwitterContext twitterCtx)
        {
            Account acct = twitterCtx.UpdateAccountSettings(null, true, 20, 6, null, null);

            SleepTime sleep = acct.Settings.SleepTime;
            Console.WriteLine(
                "Enabled: {0}, Start: {1}, End: {2}",
                sleep.Enabled, sleep.StartHour, sleep.EndHour);
        }
    }
}
