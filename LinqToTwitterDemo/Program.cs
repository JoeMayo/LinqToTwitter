using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    class Program
    {
        static void Main()
        {
            ITwitterAuthorizer auth = ChooseAuthenticationStrategy();

            try
            {
                using (var twitterCtx = new TwitterContext(auth))
                {
                    //Log
                    twitterCtx.Log = Console.Out;

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

                    if (DoThis("demo account"))
                        AccountDemos.Run(twitterCtx);

                    //BlocksDemos.Run(twitterCtx);
                    //DirectMessageDemos.Run(twitterCtx);
                    //FavoritesDemos.Run(twitterCtx);
                    //FriendshipDemos.Run(twitterCtx);
                    //GeoDemos.Run(twitterCtx);
                    //HelpDemos.Run(twitterCtx);
                    //LegalDemos.Run(twitterCtx);
                    //ListDemos.Run(twitterCtx);
                    //RawDemos.Run(twitterCtx);
                    //RelatedResultsDemos.Run(twitterCtx);
                    //SavedSearchDemos.Run(twitterCtx);
                    //SearchDemos.Run(twitterCtx);
                    //SocialGraphDemos.Run(twitterCtx);
                    //StatusDemos.Run(twitterCtx);
                    //StreamingDemo.Run(twitterCtx);

                    if (DoThis("demo trend"))
                        TrendsDemos.Run(twitterCtx);

                    //UserDemos.Run(twitterCtx);
                    //NotificationsDemos.Run(twitterCtx);
                    //ReportSpamDemos.Run(twitterCtx);
                    //OAuthDemos.Run(twitterCtx);
                    //TwitterContextDemos.Run(twitterCtx);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Press any key to end this demo.");
            Console.ReadKey();
        }
  
        static ITwitterAuthorizer ChooseAuthenticationStrategy()
        {
            Console.WriteLine("Authentication Strategy:\n\n");

            Console.WriteLine("  1 - Pin (default)");
            Console.WriteLine("  2 - Single User");
            Console.WriteLine("  3 - XAuth");

            Console.Write("\nPlease choose (1, 2, or 3): ");
            ConsoleKeyInfo input = Console.ReadKey();
            Console.WriteLine("");

            ITwitterAuthorizer auth = null;

            switch (input.Key)
            {

                case ConsoleKey.D1:
                    auth = DoPinOAuth();
                    break;
                case ConsoleKey.D2:
                    auth = DoSingleUserAuth();
                    break;
                case ConsoleKey.D3:
                    auth = DoXAuth();
                    break;
                default:
                    auth = DoPinOAuth();
                    break;
            }

            return auth;
        }

        static ITwitterAuthorizer DoSingleUserAuth()
        {
            // validate that credentials are present
            if (ConfigurationManager.AppSettings["twitterConsumerKey"].IsNullOrWhiteSpace() ||
                ConfigurationManager.AppSettings["twitterConsumerSecret"].IsNullOrWhiteSpace() ||
                ConfigurationManager.AppSettings["twitterOAuthToken"].IsNullOrWhiteSpace() ||
                ConfigurationManager.AppSettings["twitterAccessToken"].IsNullOrWhiteSpace())
            {
                Console.WriteLine("You need to set credentials in App.config/appSettings. Visit http://dev.twitter.com/apps for more info.\n");
                Console.Write("Press any key to exit...");
                Console.ReadKey();
                return null;
            }

            // configure the OAuth object
            var auth = new SingleUserAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"],
                    OAuthToken = ConfigurationManager.AppSettings["twitterOAuthToken"],
                    AccessToken = ConfigurationManager.AppSettings["twitterAccessToken"]
                }
            };

            // Remember, do not call authorize - you don't need it.
            // auth.Authorize();
            return auth;
        }

        static ITwitterAuthorizer DoXAuth()
        {
            // validate that credentials are present
            if (ConfigurationManager.AppSettings["twitterConsumerKey"].IsNullOrWhiteSpace() ||
                ConfigurationManager.AppSettings["twitterConsumerSecret"].IsNullOrWhiteSpace())
            {
                Console.WriteLine("You need to set twitterConsumerKey and twitterConsumerSecret in App.config/appSettings. Visit http://dev.twitter.com/apps for more info.\n");
                Console.Write("Press any key to exit...");
                Console.ReadKey();
                return null;
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

            return auth;
        }

        static ITwitterAuthorizer DoPinOAuth()
        {
            // validate that credentials are present
            if (ConfigurationManager.AppSettings["twitterConsumerKey"].IsNullOrWhiteSpace() ||
                ConfigurationManager.AppSettings["twitterConsumerSecret"].IsNullOrWhiteSpace())
            {
                Console.WriteLine("You need to set twitterConsumerKey and twitterConsumerSecret in App.config/appSettings. Visit http://dev.twitter.com/apps for more info.\n");
                Console.Write("Press any key to exit...");
                Console.ReadKey();
                return null;
            }

            // configure the OAuth object
            var auth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"]
                },
                AuthAccessType = AuthAccessType.NoChange,
                UseCompression = true,
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    // this executes after user authorizes, which begins with the call to auth.Authorize() below.
                    Console.WriteLine("\nAfter authorizing this application, Twitter will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };

            // start the authorization process (launches Twitter authorization page).
            auth.Authorize();
            return auth;
        }

        static bool DoThis(string what)
        {
            Console.Write("Would you like to " + what + " (y or n): ");

            var choice = Console.ReadKey();
            var doIt = choice.KeyChar != 'n' && choice.KeyChar != 'N';

            Console.WriteLine(doIt ? "es" : "o");

            return doIt;
        }
    }
}
