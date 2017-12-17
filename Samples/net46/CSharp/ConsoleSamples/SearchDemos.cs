using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Collections.Generic;
using System.Diagnostics;

namespace Linq2TwitterDemos_Console
{
    public class SearchDemos
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
                        Console.WriteLine("\n\tSearching...\n");
                        await DoSearchAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tSearching...\n");
                        await DoPagedSearchAsync(twitterCtx);
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

            Console.WriteLine("\t 0. Search");
            Console.WriteLine("\t 1. Paged Search");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }
  
        static async Task DoSearchAsync(TwitterContext twitterCtx)
        {
            string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter OR JoeMayo";

            Search searchResponse =
                await
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == searchTerm &&
                       search.IncludeEntities == true &&
                       search.TweetMode == TweetMode.Extended
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse?.Statuses != null)
                searchResponse.Statuses.ForEach(tweet =>
                    Console.WriteLine(
                        "\n  User: {0} ({1})\n  Tweet: {2}", 
                        tweet.User.ScreenNameResponse,
                        tweet.User.UserIDResponse,
                        tweet.Text ?? tweet.FullText));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task DoPagedSearchAsync(TwitterContext twitterCtx)
        {
            const int MaxSearchEntriesToReturn = 10;
            const int MaxTotalResults = 100;

            string searchTerm = "twitter";

            // oldest id you already have for this search term
            ulong sinceID = 1;

            // used after the first query to track current session
            ulong maxID; 

            var combinedSearchResults = new List<Status>();

            List<Status> searchResponse =
                await
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == searchTerm &&
                       search.Count == MaxSearchEntriesToReturn &&
                       search.SinceID == sinceID &&
                       search.TweetMode == TweetMode.Extended
                 select search.Statuses)
                .SingleOrDefaultAsync();

            if (searchResponse != null)
            {
                combinedSearchResults.AddRange(searchResponse);
                ulong previousMaxID = ulong.MaxValue;
                do
                {
                    // one less than the newest id you've just queried
                    maxID = searchResponse.Min(status => status.StatusID) - 1;

                    Debug.Assert(maxID < previousMaxID);
                    previousMaxID = maxID;

                    searchResponse =
                        await
                        (from search in twitterCtx.Search
                         where search.Type == SearchType.Search &&
                               search.Query == searchTerm &&
                               search.Count == MaxSearchEntriesToReturn &&
                               search.MaxID == maxID &&
                               search.SinceID == sinceID &&
                               search.TweetMode == TweetMode.Extended
                         select search.Statuses)
                        .SingleOrDefaultAsync();

                    combinedSearchResults.AddRange(searchResponse);
                } while (searchResponse.Any() && combinedSearchResults.Count < MaxTotalResults);

                combinedSearchResults.ForEach(tweet =>
                    Console.WriteLine(
                        "\n  User: {0} ({1})\n  Tweet: {2}",
                        tweet.User.ScreenNameResponse,
                        tweet.User.UserIDResponse,
                        tweet.Text ?? tweet.FullText)); 
            }
            else
            {
                Console.WriteLine("No entries found.");
            }
        }
    }
}
