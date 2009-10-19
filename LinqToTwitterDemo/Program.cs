using System;
using System.Linq;

using LinqToTwitter;
using System.Net;
using System.Diagnostics;
using System.Web;
using System.Collections.Specialized;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Configuration;

namespace LinqToTwitterDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // For testing globalization, uncomment and change 
            // locale to a locale that is not yours
            //Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-PT");

            //
            // get user credentials and instantiate TwitterContext
            //
            ITwitterAuthorization auth;

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["twitterConsumerKey"]) || string.IsNullOrEmpty(ConfigurationManager.AppSettings["twitterConsumerSecret"]))
            {
                Console.WriteLine("Skipping OAuth authorization demo because twitterConsumerKey and/or twitterConsumerSecret are not set in your .config file.");
                Console.WriteLine("Using username/password authorization instead.");

                // For username/password authorization demo...
                auth = new UsernamePasswordAuthorization(Utilities.GetConsoleHWnd());
            }
            else
            {
                Console.WriteLine("Discovered Twitter OAuth consumer key in .config file.  Using OAuth authorization.");

                // For OAuth authorization demo...
                auth = new DesktopOAuthAuthorization();
            }

            // TwitterContext is similar to DataContext (LINQ to SQL) or ObjectContext (LINQ to Entities)

            // For Twitter
            using (var twitterCtx = new TwitterContext(auth, "https://twitter.com/", "http://search.twitter.com/"))
            {

                // For JTweeter (Laconica)
                //var twitterCtx = new TwitterContext(passwordAuth, "http://jtweeter.com/api/", "http://search.twitter.com/");

                // For Identi.ca (Laconica)
                //var twitterCtx = new TwitterContext(passwordAuth, "http://identi.ca/api/", "http://search.twitter.com/");

                // If we're using OAuth, we need to configure it with the ConsumerKey etc. from the user.
                if (twitterCtx.AuthorizedClient is OAuthAuthorization)
                {
                    InitializeOAuthConsumerStrings(twitterCtx);
                }

                // Whatever authorization module we selected... sign on now.  
                // See the bottom of the method for sign-off procedures.
                auth.SignOn();

                //
                // status tweets
                //

                UpdateStatusDemo(twitterCtx);
                //SingleStatusQueryDemo(twitterCtx);
                //UpdateStatusWithReplyDemo(twitterCtx);
                //DestroyStatusDemo(twitterCtx);
                //UserStatusByNameQueryDemo(twitterCtx);
                //UserStatusQueryDemo(twitterCtx);
                //FirstStatusQueryDemo(twitterCtx);
                //PublicStatusQueryDemo(twitterCtx);
                //PublicStatusFilteredQueryDemo(twitterCtx);
                //MentionsStatusQueryDemo(twitterCtx);
                //FriendStatusQueryDemo(twitterCtx);

                //
                // user tweets
                //

                //UserShowWithIDQueryDemo(twitterCtx);
                //UserShowWithScreenNameQueryDemo(twitterCtx);
                //UserFriendsQueryDemo(twitterCtx);
                //UserFollowersQueryDemo(twitterCtx);
                //GetAllFollowersQueryDemo(twitterCtx);

                //
                // direct messages
                //

                //DirectMessageSentByQueryDemo(twitterCtx);
                //DirectMessageSentToQueryDemo(twitterCtx);
                //NewDirectMessageDemo(twitterCtx);
                //DestroyDirectMessageDemo(twitterCtx);

                //
                // friendship
                //

                //CreateFriendshipFollowDemo(twitterCtx);
                //FriendshipExistsDemo(twitterCtx);
                //DestroyFriendshipDemo(twitterCtx);
                //CreateFriendshipNoDeviceUpdatesDemo(twitterCtx);

                //
                // SocialGraph
                //

                //ShowFriendsDemo(twitterCtx);
                //ShowFollowersDemo(twitterCtx);

                //
                // Search
                //

                //SearchTwitterDemo(twitterCtx);
                //SearchTwitterSource(twitterCtx);
                //ExceedSearchRateLimitDemo(twitterCtx);

                //
                // Favorites
                //

                //FavoritesQueryDemo(twitterCtx);
                //CreateFavoriteDemo(twitterCtx);
                //DestroyFavoriteDemo(twitterCtx);

                //
                // Notifications
                //

                //EnableNotificationsDemo(twitterCtx);
                //EnableNotificationsWithScreenNameDemo(twitterCtx);
                //EnableNotificationsWithUserIDDemo(twitterCtx);
                //DisableNotificationsDemo(twitterCtx);

                //
                // Blocks
                //

                //CreateBlock(twitterCtx);
                //DestroyBlock(twitterCtx);
                //BlockExistsDemo(twitterCtx);
                //BlockIDsDemo(twitterCtx);
                //BlockBlockingDemo(twitterCtx);

                //
                // Help
                //

                //PerformHelpTest(twitterCtx);

                //
                // Account
                //

                //VerifyAccountCredentials(twitterCtx);
                //ViewRateLimitStatus(twitterCtx);
                //ViewRateLimitResponseHeadersDemo(twitterCtx);
                //EndSession(twitterCtx);
                //UpdateDeliveryDevice(twitterCtx);
                //UpdateAccountColors(twitterCtx);
                //UpdateAccountImage(twitterCtx);
                //UpdateAccountBackgroundImage(twitterCtx);
                //UpdateAccountBackgroundImageAndTileDemo(twitterCtx);
                //UpdateAccountInfoDemo(twitterCtx);

                //
                // Trends
                //

                //SearchTrendsDemo(twitterCtx);
                //SearchCurrentTrendsDemo(twitterCtx);
                //SearchDailyTrendsDemo(twitterCtx);
                //SearchWeeklyTrendsDemo(twitterCtx);

                //
                // Error Handling Demos
                //

                //HandleQueryExceptionDemo(twitterCtx);
                //HandleSideEffectExceptionDemo(twitterCtx);
                //HandleSideEffectWithFilePostExceptionDemo(twitterCtx);
                //HandleTimeoutErrors(twitterCtx);

                //
                // Oauth Demos
                //

                //HandleOAuthQueryDemo(twitterCtx);
                //HandleOAuthSideEffectDemo(twitterCtx);
                //HandleOAuthFilePostDemo(twitterCtx);
                //HandleOAuthReadOnlyQueryDemo(twitterCtx);
                //HandleOAuthSideEffectReadOnlyDemo(twitterCtx);
                //HandleOAuthRequestResponseDetailsDemo(twitterCtx);
                //OAuthForceLoginDemo(twitterCtx);

                //
                // Saved Search Demos
                //

                //QuerySavedSearchesDemo(twitterCtx);
                //QuerySavedSearchesShowDemo(twitterCtx);
                //CreateSavedSearchDemo(twitterCtx);
                //DestroySavedSearchDemo(twitterCtx);

                //
                // Sign-off, including optional clearing of cached credentials.
                //

                //auth.SignOff();
                //auth.ClearCachedCredentials();
            }

            Console.WriteLine("Press any key to end this demo.");
            Console.ReadKey();
        }

        #region Saved Search Demos

        /// <summary>
        /// Shows how to delete a saved search
        /// </summary>
        /// <remarks>
        /// Trying to delete a saved search that doesn't exist results
        /// in a TwitterQueryException with HTTP Status 404 (Not Found)
        /// </remarks>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DestroySavedSearchDemo(TwitterContext twitterCtx)
        {
            var savedSearch = twitterCtx.DestroySavedSearch(329820);

            Console.WriteLine("ID: {0}, Search: {1}", savedSearch.ID, savedSearch.Name);
        }

        /// <summary>
        /// shows how to create a Saved Search
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void CreateSavedSearchDemo(TwitterContext twitterCtx)
        {
            var savedSearch = twitterCtx.CreateSavedSearch("#csharp");

            Console.WriteLine("ID: {0}, Search: {1}", savedSearch.ID, savedSearch.Name);
        }

        /// <summary>
        /// shows how to retrieve a single search
        /// </summary>
        /// <remarks>
        /// Trying to delete a saved search that doesn't exist results
        /// in a TwitterQueryException with HTTP Status 404 (Not Found)
        /// </remarks>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void QuerySavedSearchesShowDemo(TwitterContext twitterCtx)
        {
            var savedSearches =
                from search in twitterCtx.SavedSearch
                where search.Type == SavedSearchType.Show &&
                      search.ID == "176136"
                select search;

            var savedSearch = savedSearches.FirstOrDefault();

            Console.WriteLine("ID: {0}, Search: {1}", savedSearch.ID, savedSearch.Name);
        }

        /// <summary>
        /// shows how to retrieve all searches
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void QuerySavedSearchesDemo(TwitterContext twitterCtx)
        {
            var savedSearches =
                from search in twitterCtx.SavedSearch
                where search.Type == SavedSearchType.Searches
                select search;

            foreach (var search in savedSearches)
            {
                Console.WriteLine("ID: {0}, Search: {1}", search.ID, search.Name);
            }
        }

        #endregion

        #region OAuth Demos

        private static void InitializeOAuthConsumerStrings(TwitterContext twitterCtx)
        {
            var oauth = (DesktopOAuthAuthorization)twitterCtx.AuthorizedClient;
            oauth.GetVerifier = () =>
            {
                Console.WriteLine("Next, you'll need to tell Twitter to authorize access.\nThis program will not have access to your credentials, which is the benefit of OAuth.\nOnce you log into Twitter and give this program permission,\n come back to this console.");
                Console.Write("Please enter the PIN that Twitter gives you after authorizing this client: ");
                return Console.ReadLine();
            };



            if (oauth.CachedCredentialsAvailable)
            {
                Console.WriteLine("Skipping OAuth authorization step because that has already been done.");
            }
        }

        /// <summary>
        /// Shows how to force user to log in
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void OAuthForceLoginDemo(TwitterContext twitterCtx)
        {
            if (twitterCtx.AuthorizedClient.IsAuthorized)
            {
                var tweets =
                    from tweet in twitterCtx.Status
                    where tweet.Type == StatusType.Friends
                    select tweet;

                tweets.ToList().ForEach(
                    tweet => Console.WriteLine(
                        "Friend: {0}\nTweet: {1}\n",
                        tweet.User.Name,
                        tweet.Text));
            }
        }

        /// <summary>
        /// shows how to retrieve the screen name and user ID from an OAuth request
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleOAuthRequestResponseDetailsDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine();
            Console.WriteLine(
                "Screen Name: {0}, User ID: {1}",
                twitterCtx.AuthorizedClient.ScreenName,
                twitterCtx.AuthorizedClient.UserId);
        }

        /// <summary>
        /// shows what happens when performing a side-effect when ReadOnly is turned on
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleOAuthSideEffectReadOnlyDemo(TwitterContext twitterCtx)
        {
            if (twitterCtx.AuthorizedClient.IsAuthorized)
            {
                var status = twitterCtx.UpdateStatus("I used LINQ to Twitter with OAuth: " + DateTime.Now.ToString());

                Console.WriteLine(
                    "Friend: {0}\nTweet: {1}\n",
                    status.User.Name,
                    status.Text);
            }
        }

        /// <summary>
        /// shows how to restrict access to read-only while performing a query
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleOAuthReadOnlyQueryDemo(TwitterContext twitterCtx)
        {
            if (twitterCtx.AuthorizedClient.IsAuthorized)
            {
                var accounts =
                    from acct in twitterCtx.Account
                    where acct.Type == AccountType.VerifyCredentials
                    select acct;

                foreach (var account in accounts)
                {
                    Console.WriteLine("Credentials for account, {0}, are okay.", account.User.Name);
                }
            }
        }

        /// <summary>
        /// hows how to use OAuth to post a file to Twitter
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleOAuthFilePostDemo(TwitterContext twitterCtx)
        {
            if (twitterCtx.AuthorizedClient.IsAuthorized)
            {
                var user = twitterCtx.UpdateAccountBackgroundImage(
                    @"C:\Users\jmayo\Documents\linq2twitter\linq2twitter\200xColor_2.png", false);

                Console.WriteLine(
                    "Name: {0}\nImage: {1}\n",
                    user.Name,
                    user.ProfileBackgroundImageUrl);
            }
        }

        /// <summary>
        /// Perform an update using OAuth
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleOAuthSideEffectDemo(TwitterContext twitterCtx)
        {
            if (twitterCtx.AuthorizedClient.IsAuthorized)
            {
                var status = twitterCtx.UpdateStatus("I used LINQ to Twitter with OAuth: " + DateTime.Now.ToString());

                Console.WriteLine(
                    "Friend: {0}\nTweet: {1}\n",
                    status.User.Name,
                    status.Text);
            }
        }

        /// <summary>
        /// perform a query using OAuth
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleOAuthQueryDemo(TwitterContext twitterCtx)
        {
            if (twitterCtx.AuthorizedClient.IsAuthorized)
            {
                var tweets =
                    from tweet in twitterCtx.Status
                    where tweet.Type == StatusType.Friends
                    select tweet;

                tweets.ToList().ForEach(
                    tweet => Console.WriteLine(
                        "Friend: {0}, Created: {1}\nTweet: {2}\n",
                        tweet.User.Name,
                        tweet.CreatedAt,
                        tweet.Text));
            }
        }

        #endregion

        #region Error Handling Demos

        /// <summary>
        /// shows how to handle a timeout error
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleTimeoutErrors(TwitterContext twitterCtx)
        {
            // force an unreasonable timeout (1 millisecond)
            twitterCtx.Timeout = 1;

            var publicTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            try
            {
                publicTweets.ToList().ForEach(
                        tweet => Console.WriteLine(
                            "User Name: {0}, Tweet: {1}",
                            tweet.User.Name,
                            tweet.Text));
            }
            catch (TwitterQueryException tqEx)
            {
                // use your logging and handling logic here

                // notice how the WebException is wrapped as the
                // inner exception of the TwitterQueryException
                Console.WriteLine(tqEx.InnerException.Message);
            }
        }

        /// <summary>
        /// shows how to handle a TwitterQueryException with a side-effect causing a file post
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleSideEffectWithFilePostExceptionDemo(TwitterContext twitterCtx)
        {
            // force the error by supplying bad credentials
            twitterCtx.AuthorizedClient = new UsernamePasswordAuthorization
            {
                UserName = "BadUserName",
                Password = "BadPassword",
            };

            try
            {
                var user = twitterCtx.UpdateAccountImage(@"C:\Users\jmayo\Pictures\JoeTwitter.jpg");
            }
            catch (TwitterQueryException tqe)
            {
                // log it to the console
                Console.WriteLine(
                    "\nHTTP Error Code: {0}\nError: {1}\nRequest: {2}\n",
                    tqe.HttpError,
                    tqe.Response.Error,
                    tqe.Response.Request);
            }
        }

        /// <summary>
        /// shows how to handle a TwitterQueryException with a side-effect
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleSideEffectExceptionDemo(TwitterContext twitterCtx)
        {
            // force the error by supplying bad credentials
            twitterCtx.AuthorizedClient = new UsernamePasswordAuthorization
            {
                UserName = "BadUserName",
                Password = "BadPassword",
            };

            try
            {
                var status = twitterCtx.UpdateStatus("Test from LINQ to Twitter - 5/2/09");
            }
            catch (TwitterQueryException tqe)
            {
                // log it to the console
                Console.WriteLine(
                    "\nHTTP Error Code: {0}\nError: {1}\nRequest: {2}\n",
                    tqe.HttpError,
                    tqe.Response.Error,
                    tqe.Response.Request);
            }
        }

        /// <summary>
        /// shows how to handle a TwitterQueryException with a query
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleQueryExceptionDemo(TwitterContext twitterCtx)
        {
            // force the error by supplying bad credentials
            twitterCtx.AuthorizedClient = new UsernamePasswordAuthorization
            {
                UserName = "BadUserName",
                Password = "BadPassword",
            };

            try
            {
                var statuses =
                        from status in twitterCtx.Status
                        where status.Type == StatusType.Mentions
                        select status;

                var statusList = statuses.ToList();
            }
            catch (TwitterQueryException tqe)
            {
                // log it to the console
                Console.WriteLine(
                    "\nHTTP Error Code: {0}\nError: {1}\nRequest: {2}\n",
                    tqe.HttpError,
                    tqe.Response.Error,
                    tqe.Response.Request);
            }
        }

        #endregion

        #region Trends Demos

        /// <summary>
        /// shows how to request weekly trends
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchWeeklyTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                from trend in twitterCtx.Trends
                where trend.Type == TrendType.Weekly &&
                      trend.ExcludeHashtags == true &&
                      trend.Date == DateTime.Now.AddDays(-14)
                select trend;

            trends.ToList().ForEach(
                trend => Console.WriteLine(
                    "Name: {0}, Query: {1}, Date: {2}",
                    trend.Name, trend.Query, trend.AsOf));
        }

        /// <summary>
        /// shows how to request daily trends
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchDailyTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                from trend in twitterCtx.Trends
                where trend.Type == TrendType.Daily &&
                      trend.Date == DateTime.Now.AddDays(-2)
                select trend;

            trends.ToList().ForEach(
                trend => Console.WriteLine(
                    "Name: {0}, Query: {1}",
                    trend.Name, trend.Query));
        }

        /// <summary>
        /// shows how to request current trends
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchCurrentTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                from trend in twitterCtx.Trends
                where trend.Type == TrendType.Current &&
                      trend.ExcludeHashtags == true
                select trend;

            trends.ToList().ForEach(
                trend => Console.WriteLine(
                    "Name: {0}, Query: {1}",
                    trend.Name, trend.Query));
        }

        /// <summary>
        /// shows how to request trends
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                from trend in twitterCtx.Trends
                where trend.Type == TrendType.Trend
                select trend;

            trends.ToList().ForEach(
                trend => Console.WriteLine(
                    "Name: {0}, Query: {1}, Date: {2}",
                    trend.Name, trend.Query, trend.AsOf));
        }

        #endregion

        #region Account Demos

        /// <summary>
        /// Shows how to update account profile info
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountInfoDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountProfile(
                "LINQ to Tweeter Test",
                "Joe@LinqToTwitter.com",
                "http://linqtotwitter.codeplex.com",
                "Anywhere In The World",
                "Testing the LINQ to Twitter Account Profile Update.");

            Console.WriteLine(
                "Name: {0}\nURL: {2}\nLocation: {3}\nDescription: {4}",
                user.Name, user.URL, user.Location, user.Description);
        }

        /// <summary>
        /// Shows how to update the background image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountBackgroundImage(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountBackgroundImage(@"C:\Users\jmayo\Documents\linq2twitter\linq2twitter\linq2twitter_v3_300x90.png", false);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the background image in an account and tiles the image
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountBackgroundImageAndTileDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountBackgroundImage(@"C:\Users\jmayo\Documents\linq2twitter\linq2twitter\linq2twitter_v3_300x90.png", true);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountImage(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountImage(@"C:\Users\jmayo\Pictures\Sgt Peppers\JoeTwitterBW.jpg");

            Console.WriteLine("User Image: " + user.ProfileImageUrl);
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
        /// Shows how to update a device
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void UpdateDeliveryDevice(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountDeliveryDevice(DeviceType.None);

            Console.WriteLine("Device Type: {0}", user.Notifications.ToString());
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
                foreach (var account in accounts)
                {
                    Console.WriteLine("Credentials for account, {0}, are okay.", account.User.Name);
                }
            }
            catch (WebException wex)
            {
                Console.WriteLine("Twitter did not recognize the credentials. Response from Twitter: " + wex.Message);
            }
        }

        #endregion

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

        #region Block Demos

        /// <summary>
        /// shows how to unblock a user
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void DestroyBlock(TwitterContext twitterCtx)
        {
            var user = twitterCtx.DestroyBlock("JoeMayo");

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// Shows how to block a user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void CreateBlock(TwitterContext twitterCtx)
        {
            var user = twitterCtx.CreateBlock("JoeMayo");

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// shows how to get a list of users that are being blocked
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void BlockBlockingDemo(TwitterContext twitterCtx)
        {
            var result =
                from blockItem in twitterCtx.Blocks
                where blockItem.Type == BlockingType.Blocking
                select blockItem;

            result.ToList().ForEach(
                block =>
                    Console.WriteLine(
                        "User, {0} is blocked.",
                        block.User.Name));
        }

        /// <summary>
        /// shows how to get a list of IDs of the users being blocked
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void BlockIDsDemo(TwitterContext twitterCtx)
        {
            var result =
                from blockItem in twitterCtx.Blocks
                where blockItem.Type == BlockingType.IDS
                select blockItem;

            result.ToList().ForEach(
                block => Console.WriteLine("ID: {0}\n", block.ID));
        }

        /// <summary>
        /// shows how to see if a specific user is being blocked
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void BlockExistsDemo(TwitterContext twitterCtx)
        {
            try
            {
                var result =
                    from blockItem in twitterCtx.Blocks
                    where blockItem.Type == BlockingType.Exists &&
                          blockItem.ScreenName == "JoeMayo"
                    select blockItem;

                result.ToList().ForEach(
                    block =>
                        Console.WriteLine(
                            "User, {0} is blocked.",
                            block.User.Name));
            }
            catch (TwitterQueryException tqe)
            {
                Console.WriteLine("User not blocked. Twitter Response: " + tqe.Response.Error);
            }
        }

        #endregion

        #region Notifications Demos

        /// <summary>
        /// Shows how to do a Notifications Follow
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void EnableNotificationsDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.EnableNotifications("15411837", null, null);

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// Shows how to do a Notifications Follow
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void EnableNotificationsWithScreenNameDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.EnableNotifications(null, null, "JoeMayo");

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// Shows how to do a Notifications Follow
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void EnableNotificationsWithUserIDDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.EnableNotifications(null, "15411837", null);

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// Shows how to do a Notifications Leave
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DisableNotificationsDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.DisableNotifications("15411837", null, null);

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        #endregion

        #region Favorites Demos

        private static void DestroyFavoriteDemo(TwitterContext twitterCtx)
        {
            var status = twitterCtx.DestroyFavorite("1552797863");

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        /// <summary>
        /// Shows how to create a Favorite
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void CreateFavoriteDemo(TwitterContext twitterCtx)
        {
            var status = twitterCtx.CreateFavorite("1552797863");

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        /// <summary>
        /// shows how to request a favorites list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void FavoritesQueryDemo(TwitterContext twitterCtx)
        {
            var favorites =
                from fav in twitterCtx.Favorites
                where fav.Type == FavoritesType.Favorites
                select fav;

            favorites.ToList().ForEach(
                fav => Console.WriteLine(
                    "User Name: {0}, Tweet: {1}",
                    fav.User.Name, fav.Text));
        }

        #endregion

        #region Search Demos

        /// <summary>
        /// shows how to perform a twitter search
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTwitterDemo(TwitterContext twitterCtx)
        {
            
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "LINQ to Twitter" &&
                      search.Page == 2 &&
                      search.PageSize == 5
                select search;

            foreach (var search in queryResults)
            {
                // here, you can see that properties are named
                // from the perspective of atom feed elements
                // i.e. the query string is called Title
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Content);
                }
            }
        }

        /// <summary>
        /// Shows how to specify a source of tweets to search for
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTwitterSource(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "LINQ to Twitter source:web"
                select search;

            foreach (var search in queryResults)
            {
                // here, you can see that properties are named
                // from the perspective of atom feed elements
                // i.e. the query string is called Title
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Content);
                }
            }
        }

        ///// <summary>
        ///// shows how to handle response when you exceed Search query rate limits
        ///// </summary>
        ///// <param name="twitterCtx"></param>
        //private static void ExceedSearchRateLimitDemo(TwitterContext twitterCtx)
        //{
        //    //
        //    // WARNING: This is for Test/Demo purposes only; 
        //    //          it makes many queries to Twitter in
        //    //          a very short amount of time, which
        //    //          has a negative impact on the service.
        //    //
        //    //          The only reason it is here is to test
        //    //          that LINQ to Twitter responds appropriately
        //    //          to Search rate limits.
        //    //

        //    var queryResults =
        //        from search in twitterCtx.Search
        //        where search.Type == SearchType.Search &&
        //              search.Query == "Testing Search Rate Limit Results"
        //        select search;

        //    try
        //    {
        //        // set to a sufficiently high number to force the HTTP 503 response
        //        // -- assumes you have the bandwidth to exceed
        //        //    limit, which you might not have
        //        var searchesToPerform = 5;

        //        for (int i = 0; i < searchesToPerform; i++)
        //        {
        //            foreach (var search in queryResults)
        //            {
        //                // here, you can see that properties are named
        //                // from the perspective of atom feed elements
        //                // i.e. the query string is called Title
        //                Console.WriteLine("\n#{0}. Query:{1}\n", i+1, search.Title);

        //                foreach (var entry in search.Entries)
        //                {
        //                    Console.WriteLine(
        //                        "ID: {0}, Source: {1}\nContent: {2}\n",
        //                        entry.ID, entry.Source, entry.Content);
        //                }
        //            } 
        //        }
        //    }
        //    catch (TwitterQueryException tqe)
        //    {
        //        if (tqe.HttpError == "503")
        //        {
        //            Console.WriteLine("HTTP Error: {0}", tqe.HttpError);
        //            Console.WriteLine("You've exceeded rate limits for search.");
        //            Console.WriteLine("Please retry in {0} seconds.", twitterCtx.RetryAfter);
        //        }
        //    }

        //    Console.WriteLine("\nComplete.");
        //}

        #endregion

        #region Social Graph Demos

        /// <summary>
        /// Shows how to list followers
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ShowFollowersDemo(TwitterContext twitterCtx)
        {
            var followers =
                from follower in twitterCtx.SocialGraph
                where follower.Type == SocialGraphType.Followers &&
                      follower.ID == "15411837"
                select follower;

            followers.ToList().ForEach(
                follower => Console.WriteLine("Follower ID: " + follower.ID));
        }

        /// <summary>
        /// Shows how to list Friends
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ShowFriendsDemo(TwitterContext twitterCtx)
        {
            var friends =
                from friend in twitterCtx.SocialGraph
                where friend.Type == SocialGraphType.Friends &&
                      friend.ScreenName == "JoeMayo"
                select friend;

            friends.ToList().ForEach(
                friend => Console.WriteLine("Friend ID: " + friend.ID));
        }

        #endregion

        #region Friendship Demos

        private static void CreateFriendshipNoDeviceUpdatesDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.CreateFriendship("LinqToTweeter", string.Empty, string.Empty, false);

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        private static void DestroyFriendshipDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.DestroyFriendship("LinqToTweeter", string.Empty, string.Empty);

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        private static void CreateFriendshipFollowDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.CreateFriendship("LinqToTweeter", string.Empty, string.Empty, true);

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        /// <summary>
        /// shows how to show that one user follows another with Friendship Exists
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void FriendshipExistsDemo(TwitterContext twitterCtx)
        {
            var friendship =
                from friend in twitterCtx.Friendship
                where friend.Type == FriendshipType.Exists &&
                      friend.SubjectUser == "LinqToTweeter" &&
                      friend.FollowingUser == "JoeMayo"
                select friend;

            Console.WriteLine(
                "JoeMayo follows LinqToTweeter: " +
                friendship.ToList().First().IsFriend);
        }

        #endregion

        #region Direct Message Demos

        /// <summary>
        /// shows how to delete a direct message
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DestroyDirectMessageDemo(TwitterContext twitterCtx)
        {
            var message = twitterCtx.DestroyDirectMessage("96404341");

            if (message != null)
            {
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}",
                    message.RecipientScreenName,
                    message.Text);
            }
        }

        /// <summary>
        /// shows how to send a new direct message
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void NewDirectMessageDemo(TwitterContext twitterCtx)
        {
            var message = twitterCtx.NewDirectMessage("16761255", "Direct Message Test - " + DateTime.Now);

            if (message != null)
            {
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}, Date: {2}",
                    message.RecipientScreenName,
                    message.Text,
                    message.CreatedAt);
            }
        }

        /// <summary>
        /// shows how to query direct messages
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DirectMessageSentToQueryDemo(TwitterContext twitterCtx)
        {
            var directMessages =
                from tweet in twitterCtx.DirectMessage
                where tweet.Type == DirectMessageType.SentTo &&
                      tweet.Count == 2
                select tweet;

            directMessages.ToList().ForEach(
                dm => Console.WriteLine(
                    "Sender: {0}, Tweet: {1}",
                    dm.SenderScreenName,
                    dm.Text));
        }

        /// <summary>
        /// shows how to query direct messages
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DirectMessageSentByQueryDemo(TwitterContext twitterCtx)
        {
            var directMessages =
                from tweet in twitterCtx.DirectMessage
                where tweet.Type == DirectMessageType.SentBy &&
                      tweet.Count == 2
                select tweet;

            directMessages.ToList().ForEach(
                dm => Console.WriteLine(
                    "Sender: {0}, Tweet: {1}",
                    dm.SenderScreenName,
                    dm.Text));
        }

        #endregion

        #region User Demos

        /// <summary>
        /// shows how to query users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserShowWithScreenNameQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Show &&
                      tweet.ScreenName == "JoeMayo"
                select tweet;

            var user = users.SingleOrDefault();

            var name = user.Name;
            var lastStatus = user.Status == null ? "No Status" : user.Status.Text;

            Console.WriteLine();
            Console.WriteLine("Name: {0}, Last Tweet: {1}\n", name, lastStatus);
        }
        
        /// <summary>
        /// shows how to query users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserShowWithIDQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Show &&
                      tweet.ID == "15411837"
                select tweet;

            var user = users.SingleOrDefault();

            Console.WriteLine(
                "Name: {0}, Last Tweet: {1}\n",
                user.Name, user.Status.Text);
        }

        /// <summary>
        /// shows how to query friends of a specified user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserFriendsQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Friends &&
                      tweet.ID == twitterCtx.UserName
                //                     tweet.ID == "15411837" // <-- user to get friends for
                select tweet;

            foreach (var user in users)
            {
                var status = user.Protected ? "Status Unavailable" : user.Status.Text;

                Console.WriteLine(
                        "ID: {0}, Name: {1}\nLast Tweet: {2}\n",
                        user.ID, user.Name, status);
            }
        }

        /// <summary>
        /// shows how to query users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserFollowersQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Followers &&
                      tweet.ID == "15411837"
                select tweet;

            foreach (var user in users)
            {
                var status =
                    user.Protected || user.Status == null ?
                        "Status Unavailable" :
                        user.Status.Text;

                Console.WriteLine(
                        "Name: {0}, Last Tweet: {1}\n",
                        user.Name, status);
            }
        }

        /// <summary>
        /// shows how to query all followers
        /// </summary>
        /// <remarks>
        /// uses the Page property because Twitter doesn't
        /// return all followers in a single call; you
        /// must page through results until you get all
        /// </remarks>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetAllFollowersQueryDemo(TwitterContext twitterCtx)
        {
            var followerList = new List<User>();

            List<User> followers = new List<User>();
            int pageNumber = 1;

            do
            {
                followers.Clear();

                followers =
                    (from follower in twitterCtx.User
                     where follower.Type == UserType.Followers &&
                           follower.ScreenName == "JoeMayo" &&
                           follower.Page == pageNumber
                     select follower)
                     .ToList();

                pageNumber++;
                followerList.AddRange(followers);
            }
            while (followers.Count > 0);

            Console.WriteLine("\nFollowers: \n");

            foreach (var user in followerList)
            {
                var status =
                    user.Protected || user.Status == null ?
                        "Status Unavailable" :
                        user.Status.Text;

                Console.WriteLine(
                        "Name: {0}, Last Tweet: {1}\n",
                        user.Name, status);
            }

            Console.WriteLine("\nFollower Count: {0}\n", followerList.Count);
        }

        #endregion

        #region Status Demos

        /// <summary>
        /// Shows how to get statuses for logged-in user's friends - just like main Twitter page
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SingleStatusQueryDemo(TwitterContext twitterCtx)
        {
            var friendTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Show &&
                      tweet.ID == "2534357295"
                select tweet;

            Console.WriteLine("\nRequested Tweet: \n");
            foreach (var tweet in friendTweets)
            {
                Console.WriteLine(
                    "User: " + tweet.User.Name +
                    "\nTweet: " + tweet.Text + 
                    "\nTweet ID: " + tweet.ID + "\n");
            }
        }

        /// <summary>
        /// Shows how to get statuses for logged-in user's friends - just like main Twitter page
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void FriendStatusQueryDemo(TwitterContext twitterCtx)
        {
            var friendTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Friends
                select tweet;

            Console.WriteLine("\nTweets for " + twitterCtx.UserName + "\n");
            foreach (var tweet in friendTweets)
            {
                Console.WriteLine(
                    "Friend: " + tweet.User.Name +
                    "\nTweet: " + tweet.Text + "\n");
            }
        }

        /// <summary>
        /// Shows how to query tweets menioning logged-in user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void MentionsStatusQueryDemo(TwitterContext twitterCtx)
        {
            var myMentions =
                from mention in twitterCtx.Status
                where mention.Type == StatusType.Mentions
                select mention;

            myMentions.ToList().ForEach(
                mention => Console.WriteLine(
                    "Name: {0}, Tweet: {1}\n",
                    mention.User.Name, mention.Text));
        }

        /// <summary>
        /// shows how to query status with a screen name for specified number of tweets
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserStatusByNameQueryDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine();

            var lastN = 20;
            var screenName = "JoeMayo";

            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                      && tweet.ScreenName == screenName
                      && tweet.Count == lastN
                select tweet;

            foreach (var tweet in statusTweets)
            {
                Console.WriteLine(
                    "(" + tweet.ID + ")" +
                    "[" + tweet.User.ID + "]" +
                    tweet.User.Name + ", " +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            }
        }

        /// <summary>
        /// shows how to query status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserStatusQueryDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine();

            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                      && tweet.ID == "15411837"  // ID for User
                      && tweet.Page == 1
                      && tweet.Count == 20
                      && tweet.SinceID == 931894254
                select tweet;

            foreach (var tweet in statusTweets)
            {
                Console.WriteLine(
                    "(" + tweet.ID + ")" +
                    "[" + tweet.User.ID + "]" +
                    tweet.User.Name + ", " +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            }
        }

        /// <summary>
        /// shows how to query status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void FirstStatusQueryDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine();

            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                      && tweet.ID == "15411837"  // ID for User
                      && tweet.Page == 1
                      && tweet.Count == 20
                      && tweet.SinceID == 931894254
                select tweet;

            var status = statusTweets.FirstOrDefault();

            Console.WriteLine(
                "(" + status.ID + ")" +
                "[" + status.User.ID + "]" +
                status.User.Name + ", " +
                status.Text + ", " +
                status.CreatedAt);
        }

        /// <summary>
        /// shows how to delete a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DestroyStatusDemo(TwitterContext twitterCtx)
        {
            var status = twitterCtx.DestroyStatus("1539399086");

            Console.WriteLine(
                "(" + status.ID + ")" +
                "[" + status.User.ID + "]" +
                status.User.Name + ", " +
                status.Text + ", " +
                status.CreatedAt);
        }

        /// <summary>
        /// shows how to update a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusWithReplyDemo(TwitterContext twitterCtx)
        {
            var tweet = twitterCtx.UpdateStatus("@LinqToTweeter Testing LINQ to Twitter with reply on " + DateTime.Now.ToString() + " #linqtotwitter", "961760788");

            Console.WriteLine(
                "(" + tweet.ID + ")" +
                "[" + tweet.User.ID + "]" +
                tweet.User.Name + ", " +
                tweet.Text + ", " +
                tweet.CreatedAt);
        }

        /// <summary>
        /// shows how to update a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusDemo(TwitterContext twitterCtx)
        {
            // the \u00C7 is C Cedilla, which I've included to ensure that non-ascii characters appear properly
            var status = "\u00C7 Testing LINQ to Twitter update status on " + DateTime.Now.ToString() + " #linqtotwitter";

            Console.WriteLine("Status being sent: " + status);

            var tweet = twitterCtx.UpdateStatus(status);

            Console.WriteLine(
                "Status returned: " +
                "(" + tweet.ID + ")" +
                "[" + tweet.User.ID + "]" +
                tweet.User.Name + ", " +
                tweet.Text + ", " +
                tweet.CreatedAt);
        }

        public class MyTweetClass
        {
            public string UserName { get; set; }
            public string Text { get; set; }
        }

        /// <summary>
        /// shows how to send a public status query and then filter
        /// </summary>
        /// <remarks>
        /// since Twitter API doesn't filter public status,
        /// you can grab the results and then filter with
        /// LINQ to Objects.
        /// </remarks>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void PublicStatusFilteredQueryDemo(TwitterContext twitterCtx)
        {
            var publicTweets =
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Public &&
                       tweet.User.Name.StartsWith("S")
                 orderby tweet.Source
                 select new MyTweetClass
                 {
                     UserName = tweet.User.Name,
                     Text = tweet.Text
                 })
                 .ToArray();

            publicTweets.ToList().ForEach(
                tweet => Console.WriteLine(
                    "User Name: {0}, Tweet: {1}",
                    tweet.UserName,
                    tweet.Text));

            //publicTweets.ToList().ForEach(
            //    tweet => Console.WriteLine(
            //        "User Name: {0}, Tweet: {1}",
            //        tweet.User.Name,
            //        tweet.Text));

            //var publicTweets = twitterCtx.Status
            //    .Where(x => x.Type == StatusType.Public)
            //    .Select(x => x.Text.Replace('\n', ' '))
            //    .ToArray();

            //publicTweets.ToList().ForEach(
            //    tweet => Console.WriteLine(
            //        "Tweet: {0}",
            //        tweet));
        }

        /// <summary>
        /// shows how to send a public status query
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void PublicStatusQueryDemo(TwitterContext twitterCtx)
        {
            var publicTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            publicTweets.ToList().ForEach(
                tweet => Console.WriteLine(
                    "User Name: {0}, Tweet: {1}",
                    tweet.User.Name,
                    tweet.Text));
        }

        #endregion
    }
}
