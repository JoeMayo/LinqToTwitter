using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Collections.Generic;
using System.Diagnostics;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
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
                    case '2':
                        Console.WriteLine("\n\tSearching...\n");
                        await DoRecentSearchAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tSearching...\n");
                        await DoFullSearchAsync(twitterCtx);
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
            Console.WriteLine("\t 2. Recent Tweets Search");
            Console.WriteLine("\t 3. Full Tweets Search");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task DoSearchAsync(TwitterContext twitterCtx)
        {
            string searchTerm = "linq to";
            //string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter OR JoeMayo";
            //searchTerm = "кот (";

            Search? searchResponse =
                await
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == searchTerm //&&
                       //search.IncludeEntities == true &&
                       //search.TweetMode == TweetMode.Extended
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse?.Statuses != null)
                searchResponse.Statuses.ForEach(tweet =>
                    Console.WriteLine(
                        "\n  User: {0} ({1})\n  Tweet: {2}",
                        tweet.User?.ScreenNameResponse,
                        tweet.User?.UserIDResponse,
                        tweet.Text ?? tweet.FullText));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task DoPagedSearchAsync(TwitterContext twitterCtx)
        {
            const int MaxSearchEntriesToReturn = 100;
            const int SearchRateLimit = 180;

            string searchTerm = "twitter";

            // oldest id you already have for this search term
            ulong sinceID = 1;

            // used after the first query to track current session
            ulong maxID;

            var combinedSearchResults = new List<Status>();

            List<Status>? searchResponse =
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

                    if (searchResponse == null) break;

                    combinedSearchResults.AddRange(searchResponse!);
                } while (searchResponse.Any() && combinedSearchResults.Count < SearchRateLimit);

                combinedSearchResults.ForEach(tweet =>
                    Console.WriteLine(
                        "\n  User: {0} ({1})\n  Tweet: {2}",
                        tweet.User?.ScreenNameResponse,
                        tweet.User?.UserIDResponse,
                        tweet.Text ?? tweet.FullText));
            }
            else
            {
                Console.WriteLine("No entries found.");
            }

        }

        static async Task DoRecentSearchAsync(TwitterContext twitterCtx)
        {
            string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter OR JoeMayo";
            searchTerm = "Twitter";

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

            TwitterSearch? searchResponse =
                await
                (from search in twitterCtx.TwitterSearch
                 where search.Type == SearchType.RecentSearch &&
                       search.Query == searchTerm &&
                       search.MaxResults == 100 &&
                       search.SortOrder == SearchSortOrder.Relevancy &&
                       search.TweetFields == TweetField.AllFieldsExceptPermissioned &&
                       search.Expansions == ExpansionField.AllTweetFields &&
                       search.MediaFields == MediaField.AllFieldsExceptPermissioned &&
                       search.PlaceFields == PlaceField.AllFields &&
                       search.PollFields == PollField.AllFields &&
                       search.UserFields == UserField.AllFields
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse?.Tweets != null)
                searchResponse.Tweets.ForEach(tweet =>
                    Console.WriteLine(
                        $"\nID: {tweet.ID}" +
                        $"\nTweet: {tweet.Text}"));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task DoFullSearchAsync(TwitterContext twitterCtx)
        {
            string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter OR JoeMayo";

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

            TwitterSearch? searchResponse =
                await
                (from search in twitterCtx.TwitterSearch
                 where search.Type == SearchType.FullSearch &&
                       search.Query == searchTerm &&
                       search.TweetFields == TweetField.AllFieldsExceptPermissioned &&
                       search.Expansions == ExpansionField.AllTweetFields &&
                       search.MediaFields == MediaField.AllFieldsExceptPermissioned &&
                       search.PlaceFields == PlaceField.AllFields &&
                       search.PollFields == PollField.AllFields &&
                       search.UserFields == UserField.AllFields
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse?.Tweets != null)
                searchResponse.Tweets.ForEach(tweet =>
                    Console.WriteLine(
                        $"\nID: {tweet.ID}" +
                        $"\nTweet: {tweet.Text}"));
            else
                Console.WriteLine("No entries found.");
        }
    }
}
