using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class AccountActivityDemos
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
                        Console.WriteLine("\n\tAdding Webhook...\n");
                        await AddWebhookAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tListing Webhooks....\n");
                        await ListWebhooksAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tDeleting Webhook...\n");
                        await DeleteWebhookAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tSending Challenge Response Check...\n");
                        await SendChallengeResponseCheckAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tAdding subscription...\n");
                        await AddSubscriptionAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tShowing subscriptions...\n");
                        await ShowSubscriptionsAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\n\tDeleting subscription...\n");
                        await DeleteSubscriptionAsync(twitterCtx);
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
            Console.WriteLine("\nAccount Demos - Please select:\n");

            Console.WriteLine("\t 0. Add Webhook");
            Console.WriteLine("\t 1. List Webhooks");
            Console.WriteLine("\t 2. Delete Webhook");
            Console.WriteLine("\t 3. Send Challenge Response Check");
            Console.WriteLine("\t 4. Add Subscription");
            Console.WriteLine("\t 5. List Subscriptions");
            Console.WriteLine("\t 6. Delete Subscription");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task AddWebhookAsync(TwitterContext twitterCtx)
        {
            try
            {
                Console.Write("What is the Webhook URL? ");
                string url = Console.ReadLine();

                AccountActivity accAct = await twitterCtx.AddAccountActivityWebhookAsync(url);

                Webhook webhook = accAct.WebhooksValue.Webhooks.SingleOrDefault();
                Console.WriteLine(
                    $"Webhook for '{webhook.Url}' " +
                    $"added with ID: {webhook.ID}, " +
                    $"created at {webhook.CreatedTimestamp}");
            }
            catch (TwitterQueryException tqe)
            {
                Console.WriteLine(tqe.Message);
            }
        }

        static async Task ListWebhooksAsync(TwitterContext twitterCtx)
        {
            var webhooksResponse =
                await
                (from acct in twitterCtx.AccountActivity
                 where acct.Type == AccountActivityType.Webhooks
                 select acct)
                .SingleOrDefaultAsync();

            if (webhooksResponse?.WebhooksValue?.Webhooks != null)
            {
                Console.WriteLine("Webhooks:");

                if (webhooksResponse.WebhooksValue.Webhooks.Any())
                    foreach (var webhook in webhooksResponse.WebhooksValue.Webhooks)
                        Console.WriteLine(
                            $"ID: {webhook.ID}, " +
                            $"Created: {webhook.CreatedTimestamp}, " +
                            $"Valid: {webhook.Valid}, " +
                            $"URL: {webhook.Url}");
                else
                    Console.WriteLine("No webhooks registered");
            }
        }

        static async Task DeleteWebhookAsync(TwitterContext twitterCtx)
        {
            ulong webhookID = GetWebhook();

            var acctActivity = await twitterCtx.DeleteAccountActivityWebhookAsync(webhookID);

            Console.WriteLine($"Webhook, {acctActivity.WebhookID}, has been deleted.");
        }

        static async Task SendChallengeResponseCheckAsync(TwitterContext twitterCtx)
        {
            ulong webhookID = GetWebhook();

            try
            {
                AccountActivity accAct = await twitterCtx.SendAccountActivityCrcAsync(webhookID);

                Console.WriteLine("Challenge response check succeeded.");
            }
            catch (TwitterQueryException tqEx) // Twitter returns a 3XX when it can't delete, which throws
            {
                Console.WriteLine($"Challenge response check failed: {tqEx.Message}");
            }
        }

        static async Task AddSubscriptionAsync(TwitterContext twitterCtx)
        {
            ulong webhookID = GetWebhook();

            try
            {
                AccountActivity accAct = await twitterCtx.AddAccountActivitySubscriptionAsync(webhookID);

                Console.WriteLine("Subscription added.");
            }
            catch (TwitterQueryException tqEx) // Twitter returns a 3XX or 4XX when it can't add, which throws
            {
                Console.WriteLine($"Unable to add subscription: {tqEx.Message}");
            }
        }

        static async Task ShowSubscriptionsAsync(TwitterContext twitterCtx)
        {
            ulong webhookID = GetWebhook();

            bool isSubscribed = false;

            try
            {
                var accAct =
                    await
                    (from act in twitterCtx.AccountActivity
                     where act.Type == AccountActivityType.Subscriptions &&
                           act.WebhookID == webhookID
                     select act)
                    .SingleOrDefaultAsync();

                isSubscribed = accAct?.SubscriptionValue?.IsSubscribed ?? false;

                Console.WriteLine($"IsSubscribed: {isSubscribed}");
            }
            catch (TwitterQueryException) // Twitter returns a 4XX when not subscribed, which throws
            {
                Console.WriteLine("Your user account is not subscribed.");
            }
        }

        static async Task DeleteSubscriptionAsync(TwitterContext twitterCtx)
        {
            ulong webhookID = GetWebhook();

            try
            {
                AccountActivity accAct = await twitterCtx.DeleteAccountActivitySubscriptionAsync(webhookID);

                Console.WriteLine("Subscription deleted.");
            }
            catch (TwitterQueryException tqEx) // Twitter returns a 3XX when it can't delete, which throws
            {
                Console.WriteLine($"Unable to delete subscription: {tqEx.Message}");
            }
        }

        /// <summary>
        /// Asks user for a webhook
        /// </summary>
        /// <remarks>
        /// Tip: Do AccountActivtyType.Webhooks query for a list of webhooks.
        /// </remarks>
        /// <returns>Webhook as ulong</returns>
        static ulong GetWebhook()
        {
            Console.Write("Webhook ID? ");
            string webhookIDString = Console.ReadLine();
            ulong.TryParse(webhookIDString, out ulong webhookID);
            return webhookID;
        }
    }
}
