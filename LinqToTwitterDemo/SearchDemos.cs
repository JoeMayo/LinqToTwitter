using System;
using System.Linq;

using System.Linq.Expressions;
using LinqToTwitter;
using System.Collections.Generic;

namespace LinqToTwitterDemo
{
    public class SearchDemos
    {
        public static void Run(TwitterContext twitterCtx)
        {
            BasicSearchSample(twitterCtx);
            //AsyncSearchSample(twitterCtx);
            //GeocodeSample(twitterCtx);
            //ConditionalSearchDemo(twitterCtx);
            //DynamicSearchDemo(twitterCtx);
        }

        static void BasicSearchSample(TwitterContext twitterCtx)
        {
            var srch =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "LINQ to Twitter" &&
                       //search.Query == @"`!@#$%^&*()_-+=.~,:;'?/|\[] éü\u00C7" &&
                       search.Count == 25
                 select search)
                .SingleOrDefault();

            Console.WriteLine("\nQuery: {0}\n", srch.SearchMetaData.Query);
            srch.Statuses.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                    entry.StatusID, entry.Source, entry.Text));

            Console.WriteLine("\n More Search demos can be downloaded from LINQ to Twitter's on-line samples at http://linqtotwitter.codeplex.com/wikipage?title=LINQ%20to%20Twitter%20Samples&referringTitle=Home");
        }

        static void AsyncSearchSample(TwitterContext twitterCtx)
        {
            (from search in twitterCtx.Search
             where search.Type == SearchType.Search &&
                   search.Query == "LINQ To Twitter"
             select search)
            .MaterializedAsyncCallback(resp =>
            {
                if (resp.Status != TwitterErrorStatus.Success)
                {
                    Exception ex = resp.Exception;
                    // handle error
                    throw ex;
                }

                Search srch = resp.State.First();
                Console.WriteLine("\nQuery: {0}\n", srch.SearchMetaData.Query);

                srch.Statuses.ForEach(entry =>
                    Console.WriteLine(
                        "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                        entry.StatusID, entry.Source, entry.Text));

                Console.WriteLine("\n More Search demos can be downloaded from LINQ to Twitter's on-line samples at http://linqtotwitter.codeplex.com/wikipage?title=LINQ%20to%20Twitter%20Samples&referringTitle=Home");
            });
        }

        static void GeocodeSample(TwitterContext twitterCtx)
        {
            var srch =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "Twitter" &&
                       search.GeoCode == "37.781157,-122.398720,1mi"
                 select search)
                .SingleOrDefault();

            Console.WriteLine("\nQuery: {0}\n", srch.SearchMetaData.Query);
            srch.Statuses.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                    entry.StatusID, entry.Source, entry.Text));

            Console.WriteLine("\n More Search demos can be downloaded from LINQ to Twitter's on-line samples at http://linqtotwitter.codeplex.com/wikipage?title=LINQ%20to%20Twitter%20Samples&referringTitle=Home");
        }

        static void DynamicSearchDemo(TwitterContext twitterCtx)
        {
            const string TwitterSearchGeocodeFormat = "{0},{1},{2}";
            string query = "Twitter";
            string language = null;
            string locale = null;
            string latitude = "37.781157";
            string longitude = "-122.398720";
            uint radius = 1;
            string radiusUnitType = "mi";

            var srchQuery =
                from srch in twitterCtx.Search
                where srch.Type == SearchType.Search
                select srch;

            if (!string.IsNullOrWhiteSpace(query))
            {
                srchQuery = srchQuery.Where(srch => srch.Query == query);
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                srchQuery = srchQuery.Where(srch => srch.SearchLanguage == language);
            }

            if (!string.IsNullOrWhiteSpace(locale))
            {
                srchQuery = srchQuery.Where(srch => srch.Locale == locale);
            }

            if (!string.IsNullOrWhiteSpace(longitude) && !string.IsNullOrWhiteSpace(latitude) && radius > 0)
            {
                var radiusString = string.Format("{0}{1}", radius, radiusUnitType.ToString().ToLower());
                var geoCodeParameter = String.Format(TwitterSearchGeocodeFormat, latitude, longitude, radiusString);

                srchQuery = srchQuery.Where(srch => srch.GeoCode == geoCodeParameter);
            }

            var srchResult = srchQuery.SingleOrDefault();

            Console.WriteLine("\nQuery: {0}\n", srchResult.SearchMetaData.Query);
            srchResult.Statuses.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                    entry.StatusID, entry.Source, entry.Text));
        }

        static void ConditionalSearchDemo(TwitterContext twitterCtx)
        {
            const string TwitterSearchGeocodeFormat = "{0},{1},{2}";
            string query = "Twitter";
            string language = null;
            string locale = null;
            string latitude = "37.781157";
            string longitude = "-122.398720";
            uint radius = 1;
            string radiusUnitType = "mi";

            Type searchType = typeof(Search);
            ParameterExpression srch = Expression.Parameter(searchType, "srch");

            var predicates = new List<Expression>();

            if (!string.IsNullOrWhiteSpace(query))
            {
                predicates.Add(
                    Expression.Equal(
                        Expression.Property(srch, "Query"),
                        Expression.Constant(query)));
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                predicates.Add(
                    Expression.Equal(
                        Expression.Property(srch, "SearchLanguage"),
                        Expression.Constant(language)));
            }

            if (!string.IsNullOrWhiteSpace(locale))
            {
                predicates.Add(
                    Expression.Equal(
                        Expression.Property(srch, "Locale"),
                        Expression.Constant(locale)));
            }

            if (!string.IsNullOrWhiteSpace(longitude) && !string.IsNullOrWhiteSpace(latitude) && radius > 0)
            {
                var radiusString = string.Format("{0}{1}", radius, radiusUnitType.ToString().ToLower());
                var geoCodeParameter = String.Format(TwitterSearchGeocodeFormat, latitude, longitude, radiusString);
                predicates.Add(
                    Expression.Equal(
                        Expression.Property(srch, "GeoCode"),
                        Expression.Constant(geoCodeParameter)));
            }

            BinaryExpression expr = Expression.Equal(
                Expression.Property(srch, "Type"),
                Expression.Constant(SearchType.Search));

            predicates.ForEach(pred => expr = Expression.AndAlso(expr, pred));

            var searchLambda =
                Expression.Lambda(expr, srch) as Expression<Func<Search, bool>>;

            var response = 
                twitterCtx.Search
                    .Where(searchLambda)
                    .SingleOrDefault();

            Console.WriteLine("\nQuery: {0}\n", response.SearchMetaData.Query);
            response.Statuses.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                    entry.StatusID, entry.Source, entry.Text));
        }

    }
}
