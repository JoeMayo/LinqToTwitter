using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class HelpDemos
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
                        Console.WriteLine("\n\tGetting Rate Limits...\n");
                        await GettingRateLimits(twitterCtx);
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

        static async Task GettingRateLimits(TwitterContext twitterCtx)
        {
            var helpResponse =
                await
                    (from help in twitterCtx.Help
                     where help.Type == HelpType.RateLimits
                     select help)
                    .SingleOrDefaultAsync();

            foreach (var category in helpResponse.RateLimits)
            {
                Console.WriteLine("\nCategory: {0}", category.Key);

                foreach (var limit in category.Value)
                {
                    Console.WriteLine(
                        "\n  Resource: {0}\n    Remaining: {1}\n    Reset: {2}\n    Limit: {3}",
                        limit.Resource, limit.Remaining, limit.Reset, limit.Limit);
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nHelp Demos - Please select:\n");

            Console.WriteLine("\t 0. Get Rate Limits");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
