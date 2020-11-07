using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Collections.Generic;
using LinqToTwitter.Common;

namespace Linq2TwitterDemos_Console
{
    class FavoriteDemos
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
                        Console.WriteLine("\n\tShowing favorites...\n");
                        await ShowFavoritesAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tFavoriting...\n");
                        await CreateFavoriteAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tUnfavoriting...\n");
                        await DestroyFavoriteAsync(twitterCtx);
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
            Console.WriteLine("\nFavorite Demos - Please select:\n");

            Console.WriteLine("\t 0. Show Favorites");
            Console.WriteLine("\t 1. Favorite a tweet");
            Console.WriteLine("\t 2. Unfavorite a tweet");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task ShowFavoritesAsync(TwitterContext twitterCtx)
        {
            const int PerQueryFavCount = 200;

            // set from a value that you previously saved
            ulong sinceID = 1; 

            var favsResponse =
                await
                    (from fav in twitterCtx.Favorites
                     where fav.Type == FavoritesType.Favorites &&
                           fav.Count == PerQueryFavCount &&
                           fav.TweetMode == TweetMode.Extended
                     select fav)
                    .ToListAsync();

            if (favsResponse == null)
            {
                Console.WriteLine("No favorites returned from Twitter.");
                return;
            }

            var favList = new List<Favorites>(favsResponse);

            // first tweet processed on current query
            ulong maxID = favList.Min(fav => fav.StatusID) - 1;

            do
            {
                favsResponse =
                    await
                        (from fav in twitterCtx.Favorites
                         where fav.Type == FavoritesType.Favorites &&
                               fav.Count == PerQueryFavCount &&
                               fav.SinceID == sinceID &&
                               fav.MaxID == maxID
                         select fav)
                        .ToListAsync();

                if (favsResponse == null || favsResponse.Count == 0) break;

                // reset first tweet to avoid re-querying the
                // same list you just received
                maxID = favsResponse.Min(fav => fav.StatusID) - 1;
                favList.AddRange(favsResponse);

            } while (favsResponse.Count > 0);

            favList.ForEach(fav => 
            {
                if (fav != null && fav.User != null)
                    Console.WriteLine(
                        "Name: {0}, Tweet: {1}",
                        fav.User.ScreenNameResponse, fav.Text);
            });

            // save this in your db for this user so you can set
            // sinceID accurately the next time you do a query
            // and avoid querying the same tweets again.
            ulong newSinceID = favList.Max(fav => fav.SinceID);
        }

        static async Task DestroyFavoriteAsync(TwitterContext twitterCtx)
        {
            var status = 
                await twitterCtx.DestroyFavoriteAsync(
                    401033367283453953ul, true);

            if (status != null)
                Console.WriteLine(
                    "User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        static async Task CreateFavoriteAsync(TwitterContext twitterCtx)
        {
            var status = await twitterCtx.CreateFavoriteAsync(401033367283453953ul);

            if (status != null)
                Console.WriteLine(
                    "User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }
    }
}
