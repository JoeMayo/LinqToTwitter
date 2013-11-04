using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class Program
    {
        static void Main()
        {
            Task demoTask = DoDemosAsync();
            demoTask.Wait();

            Console.Write("\nPress any key to close console window...");
            Console.ReadKey(true);
        }
  
        static async Task DoDemosAsync()
        {
            var auth = ChooseAuthenticationStrategy();

            await auth.AuthorizeAsync();

            // This is how you access credentials after authorization.
            // The oauthToken and oauthTokenSecret do not expire.
            // You can use the userID to associate the credentials with the user.
            // You can save credentials any way you want - database, isolated storage, etc. - it's up to you.
            // You can retrieve and load all 4 credentials on subsequent queries to avoid the need to re-authorize.
            // When you've loaded all 4 credentials, LINQ to Twitter will let you make queries without re-authorizing.
            //
            //var credentials = auth.CredentialStore;
            //string oauthToken = credentials.OAuthToken;
            //string oauthTokenSecret = credentials.OAuthTokenSecret;
            //string screenName = credentials.ScreenName;
            //ulong userID = credentials.UserID;
            //

            var twitterCtx = new TwitterContext(auth);
            char key;

            do
            {
                ShowMenu();

                key = Console.ReadKey(true).KeyChar;

                switch (key)
                {
                    case '0':
                        Console.WriteLine("\n\tRunning Status Demos...\n");
                        await StatusDemos.RunStatusDemosAsync(twitterCtx);
                        break;
                    case 'q':
                    case 'Q':
                        Console.WriteLine("\nQuitting...\n");
                        break;
                    default:
                        Console.WriteLine(key + " is unknown");
                        break;
                }

            } while (char.ToUpper(key) != 'Q');
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nPlease select category:\n");

            Console.WriteLine("\t 0. Status Demos");
            Console.WriteLine();
            Console.WriteLine("\t Q. End Program");
        }


        static IAuthorizer ChooseAuthenticationStrategy()
        {
            Console.WriteLine("Authentication Strategy:\n\n");

            Console.WriteLine("  1 - Pin (default)");
            Console.WriteLine("  2 - Application-Only");
            Console.WriteLine("  3 - Single User");
            Console.WriteLine("  4 - XAuth");

            Console.Write("\nPlease choose (1, 2, 3, or 4): ");
            ConsoleKeyInfo input = Console.ReadKey();
            Console.WriteLine("");

            IAuthorizer auth = null;

            switch (input.Key)
            {

                case ConsoleKey.D1:
                    auth = DoPinOAuth();
                    break;
                case ConsoleKey.D2:
                    auth = DoApplicationOnly();
                    break;
                case ConsoleKey.D3:
                    auth = DoSingleUserAuth();
                    break;
                case ConsoleKey.D4:
                    auth = DoXAuth();
                    break;
                default:
                    auth = DoPinOAuth();
                    break;
            }

            return auth;
        }

        static IAuthorizer DoPinOAuth()
        {
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"]
                },
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    // this executes after user authorizes, which begins with the call to auth.Authorize() below.
                    Console.WriteLine("\nAfter authorizing this application, Twitter will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };

            return auth;
        }

        static IAuthorizer DoApplicationOnly()
        {
            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"]
                },
            };

            return auth;
        }
        static IAuthorizer DoSingleUserAuth()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"],
                    AccessToken = ConfigurationManager.AppSettings["accessToken"],
                    AccessTokenSecret = ConfigurationManager.AppSettings["accessTokenSecret"]
                }
            };

            return auth;
        }

        static IAuthorizer DoXAuth()
        {
            var auth = new XAuthAuthorizer
            {
                CredentialStore = new XAuthCredentials
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"],
                    UserName = "YourUserName",
                    Password = "YourPassword"
                }
            };

            return auth;
        }
    }
}
