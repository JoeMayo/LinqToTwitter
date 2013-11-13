using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class SocialGraphDemos
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
                        Console.WriteLine("\n\tShowing friend IDs...\n");
                        await ShowFriendIDs(twitterCtx);
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

        static async Task ShowFriendIDs(TwitterContext twitterCtx)
        {
            var socialGraph =
                await
                    (from id in twitterCtx.SocialGraph
                     where id.Type == SocialGraphType.Friends &&
                           id.ScreenName == "JoeMayo"
                     select id)
                    .SingleAsync();

            socialGraph.IDs.ForEach(id => Console.WriteLine(id));
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nSocial Graph Demos - Please select:\n");

            Console.WriteLine("\t 0. Show Friend IDs");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
