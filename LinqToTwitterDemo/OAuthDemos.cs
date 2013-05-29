using System;
using System.Configuration;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows How to Handle OAuth with LINQ to Twitter
    /// </summary>
    public class OAuthDemos
    {
        /// <summary>
        /// Run all OAuth related demos
        /// </summary>
        public static void Run(TwitterContext twitterCtx)
        {
            //HandleOAuthQueryDemo(twitterCtx);
            //HandleOAuthSideEffectDemo(twitterCtx);
            //HandleOAuthFilePostDemo(twitterCtx);
            //HandleOAuthReadOnlyQueryDemo(twitterCtx);
            //HandleOAuthSideEffectReadOnlyDemo(twitterCtx);
            //HandleOAuthUpdateAccountBackgroundImageWithProgressUpdatesDemo(twitterCtx);
            //HandleOAuthRequestResponseDetailsDemo(twitterCtx);
            //OAuthForceLoginDemo(twitterCtx);
            HandleApplicationOnlyAuthentication();
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
                    where tweet.Type == StatusType.Home
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
                var user = twitterCtx.UpdateAccountBackgroundImage(@"C:\Users\jmayo\Documents\linq2twitter\linq2twitter\200xColor_2.png",
                    false,
                    true,
                    true,
                    true);

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
        /// Shows how to update the background image with OAuth
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleOAuthUpdateAccountBackgroundImageWithProgressUpdatesDemo(TwitterContext twitterCtx)
        {
            if (twitterCtx.AuthorizedClient.IsAuthorized)
            {
                twitterCtx.UploadProgressChanged +=
                        (sender, e) =>
                        {
                            Console.WriteLine("Progress: {0}%", e.PercentComplete);
                        };
                byte[] fileBytes = Utilities.GetFileBytes(@"C:\Users\jmayo\Documents\linq2twitter\linq2twitter\200xColor_2.png");
                var user = twitterCtx.UpdateAccountBackgroundImage(fileBytes, "200xColor_2.png", "png", true, true, true, true);

                Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
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
                        where tweet.Type == StatusType.Show
                        select tweet;

                tweets.ToList().ForEach(
                    tweet => Console.WriteLine(
                        "Friend: {0}, Created: {1}\nTweet: {2}\n",
                        tweet.User.Name,
                        tweet.CreatedAt,
                        tweet.Text)); 
            }
        }

        /// <summary>
        /// Demonstrates how to use ApplicationOnlyAuthorizer
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void HandleApplicationOnlyAuthentication()
        {
            var auth = new ApplicationOnlyAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"]
                }
            };

            auth.Authorize();
            //auth.Invalidate();

            var twitterCtx = new TwitterContext(auth);

            var srch =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "LINQ to Twitter"
                 select search)
                .SingleOrDefault();

            Console.WriteLine("\nQuery: {0}\n", srch.SearchMetaData.Query);
            srch.Statuses.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                    entry.StatusID, entry.Source, entry.Text));
        }

    }
}
