using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
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
                        Console.WriteLine("\n\tListing blocked Users...\n");
                        await ListBlockedUsersAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tListing blocked IDs...\n");
                        await ListBlockIDsAsyc(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tBlocking user...\n");
                        await CreateBlockAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tUnblocking user...\n");
                        await DestroyBlockAsync(twitterCtx);
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

            Console.WriteLine("\t 0. List Blocked Users");
            Console.WriteLine("\t 1. List Blocked IDs");
            Console.WriteLine("\t 2. Block a User");
            Console.WriteLine("\t 3. Unblock a User");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
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

        static async Task CreateBlockAsync(TwitterContext twitterCtx)
        {
            Console.Write("User Screen Name to Block: ");
            string userName = Console.ReadLine();

            var user = await twitterCtx.CreateBlockAsync(0, userName, true);

            if (user != null)
                Console.WriteLine("User Name: " + user.Name);
        }

        static async Task DestroyBlockAsync(TwitterContext twitterCtx)
        {
            Console.Write("User Screen Name to Unblock: ");
            string userName = Console.ReadLine();

            var user = await twitterCtx.DestroyBlockAsync(0, userName, true);

            if (user != null) 
                Console.WriteLine("User Name: " + user.Name);
        }
    }
}
