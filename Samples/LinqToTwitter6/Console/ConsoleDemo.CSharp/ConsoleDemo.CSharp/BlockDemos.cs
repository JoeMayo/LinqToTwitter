using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
{
    class BlockDemos
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
                        Console.WriteLine("\n\tLookup blocked Users...\n");
                        await LookupBlockedUsersAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tListing blocked Users...\n");
                        await ListBlockedUsersAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tListing blocked IDs...\n");
                        await ListBlockIDsAsyc(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tBlocking user...\n");
                        await BlockUserAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tUnblocking user...\n");
                        await UnblockUserAsync(twitterCtx);
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
            Console.WriteLine("\nBlock Demos - Please select:\n");

            Console.WriteLine("\t 0. Lookup Blocked Users");
            Console.WriteLine("\t 1. List Blocked Users");
            Console.WriteLine("\t 2. List Blocked IDs");
            Console.WriteLine("\t 3. Block a User");
            Console.WriteLine("\t 4. Unblock a User");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task LookupBlockedUsersAsync(TwitterContext twitterCtx)
        {
            string userID = "15411837";

            TwitterBlocksQuery? blockResponse =
                await
                    (from block in twitterCtx.TwitterBlocks
                     where
                        block.Type == BlockingType.Lookup &&
                        block.ID == userID &&
                        block.TweetFields == TweetField.AllFields &&
                        block.UserFields == UserField.AllFields
                     select block)
                    .SingleOrDefaultAsync();

            if (blockResponse != null && blockResponse.Users != null)
                blockResponse.Users.ForEach(user =>
                        Console.WriteLine(user.Name));
        }

        static async Task ListBlockedUsersAsync(TwitterContext twitterCtx)
        {
            var blockResponse =
                await
                    (from block in twitterCtx.Blocks
                     where block.Type == BlockingType.List
                     select block)
                    .SingleOrDefaultAsync();

            if (blockResponse != null && blockResponse.Users != null)
                blockResponse.Users.ForEach(user =>
                        Console.WriteLine(user.ScreenNameResponse)); 
        }
        
        static async Task ListBlockIDsAsyc(TwitterContext twitterCtx)
        {
            var result =
                await
                (from blockItem in twitterCtx.Blocks
                 where blockItem.Type == BlockingType.Ids
                 select blockItem)
                .SingleOrDefaultAsync();

            if (result != null && result.IDs != null)
                result.IDs.ForEach(block => Console.WriteLine("ID: {0}", block)); 
        }

        static async Task BlockUserAsync(TwitterContext twitterCtx)
        {
            Console.Write("User Screen Name to Block: ");
            string? userName = Console.ReadLine() ?? "";

            TwitterUserQuery? userResponse =
                await
                (from usr in twitterCtx.TwitterUser
                 where usr.Type == UserType.UsernameLookup &&
                       usr.Usernames == userName
                 select usr)
                .SingleOrDefaultAsync();

            string? targetUserID = userResponse?.ID;
            string? sourceUserID = twitterCtx.Authorizer?.CredentialStore?.UserID.ToString();

            if (targetUserID == null || sourceUserID == null)
            {
                Console.WriteLine($"Either {nameof(targetUserID)} or {nameof(sourceUserID)} is null.");
                return;
            }

            BlockingResponse? user = await twitterCtx.BlockUserAsync(sourceUserID, targetUserID);

            if (user?.Data != null)
                Console.WriteLine("Is Blocked: " + user.Data.Blocking);
        }

        static async Task UnblockUserAsync(TwitterContext twitterCtx)
        {
            Console.Write("User Screen Name to Block: ");
            string? userName = Console.ReadLine() ?? "";

            TwitterUserQuery? userResponse =
                await
                (from usr in twitterCtx.TwitterUser
                 where usr.Type == UserType.UsernameLookup &&
                       usr.Usernames == userName
                 select usr)
                .SingleOrDefaultAsync();

            string? targetUserID = userResponse?.ID;
            string? sourceUserID = twitterCtx.Authorizer?.CredentialStore?.UserID.ToString();

            if (targetUserID == null || sourceUserID == null)
            {
                Console.WriteLine($"Either {nameof(targetUserID)} or {nameof(sourceUserID)} is null.");
                return;
            }

            BlockingResponse? user = await twitterCtx.UnblockUserAsync(sourceUserID, targetUserID);

            if (user?.Data != null)
                Console.WriteLine("Is Blocked: " + user.Data.Blocking);
        }
    }
}
