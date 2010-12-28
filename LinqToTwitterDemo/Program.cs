using System;
using System.Configuration;
using System.Linq;
using LinqToTwitter;
using System.Diagnostics;

namespace LinqToTwitterDemo
{
    class Program
    {
        static void Main()
        {
            //
            // This first part is for API's that don't require authentication
            //

            Console.WriteLine("Getting Public Statuses without authentication. Press any key to continue...\n");
            Console.ReadKey();

            var ctx = new TwitterContext();

            var tweets =
                (from tweet in ctx.Status
                 where tweet.Type == StatusType.Public
                 select tweet)
                .ToList();

            tweets.ForEach(tweet => 
                Console.WriteLine(
                    "User: {0}\nTweet: {1}\n", 
                    tweet.User.Identifier.ScreenName, 
                    tweet.Text));

            Console.WriteLine("... that was public statuses with no authentication. Now, you'll see a demo of how to authenticate with OAuth. Press any key to continue...\n");
            Console.ReadKey();
            
            // Uncomment the code below if you need to use XAuth. Generally, XAuth isn't available unless you specifically
            // justify using it with Twitter: http://dev.twitter.com/pages/xauth. You should use OAuth instead.  However,
            // LINQ to Twitter supports XAuth if you're one of the rare cases that Twitter gives permission to.

            //DoXAuthDemo();

            //Console.ReadKey();
            //return;

            //
            // The rest of the example demonstrates how to authenticate with OAuth
            //

            #region Set up OAuth
            
            // validate that credentials are present
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["twitterConsumerKey"]) ||
                string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["twitterConsumerSecret"]))
            {
                Console.WriteLine("You need to set twitterConsumerKey and twitterConsumerSecret in App.config/appSettings. Visit http://dev.twitter.com/apps for more info.\n");
                Console.Write("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            // configure the OAuth object
            var auth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"]
                },
                //UseCompression = true,
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    // this executes after user authorizes, which begins with the call to auth.Authorize() below.
                    Console.WriteLine("\nAfter you authorize this application, Twitter will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };

            // start the authorization process (launches Twitter authorization page).
            auth.Authorize();

            #endregion

            using (var twitterCtx = new TwitterContext(auth, "https://api.twitter.com/1/", "https://search.twitter.com/"))
            {
                //Log
                twitterCtx.Log = Console.Out;
                
                #region Demos

                //
                // Each Run section below will execute at least one demo
                // from the specified area of the Twitter API.
                // Uncomment and navigate to code to see the example.
                //
                // LINQ to Twitter documentation "Making API Calls" is here:
                //  http://linqtotwitter.codeplex.com/wikipage?title=Making%20API%20Calls&referringTitle=Documentation
                //
                // Each section supports the Twitter API, as documented here:
                //  http://dev.twitter.com/doc
                //

                //AccountDemos.Run(twitterCtx);
                //BlocksDemos.Run(twitterCtx);
                //DirectMessageDemos.Run(twitterCtx);
                //FavoritesDemos.Run(twitterCtx);
                //FriendshipDemos.Run(twitterCtx);
                //GeoDemos.Run(twitterCtx);
                //LegalDemos.Run(twitterCtx);
                //ListDemos.Run(twitterCtx);
                //RawDemos.Run(twitterCtx);
                //SavedSearchDemos.Run(twitterCtx);
                //SearchDemos.Run(twitterCtx);
                //SocialGraphDemos.Run(twitterCtx);
                StatusDemos.Run(twitterCtx);
                //StreamingDemo.Run(twitterCtx);
                //TrendsDemos.Run(twitterCtx);
                //UserDemos.Run(twitterCtx);
                //NotificationsDemos.Run(twitterCtx);
                //HelpDemos.Run(twitterCtx);
                //ReportSpamDemos.Run(twitterCtx);
                //ErrorHandlingDemos.Run(twitterCtx);
                //OAuthDemos.Run(twitterCtx);
                //TwitterContextDemos.Run(twitterCtx);

                #endregion
            }

            Console.WriteLine("Press any key to end this demo.");
            Console.ReadKey();
        }

        private static void DoXAuthDemo()
        {
            // validate that credentials are present
            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["twitterConsumerKey"]) ||
                string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["twitterConsumerSecret"]))
            {
                Console.WriteLine("You need to set twitterConsumerKey and twitterConsumerSecret in App.config/appSettings. Visit http://dev.twitter.com/apps for more info.\n");
                Console.Write("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            // configure the OAuth object
            var auth = new XAuthAuthorizer
            {
                Credentials = new XAuthCredentials
                {
                    ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"],
                    UserName = "YourUserName",
                    Password = "YourPassword"
                }
            };

            // authorize with Twitter
            auth.Authorize();

            using (var twitterCtx = new TwitterContext(auth, "https://api.twitter.com/1/", "https://search.twitter.com/"))
            {
                //Log
                twitterCtx.Log = Console.Out;

                var users =
                    (from tweet in twitterCtx.User
                     where tweet.Type == UserType.Friends &&
                           tweet.ScreenName == "JoeMayo"
                     select tweet)
                    .ToList();

                users.ForEach(user =>
                {
                    var status =
                        user.Protected || user.Status == null ?
                            "Status Unavailable" :
                            user.Status.Text;

                    Console.WriteLine(
                        "ID: {0}, Name: {1}\nLast Tweet: {2}\n",
                        user.Identifier.UserID, user.Identifier.ScreenName, status);
                });
            }
        }
    }
}
