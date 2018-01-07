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
                    case '1':
                        Console.WriteLine("\n\tUpdating Welcome Message...\n");
                        await UpdateWelcomeMessageAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tDeleting Welcome Message...\n");
                        await DeleteWelcomeMessageAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tCreating Welcome Message Rule...\n");
                        await CreateNewWelcomeMessageRuleAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tDeleting Welcome Message Rule...\n");
                        await DeleteWelcomeMessageRuleAsync(twitterCtx);
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
            Console.WriteLine("\t 1. Update a Welcome Message");
            Console.WriteLine("\t 2. Delete a Welcome Message");
            Console.WriteLine("\t 3. Create a New Welcome Message Rule");
            Console.WriteLine("\t 4. Delete a Welcome Message Rule");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task CreateNewWelcomeMessageAsync(TwitterContext twitterCtx)
        {
            WelcomeMessage message =
                await twitterCtx.NewWelcomeMessageAsync(
                    "New Welcome Message",
                    "Welcome!");

            WelcomeMsg msg = message?.Value?.WelcomeMessage;
            if (msg != null)
            {
                Console.WriteLine(
                    $"Message ID: '{msg.Id}' \n" +
                    $"Message Name: '{msg.Name} \n" +
                    $"Message Text: '{msg.MessageData.Text}\n");
            }
        }

        static async Task UpdateWelcomeMessageAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please type welcome message ID: ");
            string respone = Console.ReadLine();
            ulong.TryParse(respone, out ulong wecomeMessageID);

            WelcomeMessage message =
                await twitterCtx.UpdateWelcomeMessageAsync(
                    wecomeMessageID,
                    "New Name",
                    "Welcome to LINQ to Twitter!");

            WelcomeMsg msg = message?.Value?.WelcomeMessage;
            if (msg != null)
            {
                Console.WriteLine(
                    $"Message ID: '{msg.Id}' \n" +
                    $"Message Name: '{msg.Name} \n" +
                    $"Message Text: '{msg.MessageData.Text}\n");
            }
        }

        static async Task DeleteWelcomeMessageAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please type welcome message ID: ");
            string respone = Console.ReadLine();
            ulong.TryParse(respone, out ulong wecomeMessageID);

            await twitterCtx.DeleteWelcomeMessageAsync(wecomeMessageID);

            Console.WriteLine("Message deleted.");
        }

        static async Task CreateNewWelcomeMessageRuleAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please type welcome message ID to set as default: ");
            string respone = Console.ReadLine();
            ulong.TryParse(respone, out ulong wecomeMessageID);

            WelcomeMessage welcomeMsg =
                await twitterCtx.NewWelcomeMessageRuleAsync(wecomeMessageID);

            WelcomeMessageRule rule = welcomeMsg?.Value?.WelcomeMessageRule;
            if (rule != null)
                Console.WriteLine(
                    $"Rule ID '{rule.ID}' " +
                    $"for welcome message ID: '{rule.WelcomeMessageID}' " +
                    $"set as default on {rule.CreatedAt}");
        }

        static async Task DeleteWelcomeMessageRuleAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please type welcome message rule ID: ");
            string respone = Console.ReadLine();
            ulong.TryParse(respone, out ulong wecomeMessageRuleID);

            await twitterCtx.DeleteWelcomeMessageRuleAsync(wecomeMessageRuleID);

            Console.WriteLine("Message rule deleted.");
        }
    }
}
