using System;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows trends demos
    /// </summary>
    public class TrendsDemos
    {
        /// <summary>
        /// Run all trends related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            SearchAvailableTrendsDemo(twitterCtx);
            SearchClosestTrendsDemo(twitterCtx);
            SearchPlaceTrendsDemo(twitterCtx);
        }

        /// <summary>
        /// Find current trends
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void SearchAvailableTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                from trnd in twitterCtx.Trends
                where trnd.Type == TrendType.Available
                select trnd;

            var trend = trends.SingleOrDefault();

            trend.Locations.ToList().ForEach(
                loc => Console.WriteLine(
                    "Name: {0}, Country: {1}, WoeID: {2}",
                    loc.Name, loc.Country, loc.WoeID));
        }

        /// <summary>
        /// Find trends near a specified lat/long
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void SearchClosestTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                from trnd in twitterCtx.Trends
                where trnd.Type == TrendType.Closest &&
                      trnd.Latitude == "37.78215" &&
                      trnd.Longitude == "-122.40060"
                select trnd;

            var trend = trends.SingleOrDefault();

            trend.Locations.ToList().ForEach(
                loc => Console.WriteLine(
                    "Name: {0}, Country: {1}, WoeID: {2}",
                    loc.Name, loc.Country, loc.WoeID));
        }

        /// <summary>
        /// Find trends at a specified WeoID
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        static void SearchPlaceTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                (from trnd in twitterCtx.Trends
                 where trnd.Type == TrendType.Place &&
                       trnd.WoeID == 2486982 // something other than 1
                 select trnd)
                .ToList();

            Console.WriteLine(
                "Location: {0}\n", 
                trends.First().Locations.First().Name);

            trends.ForEach(trnd => 
                Console.WriteLine(
                    "Name: {0}, Date: {1}, Query: {2}\nSearchUrl: {3}",
                    trnd.Name, trnd.CreatedAt, trnd.Query, trnd.SearchUrl));
        }
    }
}
