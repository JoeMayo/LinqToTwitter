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
            try
            {
                Task demoTask = DoDemosAsync();
                demoTask.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

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
                        Console.WriteLine("\n\tRunning Account Demos...\n");
                        await AccountDemos.RunAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tRunning Block Demos...\n");
                        await BlockDemos.RunAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tRunning Direct Message Demos...\n");
                        await DirectMessageDemos.RunAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tRunning Favorite Demos...\n");
                        await FavoriteDemos.RunAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tRunning Friendship Demos...\n");
                        await FriendshipDemos.RunAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tRunning Geo Demos...\n");
                        await GeoDemos.RunAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\n\tRunning Help Demos...\n");
                        await HelpDemos.RunAsync(twitterCtx);
                        break;
                    case '7':
                        Console.WriteLine("\n\tRunning List Demos...\n");
                        await ListDemos.RunAsync(twitterCtx);
                        break;
                    case '8':
                        Console.WriteLine("\n\tRunning Raw Demos...\n");
                        await RawDemos.RunAsync(twitterCtx);
                        break;
                    case '9':
                        Console.WriteLine("\n\tRunning Saved Search Demos...\n");
                        await SavedSearchDemos.RunAsync(twitterCtx);
                        break;
                    case 'a':
                    case 'A':
                        Console.WriteLine("\n\tRunning Search Demos...\n");
                        await SearchDemos.RunAsync(twitterCtx);
                        break;
                    case 'b':
                    case 'B':
                        Console.WriteLine("\n\tRunning Social Graph Demos...\n");
                        await SocialGraphDemos.RunAsync(twitterCtx);
                        break;
                    case 'c':
                    case 'C':
                        Console.WriteLine("\n\tRunning Status Demos...\n");
                        await StatusDemos.RunAsync(twitterCtx);
                        break;
                    case 'd':
                    case 'D':
                        Console.WriteLine("\n\tRunning Stream Demos...\n");
                        await StreamDemos.RunAsync(twitterCtx);
                        break;
                    case 'e':
                    case 'E':
                        Console.WriteLine("\n\tRunning Trend Demos...\n");
                        await TrendDemos.RunAsync(twitterCtx);
                        break;
                    case 'f':
                    case 'F':
                        Console.WriteLine("\n\tRunning User Demos...\n");
                        await UserDemos.RunAsync(twitterCtx);
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

            Console.WriteLine("\t 0. Account Demos");
            Console.WriteLine("\t 1. Block Demos");
            Console.WriteLine("\t 2. Direct Message Demos");
            Console.WriteLine("\t 3. Favorite Demos");
            Console.WriteLine("\t 4. Friendship Demos");
            Console.WriteLine("\t 5. Geo Demos");
            Console.WriteLine("\t 6. Help Demos");
            Console.WriteLine("\t 7. List Demos");
            Console.WriteLine("\t 8. Raw Demos");
            Console.WriteLine("\t 9. Saved Search Demos");
            Console.WriteLine("\t A. Search Demos");
            Console.WriteLine("\t B. Social Graph Demos");
            Console.WriteLine("\t C. Status Demos");
            Console.WriteLine("\t D. Stream Demos");
            Console.WriteLine("\t E. Trend Demos");
            Console.WriteLine("\t F. User Demos");
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
