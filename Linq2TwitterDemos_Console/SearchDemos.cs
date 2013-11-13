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
  
        static async Task DoSearchAsync(TwitterContext twitterCtx)
        {
            var searchResponse =
                await
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "LINQ to Twitter"
                 select search)
                .SingleOrDefaultAsync();

            searchResponse.Statuses.ForEach(tweet =>
                Console.WriteLine(
                    "User: {0}, Tweet: {1}", 
                    tweet.User.ScreenNameResponse,
                    tweet.Text));
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nSearch Demos - Please select:\n");

            Console.WriteLine("\t 0. Search");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
