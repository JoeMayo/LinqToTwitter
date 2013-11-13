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
                        await ShowFavorites(twitterCtx);
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

        static async Task ShowFavorites(TwitterContext twitterCtx)
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

        static void ShowMenu()
        {
            Console.WriteLine("\nFavorite Demos - Please select:\n");

            Console.WriteLine("\t 0. Show Favorites");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
