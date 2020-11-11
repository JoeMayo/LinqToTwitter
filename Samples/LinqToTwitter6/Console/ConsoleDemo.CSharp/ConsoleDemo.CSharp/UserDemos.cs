using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;

namespace Linq2TwitterDemos_Console
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
            const string SpammerScreenName = "realDonaldTrump";

            User? spammer = await twitterCtx.ReportSpamAsync(SpammerScreenName, performBlock: true);

            Console.WriteLine("You just reported {0} as a spammer.", spammer?.ScreenNameResponse);
        }
    }
}
