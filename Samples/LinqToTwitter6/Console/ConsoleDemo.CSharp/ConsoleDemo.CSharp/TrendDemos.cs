using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace ConsoleDemo.CSharp
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
                        Console.WriteLine("\n\tGetting trends...\n");
                        await GetTrendsForPlaceAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tGetting available trend locations...\n");
                        await GetAvailableTrendLocationsAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tGetting trends...\n");
                        await GetClosestTrendsAsync(twitterCtx);
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
            Console.WriteLine("\nTrend Demos - Please select:\n");

            Console.WriteLine("\t 0. Get Trends for a Place");
            Console.WriteLine("\t 1. Get Available Trend Locations");
            Console.WriteLine("\t 2. Get Trends Closest to a Location");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task GetTrendsForPlaceAsync(TwitterContext twitterCtx)
        {
            System.Collections.Generic.List<Trend> trends =
                await
                (from trend in twitterCtx.Trends
                 where trend.Type == TrendType.Place &&
                       trend.WoeID == 2486982
                 select trend)
                .ToListAsync();

            if (trends != null && 
                trends.Any() && 
                trends.First().Locations != null)
            {
                Console.WriteLine(
                    "Location: {0}\n",
                    trends.FirstOrDefault()?.Locations?.FirstOrDefault()?.Name);

                trends.ForEach(trnd =>
                    Console.WriteLine(
                        "Tweet Volume: {0}, Name: {1}, Date: {2}, Query: {3}\nSearchUrl: {4}",
                        trnd.TweetVolume, trnd.Name, trnd.CreatedAt, trnd.Query, trnd.SearchUrl)); 
            }
        }

        static async Task GetAvailableTrendLocationsAsync(TwitterContext twitterCtx)
        {
            Trend? trendsResponse =
                await
                (from trend in twitterCtx.Trends
                 where trend.Type == TrendType.Available
                 select trend)
                .SingleOrDefaultAsync();

            if (trendsResponse != null && trendsResponse.Locations != null)
                trendsResponse.Locations.ForEach(
                    loc => Console.WriteLine("Location: " + loc.Name));
        }

        static async Task GetClosestTrendsAsync(TwitterContext twitterCtx)
        {
            Trend? trend =
                await
                (from trnd in twitterCtx.Trends
                 where trnd.Type == TrendType.Closest &&
                       trnd.Latitude == 37.78215 &&
                       trnd.Longitude == -122.40060
                 select trnd)
                .SingleOrDefaultAsync();

            if (trend != null && trend.Locations != null)
                trend.Locations.ForEach(
                    loc => Console.WriteLine(
                        "Name: {0}, Country: {1}, WoeID: {2}",
                        loc.Name, loc.Country, loc.WoeID));
        }
    }
}
