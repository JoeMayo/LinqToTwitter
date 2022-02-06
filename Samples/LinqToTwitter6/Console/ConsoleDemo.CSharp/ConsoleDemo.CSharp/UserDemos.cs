using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
{
    public class UserDemos
    {
        internal static async Task RunAsync(TwitterContext twitterCtx)
        {
            char key;

            do
            {
                ShowMenu();

                key = Console.ReadKey(true).KeyChar;

                switch (key)
                {
                    case '0':
                        Console.WriteLine("\n\tSearching by user ID...\n");
                        await LookupByUserIDAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tSearching by user name...\n");
                        await LookupByUsernameAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tSearching...\n");
                        await FindUsersAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tShowing...\n");
                        await GetContributeesAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tSearching...\n");
                        await GetContributorsAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tGetting...\n");
                        await GetBannerSizesAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\n\tReport spammer...\n");
                        await ReportSpammerAsync(twitterCtx);
                        break;
                    case '7':
                        Console.WriteLine("\n\tFinding followers...\n");
                        await FindFollowersAsync(twitterCtx);
                        break;
                    case '8':
                        Console.WriteLine("\n\tFinding following...\n");
                        await FindFollowingAsync(twitterCtx);
                        break;
                    case '9':
                        Console.WriteLine("\n\tFollowing...\n");
                        await FollowAsync(twitterCtx);
                        break;
                    case 'a':
                    case 'A':
                        Console.WriteLine("\n\tUn-Following...\n");
                        await UnFollowAsync(twitterCtx);
                        break;
                    case 'b':
                    case 'B':
                        Console.WriteLine("\n\tFinding who retweeted...\n");
                        await DoRetweetedByAsync(twitterCtx);
                        break;
                    case 'c':
                    case 'C':
                        Console.WriteLine("\n\tRetweeting...\n");
                        await RetweetAsync(twitterCtx);
                        break;
                    case 'd':
                    case 'D':
                        Console.WriteLine("\n\tUndoing Retweet...\n");
                        await UndoRetweetAsync(twitterCtx);
                        break;
                    case 'e':
                    case 'E':
                        Console.WriteLine("\n\tGetting authenticated user...");
                        await GetMeAsync(twitterCtx);
                        break;
                    case 'q':
                    case 'Q':
                        Console.WriteLine("\nReturning...\n");
                        break;
                    default:
                        Console.WriteLine(key + " is unknown");
                        break;
                }

            } while (char.ToUpper(key) != 'Q');
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nUser Demos - Please select:\n");

            Console.WriteLine("\t 0. Lookup by User ID");
            Console.WriteLine("\t 1. Lookup by User Name");
            Console.WriteLine("\t 2. Search for Users");
            Console.WriteLine("\t 3. Contributee Accounts");
            Console.WriteLine("\t 4. Account Contributors");
            Console.WriteLine("\t 5. Get Profile Banner Sizes");
            Console.WriteLine("\t 6. Report Spammer");
            Console.WriteLine("\t 7. Find Followers");
            Console.WriteLine("\t 8. Find Following");
            Console.WriteLine("\t 9. Follow a User");
            Console.WriteLine("\t A. Un-Follow a User");
            Console.WriteLine("\t B. Retweeted by a User");
            Console.WriteLine("\t C. Retweet");
            Console.WriteLine("\t D. Undo Retweet");
            Console.WriteLine("\t E. Get Authenticated User");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task LookupByUserIDAsync(TwitterContext twitterCtx)
        {
            TwitterUserQuery? userResponse =
                await
                (from user in twitterCtx.TwitterUser
                 where user.Type == UserType.IdLookup &&
                       user.Ids == "15411837,16761255" &&
                       user.Expansions == ExpansionField.AllUserFields &&
                       user.TweetFields == TweetField.AllFieldsExceptPermissioned &&
                       user.UserFields == UserField.AllFields
                 select user)
                .SingleOrDefaultAsync();

            if (userResponse != null)
                userResponse.Users?.ForEach(user =>
                    Console.WriteLine("Name: " + user.Username));
        }

        static async Task LookupByUsernameAsync(TwitterContext twitterCtx)
        {
            TwitterUserQuery? userResponse =
                await
                (from user in twitterCtx.TwitterUser
                 where user.Type == UserType.UsernameLookup &&
                       user.Usernames == "JoeMayo,Linq2Twitr" &&
                       user.Expansions == ExpansionField.AllUserFields &&
                       user.TweetFields == TweetField.AllFieldsExceptPermissioned &&
                       user.UserFields == UserField.AllFields
                 select user)
                .SingleOrDefaultAsync();

            if (userResponse != null)
                userResponse.Users?.ForEach(user =>
                    Console.WriteLine("Name: " + user.Username));
        }

        static async Task FindUsersAsync(TwitterContext twitterCtx)
        {
            var foundUsers =
                await
                (from user in twitterCtx.User
                 where user.Type == UserType.Search &&
                       user.Query == "JoeMayo" &&
                       user.TweetMode == TweetMode.Extended
                 select user)
                .ToListAsync();

            if (foundUsers != null)
                foundUsers.ForEach(user =>
                    Console.WriteLine("User: " + user.ScreenNameResponse));
        }

        static async Task GetContributeesAsync(TwitterContext twitterCtx)
        {
            try
            {
                var users =
                    await
                    (from user in twitterCtx.User
                     where user.Type == UserType.Contributees &&
                           user.ScreenName == "biz"
                     select user)
                    .ToListAsync();

                if (users != null)
                    users.ForEach(user =>
                        Console.WriteLine("User: " + user.ScreenNameResponse));
            }
            catch (TwitterQueryException tqEx)
            {
                Console.WriteLine("Unable to query - Reason: " + tqEx.ReasonPhrase);
            }
        }

        static async Task GetContributorsAsync(TwitterContext twitterCtx)
        {
            try
            {
                var users =
                    await
                    (from user in twitterCtx.User
                     where user.Type == UserType.Contributors &&
                           user.ScreenName == "twitter"
                     select user)
                    .ToListAsync();

                if (users != null)
                    users.ForEach(user =>
                        Console.WriteLine("User: " + user.ScreenNameResponse));
            }
            catch (TwitterQueryException tqEx)
            {
                Console.WriteLine("Unable to query - Reason: " + tqEx.ReasonPhrase);
            }
        }

        static async Task GetBannerSizesAsync(TwitterContext twitterCtx)
        {
            var user =
                await
                (from usr in twitterCtx.User
                 where usr.Type == UserType.BannerSizes &&
                       usr.ScreenName == "JoeMayo"
                 select usr)
                .SingleOrDefaultAsync();

            if (user != null && user.BannerSizes != null)
                user.BannerSizes.ForEach(size =>
                    Console.WriteLine(
                        "Label: {0}, W: {1} H: {2} URL: {3}",
                        size.Label, size.Width, size.Height, size.Url));
        }

        static async Task ReportSpammerAsync(TwitterContext twitterCtx)
        {
            const string SpammerScreenName = "<put screen name here>";

            User? spammer = await twitterCtx.ReportSpamAsync(SpammerScreenName, performBlock: true);

            Console.WriteLine("You just reported {0} as a spammer.", spammer?.ScreenNameResponse);
        }

        static async Task FindFollowersAsync(TwitterContext twitterCtx)
        {
            string userID = "15411837";

            TwitterUserQuery? userResponse =
                await
                (from user in twitterCtx.TwitterUser
                 where user.Type == UserType.Followers &&
                       user.ID == userID
                 select user)
                .SingleOrDefaultAsync();

            if (userResponse != null)
                userResponse.Users?.ForEach(user =>
                    Console.WriteLine("Name: " + user.Username));
        }

        static async Task FindFollowingAsync(TwitterContext twitterCtx)
        {
            string userID = "15411837";

            TwitterUserQuery? userResponse =
                await
                (from user in twitterCtx.TwitterUser
                 where user.Type == UserType.Following &&
                       user.ID == userID
                 select user)
                .SingleOrDefaultAsync();

            if (userResponse != null)
                userResponse.Users?.ForEach(user =>
                    Console.WriteLine("ID: " + user.ID));
        }

        static async Task FollowAsync(TwitterContext twitterCtx)
        {
            string followingUser = "15411837";
            string userToFollow = "16761255";

            TwitterUserFollowResponse? response = 
                await twitterCtx.FollowAsync(followingUser, userToFollow);

            Console.WriteLine($"Is Following: {response?.Data?.Following ?? false}");
        }

        static async Task UnFollowAsync(TwitterContext twitterCtx)
        {
            string followingUser = "15411837";
            string userToFollow = "<put account ID here>";

            TwitterUserFollowResponse? response =
                await twitterCtx.UnFollowAsync(followingUser, userToFollow);

            Console.WriteLine($"Is Following: {response?.Data?.Following ?? false}");
        }

        static async Task DoRetweetedByAsync(TwitterContext twitterCtx)
        {
            string tweetID = "1446476275246194697";

            TwitterUserQuery? response =
                await
                (from user in twitterCtx.TwitterUser
                 where user.Type == UserType.RetweetedBy &&
                       user.ID == tweetID
                 select user)
                .SingleOrDefaultAsync();

            if (response?.Users != null)
                response.Users.ForEach(user =>
                    Console.WriteLine(
                        $"\nID: {user.ID}" +
                        $"\nUsername: {user.Username}" +
                        $"\nName: {user.Name}"));
            else
                Console.WriteLine("No entries found.");
        }

        async static Task RetweetAsync(TwitterContext twitterCtx)
        {
            string retweetingUser = "15411837";
            string tweetToRetweet = "1376560011678085128";

            RetweetResponse? response =
                await twitterCtx.RetweetAsync(retweetingUser, tweetToRetweet);

            Console.WriteLine($"Is Retweeted: {response?.Data?.Retweeted ?? false}");
        }

        async static Task UndoRetweetAsync(TwitterContext twitterCtx)
        {
            string retweetingUser = "15411837";
            string tweetToUndoRetweet = "1376560011678085128";

            RetweetResponse? response =
                await twitterCtx.UndoRetweetAsync(retweetingUser, tweetToUndoRetweet);

            Console.WriteLine($"Is Retweeted: {response?.Data?.Retweeted ?? false}");
        }

        static async Task GetMeAsync(TwitterContext twitterCtx)
        {
            TwitterUserQuery? response =
                await
                (from usr in twitterCtx.TwitterUser
                 where usr.Type == UserType.Me
                 select usr)
                .SingleOrDefaultAsync();

            TwitterUser? user = response?.Users?.SingleOrDefault();

            if (user != null)
                Console.WriteLine(
                    $"\nID: {user.ID}" +
                    $"\nUsername: {user.Username}" +
                    $"\nName: {user.Name}");
        }
    }
}
