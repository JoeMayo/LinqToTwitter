using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Collections.Generic;

namespace Linq2TwitterDemos_Console
{
    class WelcomeMessageDemos
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
                        Console.WriteLine("\n\tCreating Welcome Message...\n");
                        await CreateNewWelcomeMessageAsync(twitterCtx);
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
            Console.WriteLine("\nDirect Message Demos - Please select:\n");

            Console.WriteLine("\t 0. Create a New Welcome Message");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task CreateNewWelcomeMessageAsync(TwitterContext twitterCtx)
        {
            WelcomeMessage message =
                await twitterCtx.NewWelcomeMessageAsync(
                    "New Welcome Message",
                    "Welcome!");

            //DMEvent dmEvent = message?.Value?.DMEvent;
            //if (dmEvent != null)
            //    Console.WriteLine(
            //        "Recipient: {0}, Message: {1}, Date: {2}",
            //        dmEvent.MessageCreate.Target.RecipientID,
            //        dmEvent.MessageCreate.MessageData.Text,
            //        dmEvent.CreatedTimestamp);
        }
    }
}
