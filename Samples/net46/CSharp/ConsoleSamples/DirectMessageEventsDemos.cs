using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Collections.Generic;
using System.IO;

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
                    case '3':
                        Console.WriteLine("\n\tDeleting DM...\n");
                        await DeleteDirectMessageAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tSending DM with media...\n");
                        await NewDirectMessageWithMediaAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tSending DM with media...\n");
                        await NewDirectMessageWithCoordinatesAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\n\tSending DM with media...\n");
                        await NewDirectMessageWithPlaceAsync(twitterCtx);
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
            Console.WriteLine("\t 3. Delete Direct Message");
            Console.WriteLine("\t 4. Send Direct Message with Media");
            Console.WriteLine("\t 5. Send Direct Message with Coordinates");
            Console.WriteLine("\t 6. Send Direct Message with Place");
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

            DirectMessageCreate msgCreate = dmResponse?.Value?.DMEvent?.MessageCreate;

            if (dmResponse != null && msgCreate != null)
                Console.WriteLine(
                    "From ID: {0}\nTo ID:  {1}\nMessage Text: {2}",
                    msgCreate.SenderID ?? "None",
                    msgCreate.Target.RecipientID ?? "None",
                    msgCreate.MessageData.Text ?? "None");
        }

        static async Task ListDirectMessagesAsync(TwitterContext twitterCtx)
        {
            int count = 50; // set to a low number to demo paging
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
                DirectMessageCreate msgCreate = evt.MessageCreate;

                if (evt != null && msgCreate != null)
                    Console.WriteLine(
                        $"DM ID: {evt.ID}\n" +
                        $"From ID: {msgCreate.SenderID ?? "None"}\n" +
                        $"To ID:  {msgCreate.Target?.RecipientID ?? "None"}\n" +
                        $"Message Text: {msgCreate.MessageData?.Text ?? "None"}");
            });
        }

        static async Task NewDirectMessageAsync(TwitterContext twitterCtx)
        {
            const ulong Linq2TwitrID = 15411837;// 16761255;

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

        static async Task DeleteDirectMessageAsync(TwitterContext twitterCtx)
        {
            Console.Write("Which DM would you like to delete? (please enter DM ID): ");
            string dmInput = Console.ReadLine();

            ulong.TryParse(dmInput, out ulong dmID);

            try
            {
                await twitterCtx.DeleteDirectMessageEventAsync(dmID);
                Console.WriteLine("\nDM Deleted");
            }
            catch (TwitterQueryException tqEx)
            {
                Console.WriteLine($"\nProblem deleting DM: ({tqEx.ErrorCode}) - {tqEx.ReasonPhrase}");
            }
        }
        static async Task NewDirectMessageWithMediaAsync(TwitterContext twitterCtx)
        {
            const ulong Linq2TwitrID = 15411837;// 16761255;
            string mediaCategory = "dm_image";

            Media media = await twitterCtx.UploadMediaAsync(
                File.ReadAllBytes(@"..\..\images\200xColor_2.png"), 
                mediaType: "image/png", 
                additionalOwners: null, 
                mediaCategory: mediaCategory,
                shared: true);

            DirectMessageEvents message =
                await twitterCtx.NewDirectMessageEventAsync(
                    Linq2TwitrID,
                    "DM from @JoeMayo to @Linq2Twitr of $MSFT & $TSLA with #TwitterAPI #chatbot " +
                    "at http://bit.ly/2xSJWJk and http://amzn.to/2gD09X6 on " + DateTime.Now + "!'",
                    media.MediaID);

            DMEvent dmEvent = message?.Value?.DMEvent;
            if (dmEvent != null)
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}, Date: {2}",
                    dmEvent.MessageCreate.Target.RecipientID,
                    dmEvent.MessageCreate.MessageData.Text,
                    dmEvent.CreatedTimestamp);
        }

        static async Task NewDirectMessageWithCoordinatesAsync(TwitterContext twitterCtx)
        {
            const ulong Linq2TwitrID = 15411837;// 16761255;

            DirectMessageEvents message =
                await twitterCtx.NewDirectMessageEventAsync(
                    Linq2TwitrID,
                    "DM from @JoeMayo to @Linq2Twitr of $MSFT & $TSLA with #TwitterAPI #chatbot " +
                    "at http://bit.ly/2xSJWJk and http://amzn.to/2gD09X6 on " + DateTime.Now + "!'",
                    latitude: -122.443893,
                    longitude: 37.771718);

            DMEvent dmEvent = message?.Value?.DMEvent;
            if (dmEvent != null)
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}, Date: {2}",
                    dmEvent.MessageCreate.Target.RecipientID,
                    dmEvent.MessageCreate.MessageData.Text,
                    dmEvent.CreatedTimestamp);
        }

        static async Task NewDirectMessageWithPlaceAsync(TwitterContext twitterCtx)
        {
            const ulong Linq2TwitrID = 15411837;// 16761255;

            DirectMessageEvents message =
                await twitterCtx.NewDirectMessageEventAsync(
                    Linq2TwitrID,
                    "DM from @JoeMayo to @Linq2Twitr of $MSFT & $TSLA with #TwitterAPI #chatbot " +
                    "at http://bit.ly/2xSJWJk and http://amzn.to/2gD09X6 on " + DateTime.Now + "!'",
                    placeID: "5a110d312052166f");

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
