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
            //SearchPlaceTrendsDemo(twitterCtx);
        }

        /// <summary>
        /// Find locations where trending topics are occurring
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchPlaceTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                (from trnd in twitterCtx.Trends
                 where trnd.Type == TrendType.Place &&
                       trnd.WeoID == 2486982 // something other than 1
                 select trnd)
                 .ToList();

            // Location is the same for each trending item, so just read the first
            Console.WriteLine("Location: {0}\n", trends[0].Location.Name);

            trends.ForEach(trnd => EmitTrend(trnd));
        }

        /// <summary>
        /// Find locations where trending topics are occurring
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchAvailableTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                from trnd in twitterCtx.Trends
                where trnd.Type == TrendType.Available
                select trnd;

            var trend = trends.FirstOrDefault();

            trend.Locations.ToList().ForEach(
                loc => Console.WriteLine(
                    "Name: {0}, Country: {1}, WoeID: {2}",
                    loc.Name, loc.Country, loc.WoeID));
        }

        private static void EmitTrend(Trend trnd)
        {
            Console.WriteLine(
                    "Name: {0}, Date: {1}, Query: {2}\nSearchUrl: {3}",
                    trnd.Name, trnd.TrendDate, trnd.Query, trnd.SearchUrl);
        }
    }
}
