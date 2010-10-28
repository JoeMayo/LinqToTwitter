using System;
using System.Configuration;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    class Program
    {
        static void Main()
        {
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
                ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"],
                UseCompression = true,
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
                // Each section supports the Twitter API, as documented here:
                //  http://dev.twitter.com/doc
                //
                // LINQ to Twitter documentation "Making API Calls" is here:
                //  http://linqtotwitter.codeplex.com/wikipage?title=Making%20API%20Calls&referringTitle=Documentation
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
    }
}
