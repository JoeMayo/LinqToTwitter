using System;
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
                        Console.WriteLine("\n\tSearching for Waldo...\n");
                        await FindWaldo(twitterCtx);
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
  
        static async Task FindWaldo(TwitterContext twitterCtx)
        {
            var foundUsers =
                await
                (from user in twitterCtx.User
                 where user.Type == UserType.Search &&
                       user.Query == "Waldo"
                 select user)
                .ToListAsync();

            foundUsers.ForEach(user => Console.WriteLine("User: " + user.ScreenNameResponse));
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nUser Demos - Please select:\n");

            Console.WriteLine("\t 0. Where is Waldo?");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
