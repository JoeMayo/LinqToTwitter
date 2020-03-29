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
                        await ShowSentDMsAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tShowing received DMs...\n");
                        await ShowReceivedDMsAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tShowing DM...\n");
                        await ShowSpecificDMAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tSending DM...\n");
                        await NewDirectMessageAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tShowing DM...\n");
                        await DestroyDirectMessageAsync(twitterCtx);
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

            Console.WriteLine("\t 0. Sent DMs");
            Console.WriteLine("\t 1. Received DMs");
            Console.WriteLine("\t 2. Show DM");
            Console.WriteLine("\t 3. Send DM");
            Console.WriteLine("\t 4. Delete DM");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }

        static async Task ShowSentDMsAsync(TwitterContext twitterCtx)
        {
            var dmResponse =
                await
                    (from dm in twitterCtx.DirectMessage
                     where dm.Type == DirectMessageType.SentBy
                     select dm)
                    .ToListAsync();

            if (dmResponse != null)
                dmResponse.ForEach(dm => 
                {
                    if (dm != null && dm.Recipient != null)
                        Console.WriteLine(
                            "Name: {0}, Tweet: {1}",
                            dm.Recipient.ScreenNameResponse, dm.Text);
                });
        }

        static async Task ShowReceivedDMsAsync(TwitterContext twitterCtx)
        {
            var dmResponse =
                await
                    (from dm in twitterCtx.DirectMessage
                     where dm.Type == DirectMessageType.SentTo
                     select dm)
                    .ToListAsync();

            if (dmResponse != null)
                dmResponse.ForEach(dm => 
                {
                    if (dm != null && dm.Recipient != null)
                        Console.WriteLine(
                            "Name: {0}, Tweet: {1}",
                            dm.Recipient.ScreenNameResponse, dm.Text);
                });
        }

        static async Task ShowSpecificDMAsync(TwitterContext twitterCtx)
        {
            var dmResponse =
                await
                    (from dm in twitterCtx.DirectMessage
                     where dm.Type == DirectMessageType.Show &&
                           dm.ID == 2078013265
                     select dm)
                    .SingleOrDefaultAsync();

            if (dmResponse != null &&
                dmResponse.Recipient != null &&
                dmResponse.Sender != null)
            {
                Console.WriteLine(
                    "From: {0}\nTo:  {1}\nMessage: {2}",
                    dmResponse.Sender.Name,
                    dmResponse.Recipient.Name,
                    dmResponse.Text);
            }
        }

        static async Task DestroyDirectMessageAsync(TwitterContext twitterCtx)
        {
            var message = 
                await twitterCtx.DestroyDirectMessageAsync(
                    243563161037455360ul, true);

            if (message != null)
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}",
                    message.RecipientScreenName,
                    message.Text);
        }

        static async Task NewDirectMessageAsync(TwitterContext twitterCtx)
        {
            var message = await twitterCtx.NewDirectMessageAsync(
                "Linq2Twitr", "Direct Message Test - " + DateTime.Now + "!'");

            if (message != null)
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}, Date: {2}",
                    message.RecipientScreenName,
                    message.Text,
                    message.CreatedAt);
        }
    }
}
