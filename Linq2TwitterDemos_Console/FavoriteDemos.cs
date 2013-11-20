using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

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
            Console.WriteLine("\t Q. Return to Main menu");
        }

        static async Task ShowFavoritesAsync(TwitterContext twitterCtx)
        {
            var favsResponse =
                await
                    (from fav in twitterCtx.Favorites
                     where fav.Type == FavoritesType.Favorites
                     select fav)
                    .ToListAsync();

            favsResponse.ForEach(fav => 
                Console.WriteLine(
                    "Name: {0}, Tweet: {1}",
                    fav.User.ScreenNameResponse, fav.Text));
        }
        static async Task DestroyFavoriteAsync(TwitterContext twitterCtx)
        {
            var status = await twitterCtx.DestroyFavoriteAsync(401033367283453953ul, true);

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        static async Task CreateFavoriteAsync(TwitterContext twitterCtx)
        {
            var status = await twitterCtx.CreateFavoriteAsync(401033367283453953ul);

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }
    }
}
