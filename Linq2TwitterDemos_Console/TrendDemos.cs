using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    public class TrendDemos
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
                        Console.WriteLine("\n\tGetting available trend locations...\n");
                        await GetAvailableTrendLocations(twitterCtx);
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
  
        static async Task GetAvailableTrendLocations(TwitterContext twitterCtx)
        {
            var trendsResponse =
                await
                (from trend in twitterCtx.Trends
                 where trend.Type == TrendType.Available
                 select trend)
                .SingleOrDefaultAsync();

            trendsResponse.Locations.ForEach(loc => Console.WriteLine("Location: " + loc.Name));
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nTrend Demos - Please select:\n");

            Console.WriteLine("\t 0. Get Available Trend Locations");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }
    }
}
