using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    public class VineDemos
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
                        Console.WriteLine("\n\tGetting HTML...\n");
                        await GetEmbeddedHtml(twitterCtx);
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

            Console.WriteLine("\t 0. Get Embedded HTML");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task GetEmbeddedHtml(TwitterContext twitterCtx)
        {
            var vineResponse =
                await
                (from vine in twitterCtx.Vine
                 where vine.Type == VineType.Oembed &&
                       vine.Url == "https://vine.co/v/Ml16lZVTTxe"
                 select vine)
                .SingleOrDefaultAsync();

            Console.WriteLine("\nOembed HTML:\n\n" + vineResponse.Html);
        }
    }
}
