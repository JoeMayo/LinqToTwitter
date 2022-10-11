using System;
using System.Net;
using System.Threading.Tasks;
using ConsoleDemo.CSharp;
using LinqToTwitter;
using LinqToTwitter.OAuth;

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

static async Task DoDemosAsync()
{
    IAuthorizer auth = OAuth.ChooseAuthenticationStrategy();

    await auth.AuthorizeAsync();

    // For OAuth 1.0A Only: This is how you access credentials after authorization.
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
    // For OAuth 2.0 (preferred), you can get credentials like this:
    //var credentials = auth.CredentialStore as IOAuth2CredentialStore;
    //string accessToken = credentials.AccessToken;
    //string refreshToken = credentials.RefreshToken
    //

    var twitterCtx = new TwitterContext(auth);
    char key;

    do
    {
        ShowMenu();

        key = Console.ReadKey(true).KeyChar;

        switch (char.ToUpper(key))
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
                Console.WriteLine("\n\tRunning Media Demos...\n");
                await MediaDemos.RunAsync(twitterCtx);
                break;
            case '9':
                Console.WriteLine("\n\tRunning Mutes Demos...\n");
                await MuteDemos.RunAsync(twitterCtx);
                break;
            case 'A':
                Console.WriteLine("\n\tRunning Raw Demos...\n");
                await RawDemos.RunAsync(twitterCtx);
                break;
            case 'B':
                Console.WriteLine("\n\tRunning Saved Search Demos...\n");
                await SavedSearchDemos.RunAsync(twitterCtx);
                break;
            case 'C':
                Console.WriteLine("\n\tRunning Search Demos...\n");
                await SearchDemos.RunAsync(twitterCtx);
                break;
            case 'D':
                Console.WriteLine("\n\tRunning Status Demos...\n");
                await StatusDemos.RunAsync(twitterCtx);
                break;
            case 'E':
                Console.WriteLine("\n\tRunning Stream Demos...\n");
                await StreamDemos.RunAsync(twitterCtx);
                break;
            case 'F':
                Console.WriteLine("\n\tRunning Trend Demos...\n");
                await TrendDemos.RunAsync(twitterCtx);
                break;
            case 'G':
                Console.WriteLine("\n\tRunning User Demos...\n");
                await UserDemos.RunAsync(twitterCtx);
                break;
            case 'H':
                Console.WriteLine("\n\tRunning Welcome Message Demos...\n");
                await WelcomeMessageDemos.RunAsync(twitterCtx);
                break;
            case 'I':
                Console.WriteLine("\n\tRunning Tweet Demos...\n");
                await TweetDemos.RunAsync(twitterCtx);
                break;
            case 'J':
                Console.WriteLine("\n\tRunning Compliance Demos...\n");
                await ComplianceDemos.RunAsync(twitterCtx);
                break;
            case 'K':
                Console.WriteLine("\n\tRunning Like Demos...\n");
                await LikeDemos.RunAsync(twitterCtx);
                break;
            case 'L':
                Console.WriteLine("\n\tRunning Counts Demos...\n");
                await CountsDemos.RunAsync(twitterCtx);
                break;
            case 'M':
                Console.WriteLine("\n\tRunning Spaces Demos...\n");
                await SpacesDemos.RunAsync(twitterCtx);
                break;
            case 'N':
                Console.WriteLine("\n\tRunning Bookmark Demos...\n");
                await BookmarkDemos.RunAsync(twitterCtx);
                break;
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
    Console.WriteLine("\t 4. Friendship Demos");
    Console.WriteLine("\t 5. Geo Demos");
    Console.WriteLine("\t 6. Help Demos");
    Console.WriteLine("\t 7. List Demos");
    Console.WriteLine("\t 8. Media Demos");
    Console.WriteLine("\t 9. Mutes Demos");
    Console.WriteLine("\t A. Raw Demos");
    Console.WriteLine("\t B. Saved Search Demos");
    Console.WriteLine("\t C. Search Demos");
    Console.WriteLine("\t D. Status Demos");
    Console.WriteLine("\t E. Stream Demos");
    Console.WriteLine("\t F. Trend Demos");
    Console.WriteLine("\t G. User Demos");
    Console.WriteLine("\t H. Welcome Message Demos");
    Console.WriteLine("\t I. Tweet Demos");
    Console.WriteLine("\t J. Compliance Demos");
    Console.WriteLine("\t K. Like Demos");
    Console.WriteLine("\t L. Counts Demos");
    Console.WriteLine("\t M. Spaces Demos");
    Console.WriteLine("\t N. Bookmark Demos");
    Console.WriteLine();
    Console.Write("\t Q. End Program");
}