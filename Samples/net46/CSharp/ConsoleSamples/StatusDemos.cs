using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
                        Console.WriteLine("\n\tShowing mentions timeline...");
                        await ShowMentionsTimelineAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tShowing user timeline...\n");
                        await RunUserTimelineQueryAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tShowing home timeline...\n");
                        await RunHomeTimelineQueryAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tShowing retweets...\n");
                        await RetweetsOfMeStatusQueryAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tShowing retweets...\n");
                        await RetweetsQueryAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tShowing tweet...\n");
                        await SingleStatusQueryAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\n\tDeleting tweet...\n");
                        await DeleteTweetAsync(twitterCtx);
                        break;
                    case '7':
                        Console.WriteLine("\n\tTweeting...\n");
                        await TweetAsync(twitterCtx);
                        break;
                    case '8':
                        Console.WriteLine("\n\tReplying...\n");
                        await ReplyAsync(twitterCtx);
                        break;
                    case '9':
                        Console.WriteLine("\n\tRetweeting...\n");
                        await RetweetAsync(twitterCtx);
                        break;
                    case 'a':
                    case 'A':
                        Console.WriteLine("\n\tTweeting images...\n");
                        await UploadMultipleImagesAsync(twitterCtx);
                        break;
                    case 'b':
                    case 'B':
                        Console.WriteLine("\n\tGetting oembed...\n");
                        await OEmbedStatusAsync(twitterCtx);
                        break;
                    case 'c':
                    case 'C':
                        Console.WriteLine("\n\tGetting retweeters...\n");
                        await RetweetersAsync(twitterCtx);
                        break;
                    case 'd':
                    case 'D':
                        Console.WriteLine("\n\tFollowing conversation...\n");
                        await GetConversationAsync(twitterCtx);
                        break;
                    case 'e':
                    case 'E':
                        Console.WriteLine("\n\tLooking up tweets...\n");
                        await LookupTweetsAsyc(twitterCtx);
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

            Console.WriteLine("\t 0. Mentions Timeline");
            Console.WriteLine("\t 1. User Timeline");
            Console.WriteLine("\t 2. Home Timeline");
            Console.WriteLine("\t 3. Retweets of Me Timeline");
            Console.WriteLine("\t 4. Retweets of a Tweet");
            Console.WriteLine("\t 5. Show Specific Tweet");
            Console.WriteLine("\t 6. Delete a Tweet");
            Console.WriteLine("\t 7. Update Status");
            Console.WriteLine("\t 8. Reply to a Tweet");
            Console.WriteLine("\t 9. Retweet a Tweet");
            Console.WriteLine("\t A. Tweet Multiple Images");
            Console.WriteLine("\t B. Get Oembed Tweet");
            Console.WriteLine("\t C. Get Retweeters");
            Console.WriteLine("\t D. Follow Conversation");
            Console.WriteLine("\t E. Lookup Tweets");

            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }

        static void PrintTweetsResults(List<Status> tweets)
        {
            if (tweets != null)
                tweets.ForEach(tweet => 
                {
                    if (tweet != null && tweet.User != null)
                        Console.WriteLine(
                            "ID: [{0}] Name: {1}\n\tTweet: {2}",
                            tweet.StatusID, tweet.User.ScreenNameResponse, tweet.Text);
                });
        }
  
        static async Task ShowMentionsTimelineAsync(TwitterContext twitterCtx)
        {
            var tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Mentions &&
                       tweet.ScreenName == "JoeMayo"
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

        static async Task RetweetsOfMeStatusQueryAsync(TwitterContext twitterCtx)
        {
            var myRetweets =
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
            ulong tweetID = 196991337554378752;

            var publicTweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Retweets &&
                       tweet.ID == tweetID
                 select tweet)
                .ToListAsync();

            if (publicTweets != null)
                publicTweets.ForEach(tweet =>
                {
                    if (tweet != null && tweet.User != null)
                        Console.WriteLine(
                            "@{0} {1} ({2})",
                            tweet.User.ScreenNameResponse,
                            tweet.Text,
                            tweet.RetweetCount);
                });
        }

        static async Task SingleStatusQueryAsync(TwitterContext twitterCtx)
        {
            ulong tweetID = 449660889793581056;

            var friendTweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Show &&
                       tweet.ID == tweetID
                 select tweet)
                .ToListAsync();

            if (friendTweets != null)
            {
                Console.WriteLine("\nTweets: \n");
                friendTweets.ForEach(tweet =>
                {
                    if (tweet != null && tweet.User != null)
                        Console.WriteLine(
                            "User: " + tweet.User.Name +
                            "\nTweet: " + tweet.Text +
                            "\nTweet ID: " + tweet.ID + "\n");
                }); 
            }
        }

        static async Task DeleteTweetAsync(TwitterContext twitterCtx)
        {
            ulong tweetID = 280433519057068033;

            Status status = 
                await twitterCtx.DeleteTweetAsync(tweetID);

            if (status != null && status.User != null)
                Console.WriteLine(
                    "(" + status.StatusID + ")" +
                    "[" + status.User.UserID + "]" +
                    status.User.ScreenNameResponse + ", " +
                    status.Text + ", " +
                    status.CreatedAt);
        }

        static async Task TweetAsync(TwitterContext twitterCtx)
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

                Status tweet = await twitterCtx.TweetAsync(status);

                if (tweet != null)
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

        static async Task ReplyAsync(TwitterContext twitterCtx)
        {
            ulong tweetID = 401033367283453953;
            string status = "@JoeMayo Testing ReplyAsync #Linq2Twitter £ ";

                Status tweet = await twitterCtx.ReplyAsync(tweetID, status);

                if (tweet != null)
                    Console.WriteLine(
                        "Status returned: " +
                        "(" + tweet.StatusID + ")" +
                        tweet.User.Name + ", " +
                        tweet.Text + "\n");
        }

        static async Task RetweetAsync(TwitterContext twitterCtx)
        {
            ulong tweetID = 401033367283453953;

            Status retweet = await twitterCtx.RetweetAsync(tweetID);

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

        static async Task UploadMultipleImagesAsync(TwitterContext twitterCtx)
        {
            string status = 
                "Testing multi-image tweet #Linq2Twitter £ " + 
                DateTime.Now.ToString(CultureInfo.InvariantCulture);

            var imageUploadTasks = 
                new List<Task<Media>> 
                {
                    twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\images\200xColor_2.png")),
                    twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\images\WP_000003.jpg")),
                    twitterCtx.UploadMediaAsync(File.ReadAllBytes(@"..\..\images\13903749474_86bd1290de_o.jpg")),
                };

            await Task.WhenAll(imageUploadTasks);

            var mediaIds =
                (from tsk in imageUploadTasks
                 select tsk.Result.MediaID)
                .ToList();

            Status tweet = await twitterCtx.TweetAsync(status, mediaIds);

            if (tweet != null)
                Console.WriteLine("Tweet sent: " + tweet.Text);
        }

        static async Task OEmbedStatusAsync(TwitterContext twitterCtx)
        {
            ulong tweetID = 305050067973312514;

            var embeddedStatus =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Oembed &&
                       tweet.ID == tweetID
                 select tweet.EmbeddedStatus)
                .SingleOrDefaultAsync();

            if (embeddedStatus != null)
                Console.WriteLine(
                    "Embedded Status Html: \n\n" + embeddedStatus.Html);
        }

        static async Task RetweetersAsync(TwitterContext twitterCtx)
        {
            ulong tweetID = 210591841312190464;

            Status status =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Retweeters &&
                       tweet.ID == tweetID
                 select tweet)
                .SingleOrDefaultAsync();

            if (status != null && status.User != null)
                status.Users.ForEach(
                    userID => Console.WriteLine("User ID: " + userID));
        }

        static async Task GetConversationAsync(TwitterContext twitterCtx)
        {
            var tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Conversation &&
                       tweet.ID == 420611683317342208ul
                 select tweet)
                .ToListAsync();

            PrintTweetsResults(tweets);
        }

        static async Task LookupTweetsAsyc(TwitterContext twitterCtx)
        {
            var tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Lookup &&
                       tweet.TweetIDs == "460788892723978241,462758132448362496,460060836967768064"
                 select tweet)
                .ToListAsync();

            PrintTweetsResults(tweets);
        }
    }
}
