using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class SavedSearchDemos
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
                        Console.WriteLine("\n\tShowing saved searches...\n");
                        await ShowSavedSearches(twitterCtx);
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

        static async Task ShowSavedSearches(TwitterContext twitterCtx)
        {
            var savedSearches =
                await
                    (from search in twitterCtx.SavedSearch
                     where search.Type == SavedSearchType.Searches
                     select search)
                    .ToListAsync();

            savedSearches.ForEach(search => Console.WriteLine("Search: " + search.Query));
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nSaved Search Demos - Please select:\n");

            Console.WriteLine("\t 0. Show Saved Searches");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
