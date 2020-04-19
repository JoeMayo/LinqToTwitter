using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class Program
    {
        static async Task Main()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                await DoDemosAsync();
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
            IAuthorizer auth = ChooseAuthenticationStrategy();

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
                        Console.WriteLine("\n\tRunning Account Activity Demos...\n");
                        await AccountActivityDemos.RunAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tRunning Block Demos...\n");
                        await BlockDemos.RunAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tRunning Direct Message Events Demos...\n");
                        await DirectMessageEventsDemos.RunAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tRunning Favorite Demos...\n");
                        await FavoriteDemos.RunAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tRunning Friendship Demos...\n");
                        await FriendshipDemos.RunAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\n\tRunning Geo Demos...\n");
                        await GeoDemos.RunAsync(twitterCtx);
                        break;
                    case '7':
                        Console.WriteLine("\n\tRunning Help Demos...\n");
                        await HelpDemos.RunAsync(twitterCtx);
                        break;
                    case '8':
                        Console.WriteLine("\n\tRunning List Demos...\n");
                        await ListDemos.RunAsync(twitterCtx);
                        break;
                    case '9':
                        Console.WriteLine("\n\tRunning Media Demos...\n");
                        await MediaDemos.RunAsync(twitterCtx);
                        break;
                    case 'a':
                    case 'A':
                        Console.WriteLine("\n\tRunning Mutes Demos...\n");
                        await MuteDemos.RunAsync(twitterCtx);
                        break;
                    case 'b':
                    case 'B':
                        Console.WriteLine("\n\tRunning Raw Demos...\n");
                        await RawDemos.RunAsync(twitterCtx);
                        break;
                    case 'c':
                    case 'C':
                        Console.WriteLine("\n\tRunning Saved Search Demos...\n");
                        await SavedSearchDemos.RunAsync(twitterCtx);
                        break;
                    case 'd':
                    case 'D':
                        Console.WriteLine("\n\tRunning Search Demos...\n");
                        await SearchDemos.RunAsync(twitterCtx);
                        break;
                    case 'e':
                    case 'E':
                        Console.WriteLine("\n\tRunning Status Demos...\n");
                        await StatusDemos.RunAsync(twitterCtx);
                        break;
                    case 'f':
                    case 'F':
                        Console.WriteLine("\n\tRunning Stream Demos...\n");
                        await StreamDemos.RunAsync(twitterCtx);
                        break;
                    case 'g':
                    case 'G':
                        Console.WriteLine("\n\tRunning Trend Demos...\n");
                        await TrendDemos.RunAsync(twitterCtx);
                        break;
                    case 'h':
                    case 'H':
                        Console.WriteLine("\n\tRunning User Demos...\n");
                        await UserDemos.RunAsync(twitterCtx);
                        break;
                    case 'i':
                    case 'I':
                        Console.WriteLine("\n\tRunning Welcome Message Demos...\n");
                        await WelcomeMessageDemos.RunAsync(twitterCtx);
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
            Console.WriteLine("\t 1. Account Activity Demos");
            Console.WriteLine("\t 2. Block Demos");
            Console.WriteLine("\t 3. Direct Message Event Demos");
            Console.WriteLine("\t 4. Favorite Demos");
            Console.WriteLine("\t 5. Friendship Demos");
            Console.WriteLine("\t 6. Geo Demos");
            Console.WriteLine("\t 7. Help Demos");
            Console.WriteLine("\t 8. List Demos");
            Console.WriteLine("\t 9. Media Demos");
            Console.WriteLine("\t A. Mutes Demos");
            Console.WriteLine("\t B. Raw Demos");
            Console.WriteLine("\t C. Saved Search Demos");
            Console.WriteLine("\t D. Search Demos");
            Console.WriteLine("\t E. Status Demos");
            Console.WriteLine("\t F. Stream Demos");
            Console.WriteLine("\t G. Trend Demos");
            Console.WriteLine("\t H. User Demos");
            Console.WriteLine("\t I. Vine Demos");
            Console.WriteLine("\t J. Welcome Message Demos");
            Console.WriteLine();
            Console.Write("\t Q. End Program");
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

            IAuthorizer auth = input.Key switch
            {
                ConsoleKey.D1 => DoPinOAuth(),
                ConsoleKey.D2 => DoApplicationOnlyAuth(),
                ConsoleKey.D3 => DoSingleUserAuth(),
                ConsoleKey.D4 => DoXAuth(),
                _ => DoPinOAuth(),
            };

            return auth;
        }

        static IAuthorizer DoPinOAuth()
        {
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
                },
                GoToTwitterAuthorization = pageLink =>
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = pageLink,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                },
                GetPin = () =>
                {
                    Console.WriteLine(
                        "\nAfter authorizing this application, Twitter " +
                        "will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };

            return auth;
        }

        static IAuthorizer DoApplicationOnlyAuth()
        {
            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
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
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret),
                    AccessToken = Environment.GetEnvironmentVariable(OAuthKeys.TwitterAccessToken),
                    AccessTokenSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterAccessTokenSecret)
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
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret),
                    UserName = "YourUserName",
                    Password = "YourPassword"
                }
            };

            return auth;
        }
    }
}
