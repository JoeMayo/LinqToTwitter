using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Collections.Generic;

namespace Linq2TwitterDemos_Console
{
    class DirectMessageEventsDemos
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
                        Console.WriteLine("\n\tShowing DMs...\n");
                        await ShowDirectMessagesAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tListing DMs...\n");
                        await ListDirectMessagesAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tSending DM...\n");
                        await NewDirectMessageAsync(twitterCtx);
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

            Console.WriteLine("\t 0. Show Direct Messages");
            Console.WriteLine("\t 1. List Direct Messages");
            Console.WriteLine("\t 2. Send Direct Message");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task ShowDirectMessagesAsync(TwitterContext twitterCtx)
        {
            DirectMessageEvents dmResponse =
                await
                    (from dm in twitterCtx.DirectMessageEvents
                     where dm.Type == DirectMessageEventsType.Show &&
                           dm.ID == 917929712638246916
                     select dm)
                    .SingleOrDefaultAsync();

            MessageCreate msgCreate = dmResponse?.Value?.DMEvent?.MessageCreate;

            if (dmResponse != null && msgCreate != null)
                Console.WriteLine(
                    "From ID: {0}\nTo ID:  {1}\nMessage Text: {2}",
                    msgCreate.SenderID ?? "None",
                    msgCreate.Target.RecipientID ?? "None",
                    msgCreate.MessageData.Text ?? "None");
        }

        static async Task ListDirectMessagesAsync(TwitterContext twitterCtx)
        {
            int count = 10; // intentionally set to a low number to demo paging
            string cursor = "";
            List<DMEvent> allDmEvents = new List<DMEvent>();

            // you don't have a valid cursor until after the first query
            DirectMessageEvents dmResponse =
                await
                    (from dm in twitterCtx.DirectMessageEvents
                     where dm.Type == DirectMessageEventsType.List &&
                           dm.Count == count
                     select dm)
                    .SingleOrDefaultAsync();

            allDmEvents.AddRange(dmResponse.Value.DMEvents);
            cursor = dmResponse.Value.NextCursor;

            while (!string.IsNullOrWhiteSpace(cursor))
            {
                dmResponse =
                    await
                        (from dm in twitterCtx.DirectMessageEvents
                         where dm.Type == DirectMessageEventsType.List &&
                               dm.Count == count &&
                               dm.Cursor == cursor
                         select dm)
                        .SingleOrDefaultAsync();

                allDmEvents.AddRange(dmResponse.Value.DMEvents);
                cursor = dmResponse.Value.NextCursor;
            }

            if (!allDmEvents.Any())
            {
                Console.WriteLine("No items returned");
                return;
            }

            Console.WriteLine($"Response Count: {allDmEvents.Count}");
            Console.WriteLine("Responses:");

            allDmEvents.ForEach(evt =>
            {
                MessageCreate msgCreate = evt.MessageCreate;

                if (evt != null && msgCreate != null)
                    Console.WriteLine(
                        "From ID: {0}\nTo ID:  {1}\nMessage Text: {2}",
                        msgCreate.SenderID ?? "None",
                        msgCreate.Target?.RecipientID ?? "None",
                        msgCreate.MessageData?.Text ?? "None");
            });
        }

        static async Task NewDirectMessageAsync(TwitterContext twitterCtx)
        {
            const ulong Linq2TwitrID = 16761255;

            DirectMessageEvents message = 
                await twitterCtx.NewDirectMessageEventAsync(
                    Linq2TwitrID, 
                    "DM from @JoeMayo to @Linq2Twitr of $MSFT & $TSLA with #TwitterAPI #chatbot " +
                    "at http://bit.ly/2xSJWJk and http://amzn.to/2gD09X6 on " + DateTime.Now + "!'");

            DMEvent dmEvent = message?.Value?.DMEvent;
            if (dmEvent != null)
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}, Date: {2}",
                    dmEvent.MessageCreate.Target.RecipientID,
                    dmEvent.MessageCreate.MessageData.Text,
                    dmEvent.CreatedTimestamp);
        }
    }
}
