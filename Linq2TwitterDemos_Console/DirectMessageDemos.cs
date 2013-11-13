using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class DirectMessageDemos
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
                        Console.WriteLine("\n\tShowing sent DMs...\n");
                        await ShowSentDMs(twitterCtx);
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

        static async Task ShowSentDMs(TwitterContext twitterCtx)
        {
            var dmResponse =
                await
                    (from dm in twitterCtx.DirectMessage
                     where dm.Type == DirectMessageType.SentBy
                     select dm)
                    .ToListAsync();

            dmResponse.ForEach(dm => 
                Console.WriteLine(
                    "Name: {0}, Tweet: {1}",
                    dm.Recipient.ScreenNameResponse, dm.Text));
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nDirect Message Demos - Please select:\n");

            Console.WriteLine("\t 0. Sent DMs");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
