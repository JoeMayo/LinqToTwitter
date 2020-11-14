using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Collections.Generic;
using System.Diagnostics;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
{
    public class TweetDemos
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
                        Console.WriteLine("\n\tLooking up a single tweet...\n");
                        await DoSingleTweetLookupAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tLooking up multiple tweets...\n");
                        await DoMultiTweetLookupAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tHiding a tweet...\n");
                        await HideTweetAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tUn-Hiding a tweet...\n");
                        await UnHideTweetAsync(twitterCtx);
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
            Console.WriteLine("\nSearch Demos - Please select:\n");

            Console.WriteLine("\t 0. Single Tweet Lookup");
            Console.WriteLine("\t 1. Multi-Tweet Lookup");
            Console.WriteLine("\t 2. Hide a Tweet");
            Console.WriteLine("\t 3. Un-Hide a Tweet");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task DoSingleTweetLookupAsync(TwitterContext twitterCtx)
        {
            const string TweetID = "1305895383260782593";

            // default is id and text and this also brings in created_at and geo
            string tweetFields =
                string.Join(",",
                    new string[]
                    {
                        TweetField.CreatedAt,
                        TweetField.ID,
                        TweetField.Text,
                        TweetField.Geo
                    });

            TweetQuery? tweetResponse =
                await
                (from tweet in twitterCtx.Tweets
                 where tweet.Type == TweetType.Lookup &&
                       tweet.Ids == TweetID &&
                       tweet.TweetFields == TweetField.AllFieldsExceptPermissioned &&
                       tweet.Expansions == ExpansionField.AllTweetFields &&
                       tweet.MediaFields == MediaField.AllFieldsExceptPermissioned &&
                       tweet.PlaceFields == PlaceField.AllFields &&
                       tweet.PollFields == PollField.AllFields &&
                       tweet.UserFields == UserField.AllFields
                 select tweet)
                .SingleOrDefaultAsync();

            if (tweetResponse?.Tweets != null)
                tweetResponse.Tweets.ForEach(tweet =>
                    Console.WriteLine(
                        $"\nUser: {tweet.ID}" +
                        $"\nTweet: {tweet.Text}"));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task DoMultiTweetLookupAsync(TwitterContext twitterCtx)
        {
            const string TweetIds = "1305895383260782593,1322227053161148422";

            // default is id and text and this also brings in created_at and geo
            string tweetFields =
                string.Join(",",
                    new string[]
                    {
                        TweetField.CreatedAt,
                        TweetField.ID,
                        TweetField.Text,
                        TweetField.Geo
                    });

            TweetQuery? tweetResponse =
                await
                (from tweet in twitterCtx.Tweets
                 where tweet.Type == TweetType.Lookup &&
                       tweet.Ids == TweetIds &&
                       tweet.TweetFields == TweetField.AllFieldsExceptPermissioned &&
                       tweet.Expansions == ExpansionField.AllTweetFields &&
                       tweet.MediaFields == MediaField.AllFieldsExceptPermissioned &&
                       tweet.PlaceFields == PlaceField.AllFields &&
                       tweet.PollFields == PollField.AllFields &&
                       tweet.UserFields == UserField.AllFields
                 select tweet)
                .SingleOrDefaultAsync();

            if (tweetResponse?.Tweets != null)
                tweetResponse.Tweets.ForEach(tweet =>
                    Console.WriteLine(
                        $"\nUser: {tweet.ID}" +
                        $"\nTweet: {tweet.Text}"));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task HideTweetAsync(TwitterContext twitterCtx)
        {
            bool isHidden = await twitterCtx.HideTweetAsync("1322667606688423936");

            Console.WriteLine($"Is Hidden: {isHidden}");
        }

        static async Task UnHideTweetAsync(TwitterContext twitterCtx)
        {
            bool isHidden = await twitterCtx.UnHideTweetAsync("1322667606688423936");

            Console.WriteLine($"Is Hidden: {isHidden}");
        }
    }
}
