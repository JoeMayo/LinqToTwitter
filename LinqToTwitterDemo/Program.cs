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
                // If you wanted to pass the consumer key and secret in programmatically, you could do so as shown here.
                // Otherwise this information is pulled out of your .config file.
                ////var desktopAuth = (DesktopOAuthAuthorization)auth;
                ////desktopAuth.ConsumerKey = "some key";
                ////desktopAuth.ConsumerSecret = "some secret";
            }

            auth.UseCompression = true;

            // TwitterContext is similar to DataContext (LINQ to SQL) or ObjectContext (LINQ to Entities)

            // For Twitter
            using (var twitterCtx = new TwitterContext(auth, "https://api.twitter.com/1/", "https://search.twitter.com/"))
            //using (var twitterCtx = new TwitterContext(auth, "https://api.twitter.com/1/", "https://api.twitter.com/1/"))
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
                try
                {
                    auth.SignOn();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Login canceled. Demo exiting.");
                    return;
                }

                //AccountDemos.Run(twitterCtx);
                //BlocksDemos.Run(twitterCtx);
                //DirectMessageDemos.Run(twitterCtx);
                //FavoritesDemos.Run(twitterCtx);
                //FriendshipDemos.Run(twitterCtx);
                //GeoDemos.Run(twitterCtx);
                //ListDemos.Run(twitterCtx);
                //SavedSearchDemos.Run(twitterCtx);
                SearchDemos.Run(twitterCtx);
                //SocialGraphDemos.Run(twitterCtx);
                //StatusDemos.Run(twitterCtx);
                //TrendsDemos.Run(twitterCtx);
                //UserDemos.Run(twitterCtx);
                //NotificationsDemos.Run(twitterCtx);
                //HelpDemos.Run(twitterCtx);
                //ReportSpamDemos.Run(twitterCtx);
                //ErrorHandlingDemos.Run(twitterCtx);
                //OAuthDemos.Run(twitterCtx);
                //TwitterContextDemos.Run(twitterCtx);

                //
                // Sign-off, including optional clearing of cached credentials.
                //

                //auth.SignOff();
                //auth.ClearCachedCredentials();
            }

            Console.WriteLine("Press any key to end this demo.");
            Console.ReadKey();
        }

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
    }
}
