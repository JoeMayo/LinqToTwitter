using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Diagnostics;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
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
                        Console.WriteLine("\n\tShowing retweets...\n");
                        await RetweetsOfMeStatusQueryAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tShowing retweets...\n");
                        await RetweetsQueryAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tRetweeting...\n");
                        await RetweetAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tGetting oembed...\n");
                        await OEmbedStatusAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tGetting retweeters...\n");
                        await RetweetersAsync(twitterCtx);
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

            Console.WriteLine("\t 0. Retweets of Me Timeline");
            Console.WriteLine("\t 1. Retweets of a Tweet");
            Console.WriteLine("\t 2. Retweet a Tweet");
            Console.WriteLine("\t 3. Get Oembed Tweet");
            Console.WriteLine("\t 4. Get Retweeters");

            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static void PrintTweetsResults(List<Status> tweets)
        {
            if (tweets != null)
                tweets.ForEach(tweet =>
                {
                    if (tweet != null && tweet.User != null)
                        Console.WriteLine(
                            "ID: [{0}] Name: {1}\n\tTweet: {2}",
                            tweet.StatusID, tweet.User.ScreenNameResponse,
                            string.IsNullOrWhiteSpace(tweet.Text) ? tweet.FullText : tweet.Text);
                });
        }

        static async Task RetweetsOfMeStatusQueryAsync(TwitterContext twitterCtx)
        {
            List<Status> myRetweets =
                await
                (from retweet in twitterCtx.Status
                 where retweet.Type == StatusType.RetweetsOfMe &&
                       retweet.Count == 100
                 select retweet)
                .ToListAsync();

            PrintTweetsResults(myRetweets);
        }

        static async Task RetweetsQueryAsync(TwitterContext twitterCtx)
        {
            ulong tweetID = 806571633754284032;

            List<Status> retweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Retweets &&
                       tweet.ID == tweetID
                 select tweet)
                .ToListAsync();

            if (retweets != null)
                retweets.ForEach(tweet =>
                {
                    if (tweet != null && tweet.User != null)
                        Console.WriteLine(
                            "@{0} {1} ({2})",
                            tweet.User.ScreenNameResponse,
                            tweet.Text,
                            tweet.RetweetCount);
                });
        }

        static async Task RetweetAsync(TwitterContext twitterCtx)
        {
            ulong tweetID = 1250088275861049345;

            Status? retweet = await twitterCtx.RetweetAsync(tweetID);

            if (retweet != null &&
                retweet.RetweetedStatus != null &&
                retweet.RetweetedStatus.User != null)
            {
                Console.WriteLine("Retweeted Tweet: ");
                Console.WriteLine(
                    "\nUser: " + retweet.RetweetedStatus.User.ScreenNameResponse +
                    "\nTweet: " + retweet.RetweetedStatus.Text +
                    "\nTweet ID: " + retweet.RetweetedStatus.ID + "\n");
            }
        }

        static async Task OEmbedStatusAsync(TwitterContext twitterCtx)
        {
            string url = "https://twitter.com/JoeMayo/status/1450247082019672066";

            EmbeddedStatus? embeddedStatus =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Oembed &&
                       tweet.OEmbedUrl == url
                 select tweet.EmbeddedStatus)
                .SingleOrDefaultAsync();

            if (embeddedStatus != null)
                Console.WriteLine(
                    "Embedded Status Html: \n\n" + embeddedStatus.Html);
        }

        static async Task RetweetersAsync(TwitterContext twitterCtx)
        {
            ulong tweetID = 210591841312190464;

            Status? status =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Retweeters &&
                       tweet.ID == tweetID
                 select tweet)
                .SingleOrDefaultAsync();

            if (status != null && status.User != null)
                status.Users?.ForEach(
                    userID => Console.WriteLine("User ID: " + userID));
        }
    }
}
