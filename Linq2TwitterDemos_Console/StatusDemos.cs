using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class StatusDemos
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
                        Console.WriteLine("\n\tTweeting...\n");
                        await DoTweetAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tShowing home timeline...\n");
                        await RunHomeTimelineQueryAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tShowing user timeline...\n");
                        await RunUserTimelineQueryAsync(twitterCtx);
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
            Console.WriteLine("\nStatus Demos - Please select:\n");

            Console.WriteLine("\t 0. Update Status");
            Console.WriteLine("\t 1. Home Timeline");
            Console.WriteLine("\t 2. User Timeline");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }

        static void PrintTweetsResults(List<Status> tweets)
        {
            int i = 0;
            tweets.ForEach(
                tweet => Console.WriteLine(
                "Name: {0}, Tweet: {1}",
                tweet.User.ScreenNameResponse, tweet.Text));

            var tweetey = tweets.FirstOrDefault();

            if (tweetey == null)
            {
                Console.WriteLine("No results.");
            }
            else if (tweetey.Entities.HashTagEntities.Count == 1)
            {
                Console.WriteLine("Hashtag entity: {0}", tweetey.Entities.HashTagEntities[i].Tag);
            }
        }

        static async Task DoTweetAsync(TwitterContext twitterCtx)
        {
            Console.Write("Enter your status update: ");
            string status = Console.ReadLine();

            Console.WriteLine("\nStatus being sent: \n\n\"{0}\"", status);
            Console.Write("\nDo you want to update your status? (y or n): ");
            string confirm = Console.ReadLine();

            if (confirm.ToUpper() == "N")
            {
                Console.WriteLine("\nThis status is *not* being sent.");
            }
            else if (confirm.ToUpper() == "Y")
            {
                Console.WriteLine("\nPress any key to post tweet...\n");
                Console.ReadKey(true);

                var tweet = await twitterCtx.TweetAsync(status);

                Console.WriteLine(
                    "Status returned: " +
                    "(" + tweet.StatusID + ")" +
                    tweet.User.Name + ", " +
                    tweet.Text + "\n");
            }
            else
            {
                Console.WriteLine("Not a valid entry.");
            }
        }

        static async Task RunHomeTimelineQueryAsync(TwitterContext twitterCtx)
        {
            var tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Home
                 select tweet)
                .ToListAsync();

            PrintTweetsResults(tweets);
        }

        static async Task RunUserTimelineQueryAsync(TwitterContext twitterCtx)
        {
            var tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.User &&
                       tweet.ScreenName == "JoeMayo"
                 select tweet)
                .ToListAsync();

            PrintTweetsResults(tweets);
        }
    }
}
