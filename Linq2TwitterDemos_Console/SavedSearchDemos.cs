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
                        await ShowSavedSearchesAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tShowing saved search...\n");
                        await ShowSavedSearchAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tCreating...\n");
                        await CreateSavedSearchAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tDeleting...\n");
                        await DestroySavedSearchAsync(twitterCtx);
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
            Console.WriteLine("\nSaved Search Demos - Please select:\n");

            Console.WriteLine("\t 0. Show Saved Searches");
            Console.WriteLine("\t 1. Show Saved Search");
            Console.WriteLine("\t 2. Create Saved Search");
            Console.WriteLine("\t 3. Destroy Saved Search");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }

        static async Task ShowSavedSearchesAsync(TwitterContext twitterCtx)
        {
            var savedSearches =
                await
                    (from search in twitterCtx.SavedSearch
                     where search.Type == SavedSearchType.Searches
                     select search)
                    .ToListAsync();

            if (savedSearches != null)
                savedSearches.ForEach(
                    search => Console.WriteLine("Search: " + search.Query));
        }

        static async Task ShowSavedSearchAsync(TwitterContext twitterCtx)
        {
            ulong savedSearchID = 306668698;

            var savedSearch =
                await
                (from search in twitterCtx.SavedSearch
                 where search.Type == SavedSearchType.Show &&
                       search.ID == savedSearchID
                 select search)
                .SingleOrDefaultAsync();

            if (savedSearch != null)
                Console.WriteLine(
                    "ID: {0}, Search: {1}", 
                    savedSearch.ID, savedSearch.Name);
        }

        static async Task CreateSavedSearchAsync(TwitterContext twitterCtx)
        {
            SavedSearch savedSearch = 
                await twitterCtx.CreateSavedSearchAsync("linq");

            if (savedSearch != null)
                Console.WriteLine(
                    "ID: {0}, Search: {1}", 
                    savedSearch.IDResponse, savedSearch.Query);
        }

        static async Task DestroySavedSearchAsync(TwitterContext twitterCtx)
        {
            ulong savedSearchID = 0;

            SavedSearch savedSearch = 
                await twitterCtx.DestroySavedSearchAsync(savedSearchID);

            if (savedSearch != null)
                Console.WriteLine(
                    "ID: {0}, Search: {1}", 
                    savedSearch.ID, savedSearch.Name);
        }
    }
}
