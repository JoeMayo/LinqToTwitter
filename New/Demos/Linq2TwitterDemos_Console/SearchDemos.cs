using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

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
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
  
        static async Task DoSearchAsync(TwitterContext twitterCtx)
        {
            string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter";
            //string searchTerm = "#ömer -RT -instagram news source%3Afoursquare";

            var searchResponse =
                await
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == searchTerm
                 select search)
                .SingleOrDefaultAsync();

            if (searchResponse != null && searchResponse.Statuses != null)
                searchResponse.Statuses.ForEach(tweet =>
                    Console.WriteLine(
                        "\n  User: {0} ({1})\n  Tweet: {2}", 
                        tweet.User.ScreenNameResponse,
                        tweet.User.UserIDResponse,
                        tweet.Text));
        }
    }
}
