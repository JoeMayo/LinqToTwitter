using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class RawDemos
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
                        Console.WriteLine("\n\tGetting account settings...\n");
                        await GetAccountSettings(twitterCtx);
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

        static async Task GetAccountSettings(TwitterContext twitterCtx)
        {
            var rawResponse =
                await
                    (from raw in twitterCtx.RawQuery
                     where raw.QueryString == "account/settings.json"
                     select raw)
                    .SingleAsync();

            Console.WriteLine("JSON Result: " + rawResponse.Result);
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nRaw Demos - Please select:\n");

            Console.WriteLine("\t 0. Query Account Settings");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
