using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
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
                        Console.WriteLine("\n\tHiding a reply...\n");
                        await HideReplyAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tUn-Hiding a reply...\n");
                        await UnHideReplyAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tGetting the Mentions Timeline...\n");
                        await GetMentionsTimelineAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tGetting the Tweets Timeline...\n");
                        await GetTweetsTimelineAsync(twitterCtx);
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
            Console.WriteLine("\t 2. Hide a Reply");
            Console.WriteLine("\t 3. Un-Hide a Reply");
            Console.WriteLine("\t 4. Mentions Timeline");
            Console.WriteLine("\t 5. Tweets Timeline");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task DoSingleTweetLookupAsync(TwitterContext twitterCtx)
        {
            const string TweetID = "1343794019927658496";

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
                       tweet.MediaFields == MediaField.AllFields.Replace(",promoted_metrics", "") &&
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
            const string TweetIds = "1441239829472165900,1305895383260782593,1322227053161148422";

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

        static async Task HideReplyAsync(TwitterContext twitterCtx)
        {
            const string TweetID = "1327749647515881473";

            TweetHideResponse? hideResp = await twitterCtx.HideReplyAsync(TweetID);

            Console.WriteLine($"Is Hidden: {hideResp?.Data?.Hidden}");
        }

        static async Task UnHideReplyAsync(TwitterContext twitterCtx)
        {
            const string TweetID = "1327749647515881473";

            TweetHideResponse? hideResp = await twitterCtx.UnhideReplyAsync(TweetID);

            Console.WriteLine($"Is Hidden: {hideResp?.Data?.Hidden}");
        }

        static async Task GetMentionsTimelineAsync(TwitterContext twitterCtx)
        {
            string userID = "15411837";

            TweetQuery? tweetResponse =
                await
                (from tweet in twitterCtx.Tweets
                 where tweet.Type == TweetType.MentionsTimeline &&
                       tweet.ID == userID
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

        static async Task GetTweetsTimelineAsync(TwitterContext twitterCtx)
        {
            string userID = "15411837";

            TweetQuery? tweetResponse =
                await
                (from tweet in twitterCtx.Tweets
                 where tweet.Type == TweetType.TweetsTimeline &&
                       tweet.ID == userID
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
    }
}
