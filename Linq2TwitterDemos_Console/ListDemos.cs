using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class ListDemos
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
                        Console.WriteLine("\n\tGetting Lists...\n");
                        await GetListsForUser(twitterCtx);
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

        static async Task GetListsForUser(TwitterContext twitterCtx)
        {
            var lists =
                await
                    (from list in twitterCtx.List
                     where list.Type == ListType.Lists &&
                           list.ScreenName == "Linq2Tweeter"
                     select list)
                    .ToListAsync();

            lists.ForEach(list => Console.WriteLine("Slug: " + list.SlugResult));
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nList Demos - Please select:\n");

            Console.WriteLine("\t 0. Get Lists for User");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
