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
                        Console.WriteLine("\nShowing Welcome Message...\n");
                        await ShowWelcomeMessageAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\nListing Welcome Messages...\n");
                        await ListWelcomeMessagesAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tDeleting Welcome Message...\n");
                        await DeleteWelcomeMessageAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tCreating Welcome Message Rule...\n");
                        await CreateNewWelcomeMessageRuleAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\nShowing Welcome Message Rules...\n");
                        await ShowWelcomeMessageRuleAsync(twitterCtx);
                        break;
                    case '7':
                        Console.WriteLine("\nListing Welcome Message Rules...\n");
                        await ListWelcomeMessageRulesAsync(twitterCtx);
                        break;
                    case '8':
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
            Console.WriteLine("\t 2. Show a Welcome Message");
            Console.WriteLine("\t 3. List Welcome Messages");
            Console.WriteLine("\t 4. Delete a Welcome Message");
            Console.WriteLine("\t 5. Create a New Welcome Message Rule");
            Console.WriteLine("\t 6. Show a Welcome Message Rule");
            Console.WriteLine("\t 7. List Welcome Message Rules");
            Console.WriteLine("\t 8. Delete a Welcome Message Rule");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task CreateNewWelcomeMessageAsync(TwitterContext twitterCtx)
        {
            WelcomeMessage message =
                await twitterCtx.NewWelcomeMessageAsync(
                    "New Welcome Message",
                    "Welcome!");

            WelcomeMsg? msg = message?.Value?.WelcomeMessage;
            if (msg != null)
            {
                Console.WriteLine(
                    $"Message ID: '{msg.Id}' \n" +
                    $"Message Name: '{msg.Name} \n" +
                    $"Message Text: '{msg.MessageData?.Text}\n");
            }
        }

        static async Task UpdateWelcomeMessageAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please type welcome message ID: ");
            string? respone = Console.ReadLine();
            ulong.TryParse(respone, out ulong wecomeMessageID);

            WelcomeMessage message =
                await twitterCtx.UpdateWelcomeMessageAsync(
                    wecomeMessageID,
                    "New Name",
                    "Welcome to LINQ to Twitter!");

            WelcomeMsg? msg = message?.Value?.WelcomeMessage;
            if (msg != null)
            {
                Console.WriteLine(
                    $"Message ID: '{msg.Id}' \n" +
                    $"Message Name: '{msg.Name} \n" +
                    $"Message Text: '{msg.MessageData?.Text}\n");
            }
        }

        static async Task ShowWelcomeMessageAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please type welcome message ID: ");
            string? respone = Console.ReadLine();
            ulong.TryParse(respone, out ulong wecomeMessageID);

            WelcomeMessage? message =
                await
                (from welcomeMsg in twitterCtx.WelcomeMessage
                 where welcomeMsg.Type == WelcomeMessageType.ShowMessage &&
                       welcomeMsg.ID == wecomeMessageID
                 select welcomeMsg)
                .SingleOrDefaultAsync();

            WelcomeMsg? msg = message?.Value?.WelcomeMessage;
            if (msg != null)
            {
                Console.WriteLine(
                    $"Message ID: '{msg.Id}' \n" +
                    $"Message Name: '{msg.Name} \n" +
                    $"Message Text: '{msg.MessageData?.Text}\n");
            }
        }

        static async Task ListWelcomeMessagesAsync(TwitterContext twitterCtx)
        {
            int count = 10; // set to a low number to demo paging
            string? cursor = null;
            List<WelcomeMsg> allWelcomeMessages = new List<WelcomeMsg>();

            // you don't have a valid cursor until after the first query
            WelcomeMessage? message =
                await
                (from welcomeMsg in twitterCtx.WelcomeMessage
                 where welcomeMsg.Type == WelcomeMessageType.ListMessages &&
                       welcomeMsg.Count == count
                 select welcomeMsg)
                .SingleOrDefaultAsync();

            SetMessagesAndCursor(cursor, allWelcomeMessages, message);

            while (!string.IsNullOrWhiteSpace(cursor))
            {
                message =
                    await
                    (from welcomeMsg in twitterCtx.WelcomeMessage
                     where welcomeMsg.Type == WelcomeMessageType.ListMessages &&
                           welcomeMsg.Count == count &&
                           welcomeMsg.Cursor == cursor
                     select welcomeMsg)
                    .SingleOrDefaultAsync();

                SetMessagesAndCursor(cursor, allWelcomeMessages, message);
            }

            if (!allWelcomeMessages.Any())
            {
                Console.WriteLine("No items returned");
                return;
            }

            Console.WriteLine($"Response Count: {allWelcomeMessages.Count}");
            Console.WriteLine("Responses:\n");

            allWelcomeMessages.ForEach(msg =>
            {
                if (msg != null)
                {
                    Console.WriteLine(
                        $"Message ID: '{msg.Id}' \n" +
                        $"      Name: '{msg.Name} \n" +
                        $"      Text: '{msg.MessageData?.Text}\n");
                }
            });

            static void SetMessagesAndCursor(string? cursor, List<WelcomeMsg> allWelcomeMessages, WelcomeMessage? message)
            {
                if (message?.Value?.WelcomeMessages != null)
                {
                    allWelcomeMessages.AddRange(message.Value.WelcomeMessages);
                    cursor = message.Value?.NextCursor;
                }
            }
        }

        static async Task DeleteWelcomeMessageAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please type welcome message ID: ");
            string? respone = Console.ReadLine();
            ulong.TryParse(respone, out ulong wecomeMessageID);

            await twitterCtx.DeleteWelcomeMessageAsync(wecomeMessageID);

            Console.WriteLine("Message deleted.");
        }

        static async Task CreateNewWelcomeMessageRuleAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please type welcome message ID to set as default: ");
            string? respone = Console.ReadLine();
            ulong.TryParse(respone, out ulong wecomeMessageID);

            WelcomeMessage welcomeMsg =
                await twitterCtx.NewWelcomeMessageRuleAsync(wecomeMessageID);

            WelcomeMessageRule? rule = welcomeMsg?.Value?.WelcomeMessageRule;
            if (rule != null)
                Console.WriteLine(
                    $"Rule ID '{rule.ID}' " +
                    $"for welcome message ID: '{rule.WelcomeMessageID}' " +
                    $"set as default on {rule.CreatedAt}");
        }

        static async Task ShowWelcomeMessageRuleAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please type welcome message rule ID: ");
            string? respone = Console.ReadLine();
            ulong.TryParse(respone, out ulong wecomeMessageID);

            WelcomeMessage? message =
                await
                (from welcomeMsg in twitterCtx.WelcomeMessage
                 where welcomeMsg.Type == WelcomeMessageType.ShowRule &&
                       welcomeMsg.ID == wecomeMessageID
                 select welcomeMsg)
                .SingleOrDefaultAsync();

            WelcomeMessageRule? rule = message?.Value?.WelcomeMessageRule;
            if (rule != null)
            {
                Console.WriteLine(
                    $"Rule ID '{rule.ID}' \n" +
                    $"for welcome message ID: '{rule.WelcomeMessageID}' \n" +
                    $"set as default on {rule.CreatedAt}\n");
            }
        }

        static async Task ListWelcomeMessageRulesAsync(TwitterContext twitterCtx)
        {
            int count = 5; // set to a low number to demo paging
            string cursor = "";
            List<WelcomeMessageRule> allWelcomeMessageRules = new List<WelcomeMessageRule>();

            // you don't have a valid cursor until after the first query
            WelcomeMessage? message =
                await
                (from welcomeMsg in twitterCtx.WelcomeMessage
                 where welcomeMsg.Type == WelcomeMessageType.ListRules &&
                       welcomeMsg.Count == count
                 select welcomeMsg)
                .SingleOrDefaultAsync();

            cursor = SetMessageRules(allWelcomeMessageRules, message);

            while (!string.IsNullOrWhiteSpace(cursor))
            {
                message =
                    await
                    (from welcomeMsg in twitterCtx.WelcomeMessage
                     where welcomeMsg.Type == WelcomeMessageType.ListRules &&
                           welcomeMsg.Count == count &&
                           welcomeMsg.Cursor == cursor
                     select welcomeMsg)
                    .SingleOrDefaultAsync();

                cursor = SetMessageRules(allWelcomeMessageRules, message);
            }

            if (!allWelcomeMessageRules.Any())
            {
                Console.WriteLine("No items returned");
                return;
            }

            Console.WriteLine($"Response Count: {allWelcomeMessageRules.Count}");
            Console.WriteLine("Responses:\n");

            allWelcomeMessageRules.ForEach(rule =>
            {
                if (rule != null)
                {
                    Console.WriteLine(
                        $"Rule ID '{rule.ID}' \n" +
                        $"for welcome message ID: '{rule.WelcomeMessageID}' \n" +
                        $"set as default on {rule.CreatedAt}\n");
                }
            });

            static string SetMessageRules(List<WelcomeMessageRule> allWelcomeMessageRules, WelcomeMessage? message)
            {
                string cursor;
                List<WelcomeMessageRule> rules = message?.Value?.WelcomeMessageRules ?? new List<WelcomeMessageRule>();
                allWelcomeMessageRules.AddRange(rules);
                cursor = message?.Value?.NextCursor ?? "";
                return cursor;
            }
        }

        static async Task DeleteWelcomeMessageRuleAsync(TwitterContext twitterCtx)
        {
            Console.Write("Please type welcome message rule ID: ");
            string? respone = Console.ReadLine();
            ulong.TryParse(respone, out ulong wecomeMessageRuleID);

            await twitterCtx.DeleteWelcomeMessageRuleAsync(wecomeMessageRuleID);

            Console.WriteLine("Message rule deleted.");
        }
    }
}
