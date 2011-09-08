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
            SearchLocationTrendsDemo(twitterCtx);
            SearchTrendsDemo(twitterCtx);
            SearchDailyTrendsDemo(twitterCtx);
            SearchWeeklyTrendsDemo(twitterCtx);
        }

        #region Trends Demos

        /// <summary>
        /// Find locations where trending topics are occurring
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchLocationTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                (from trnd in twitterCtx.Trends
                 where trnd.Type == TrendType.Location &&
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

        /// <summary>
        /// shows how to request weekly trends
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchWeeklyTrendsDemo(TwitterContext twitterCtx)
        {
            // remember to truncate seconds (maybe even minutes) because they
            // will never compare evenly, causing your list to be empty
            var trends =
                from trend in twitterCtx.Trends
                where trend.Type == TrendType.Weekly &&
                      trend.ExcludeHashtags == true &&
                      trend.Date == DateTime.Now.AddDays(-14).Date // <-- no time part
                select trend;

            trends.ToList().ForEach(trnd => EmitTrend(trnd));
        }

        /// <summary>
        /// shows how to request daily trends
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchDailyTrendsDemo(TwitterContext twitterCtx)
        {
            // remember to truncate time because they
            // will never compare evenly, causing your list to be empty
            var trends =
                (from trend in twitterCtx.Trends
                 where trend.Type == TrendType.Daily &&
                       trend.Date == DateTime.Now.AddDays(-2).Date // <-- no time part
                 select trend)
                 .ToList();

            trends.ForEach(trnd => EmitTrend(trnd));
        }

        /// <summary>
        /// shows how to request trends (world-wide)
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTrendsDemo(TwitterContext twitterCtx)
        {
            var trends =
                from trend in twitterCtx.Trends
                where trend.Type == TrendType.Trend
                select trend;

            trends.ToList().ForEach(trnd => EmitTrend(trnd));
        }

        private static void EmitTrend(Trend trnd)
        {
            Console.WriteLine(
                    "Name: {0}, Date: {1}, Query: {2}\nSearchUrl: {3}",
                    trnd.Name, trnd.AsOf, trnd.Query, trnd.SearchUrl);
        }
        #endregion
    }
}
