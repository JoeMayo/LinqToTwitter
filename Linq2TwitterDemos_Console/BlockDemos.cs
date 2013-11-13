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
                        Console.WriteLine("\n\tListing Blocked Users...\n");
                        await ListBlockedUsersAsync(twitterCtx);
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

        static async Task ListBlockedUsersAsync(TwitterContext twitterCtx)
        {
            var blockResponse =
                await
                    (from block in twitterCtx.Blocks
                     where block.Type == BlockingType.List
                     select block)
                    .SingleOrDefaultAsync();

            blockResponse.Users.ForEach(user => 
                Console.WriteLine(user.ScreenNameResponse));
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nBlock Demos - Please select:\n");

            Console.WriteLine("\t 0. List Blocked Users");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
