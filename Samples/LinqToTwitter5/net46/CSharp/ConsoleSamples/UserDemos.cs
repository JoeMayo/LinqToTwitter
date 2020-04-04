using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

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
                        Console.WriteLine("\n\tLooking...\n");
                        await LookupUsersAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tShowing...\n");
                        await ShowUserDetailsAsync(twitterCtx);
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
                        Console.WriteLine("\n\tGetting...\n");
                        await GetUsersInSuggestedCategoriesAsync(twitterCtx);
                        break;
                    case '7':
                        Console.WriteLine("\n\tGetting  ...\n");
                        await GetSuggestedCategoryListQueryAsync(twitterCtx);
                        break;
                    case '8':
                        Console.WriteLine("\n\tGetting...\n");
                        await ShowUsersInCategoryAsync(twitterCtx);
                        break;
                    case '9':
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

            Console.WriteLine("\t 0. Lookup Users");
            Console.WriteLine("\t 1. Show User Info");
            Console.WriteLine("\t 2. Search for Users");
            Console.WriteLine("\t 3. Contributee Accounts");
            Console.WriteLine("\t 4. Account Contributors");
            Console.WriteLine("\t 5. Get Profile Banner Sizes");
            Console.WriteLine("\t 6. Get Suggested Users");
            Console.WriteLine("\t 7. Get Suggestion Categories");
            Console.WriteLine("\t 8. Get Suggested User Tweets");
            Console.WriteLine("\t 9. Report Spammer");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task LookupUsersAsync(TwitterContext twitterCtx)
        {
            var userResponse =
                await
                (from user in twitterCtx.User
                 where user.Type == UserType.Lookup &&
                       user.ScreenNameList == "JoeMayo,Linq2Twitr" &&
                       user.TweetMode == TweetMode.Extended
                 select user)
                .ToListAsync();

            if (userResponse != null)
                userResponse.ForEach(user => 
                    Console.WriteLine("Name: " + user.ScreenNameResponse));
        }

        static async Task ShowUserDetailsAsync(TwitterContext twitterCtx)
        {
            string screenName = "JoeMayo";
            var user =
                await
                (from usr in twitterCtx.User
                 where usr.Type == UserType.Show &&
                       usr.ScreenName == screenName &&
                       usr.TweetMode == TweetMode.Extended
                 select usr)
                .SingleOrDefaultAsync();

            if (user != null)
            {
                var name = user.ScreenNameResponse;
                var lastStatus =
                    user.Status == null ? "No Status" : user.Status.Text;

                Console.WriteLine();
                Console.WriteLine(
                    "Name: {0}, Last Tweet: {1}\n", name, lastStatus); 
            }
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
            catch (TwitterQueryException tqEx) when (tqEx.ErrorCode == 220)
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
            catch (TwitterQueryException tqEx) when (tqEx.ErrorCode == 220)
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

        static async Task GetUsersInSuggestedCategoriesAsync(TwitterContext twitterCtx)
        {
            var userResponse =
                await
                (from user in twitterCtx.User
                 where user.Type == UserType.Category &&
                       user.Slug == "Funny"
                 select user)
                .SingleOrDefaultAsync();

            if (userResponse != null && 
                userResponse.Categories != null && 
                userResponse.Categories.Any() && 
                userResponse.Categories.First().Users != null)
            {
                List<User> users = userResponse.Categories.First().Users;

                users.ForEach(user =>
                    Console.WriteLine("User: " + user.ScreenNameResponse)); 
            }
        }

        static async Task GetSuggestedCategoryListQueryAsync(TwitterContext twitterCtx)
        {
            var user =
                await
                (from tweet in twitterCtx.User
                 where tweet.Type == UserType.Categories
                 select tweet)
                .SingleOrDefaultAsync();

            if (user != null && user.Categories != null)
                user.Categories.ForEach(cat => 
                    Console.WriteLine("Category: " + cat.Name));
        }

        static async Task ShowUsersInCategoryAsync(TwitterContext twitterCtx)
        {
            var catUsers =
                await
                (from user in twitterCtx.User
                 where user.Type == UserType.CategoryStatus &&
                       user.Slug == "Technology"
                 select user)
                .ToListAsync();

            if (catUsers != null)
            {
                Console.WriteLine("Tweets: \n");

                catUsers.ForEach(user =>
                {
                    if (user != null && user.Status != null)
                        Console.WriteLine(
                            "User: {0}\nTweet: {1}\n",
                            user.ScreenNameResponse,
                            user.Status == null ?
                                "<Tweet not available.>" :
                                user.Status.Text);
                }); 
            }
        }

        static async Task ReportSpammerAsync(TwitterContext twitterCtx)
        {
            const string SpammerScreenName = "realDonaldTrump";

            User spammer = await twitterCtx.ReportSpamAsync(SpammerScreenName);

            Console.WriteLine("You just reported {0} as a spammer.", spammer.ScreenNameResponse);
        }
    }
}
