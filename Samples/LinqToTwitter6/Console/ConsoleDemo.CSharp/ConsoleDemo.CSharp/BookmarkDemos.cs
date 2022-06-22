using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace ConsoleDemo.CSharp
{
    class BookmarkDemos
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
                        Console.WriteLine("\n\tBookmarking tweet...\n");
                        await BookmarkAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tRemoving bookmark...\n");
                        await RemoveBookmarkAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tGetting bookmarks...\n");
                        await GetBookmarksAsync(twitterCtx);
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
            Console.WriteLine("\nBookmark Demos - Please select:\n");

            Console.WriteLine("\t 0. Bookmark a Tweet");
            Console.WriteLine("\t 1. Unbookmark a Tweet");
            Console.WriteLine("\t 2. Bookmarks");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task BookmarkAsync(TwitterContext twitterCtx)
        {
            string tweetID = "1371844879043723273";
            string userID = "15411837";

            if (userID == null)
            {
                Console.WriteLine($"{nameof(userID)} is null.");
                return;
            }

            BookmarkResponse? user = await twitterCtx.BookmarkAsync(userID, tweetID);

            if (user?.Data != null)
                Console.WriteLine("Is Bookmarked: " + user.Data.Bookmarked);
        }

        static async Task RemoveBookmarkAsync(TwitterContext twitterCtx)
        {
            string? tweetID = "1371844879043723273";
            string userID = "15411837";

            if (userID == null)
            {
                Console.WriteLine($"{nameof(userID)} is null.");
                return;
            }

            BookmarkResponse? user = await twitterCtx.RemoveBookmarkAsync(userID, tweetID);

            if (user?.Data != null)
                Console.WriteLine("Is Bookmarked: " + user.Data.Bookmarked);
        }

        static async Task GetBookmarksAsync(TwitterContext twitterCtx)
        {
            string userID = "15411837";

            TweetQuery? tweetResponse =
                await
                (from tweet in twitterCtx.Tweets
                 where tweet.Type == TweetType.Bookmarks &&
                       tweet.ID == userID
                 select tweet)
                .SingleOrDefaultAsync();

            if (tweetResponse?.Tweets != null)
                tweetResponse.Tweets.ForEach(tweet =>
                    Console.WriteLine(
                        $"\nID: {tweet.ID}" +
                        $"\nTweet: {tweet.Text}"));
            else
                Console.WriteLine("No entries found.");
        }
    }
}
