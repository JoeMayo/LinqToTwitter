using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows search demos
    /// </summary>
    public class SearchDemos
    {
        /// <summary>
        /// Run all search related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            SearchTwitterDemo(twitterCtx);
            //SearchOperatorDemo(twitterCtx);
            //SearchTwitterSinceIDDemo(twitterCtx);
            //SearchTwitterLocationDemo(twitterCtx);
            //SearchTwitterLocaleDemo(twitterCtx);
            //SearchAndUseStatusTwitterDemo(twitterCtx);
            //SearchByLanguageTwitterDemo(twitterCtx);
            //SearchSinceDateTwitterDemo(twitterCtx);
            //SearchTwitterSource(twitterCtx);
            //SearchWithResultType(twitterCtx);
            //SearchWithWordQuery(twitterCtx);
            //SearchWithPersonReferenceQuery(twitterCtx);
            //SearchWithPersonFromQuery(twitterCtx);
            //SearchWithAttitudeQuery(twitterCtx);
            //SearchWithLinksQuery(twitterCtx);
            //SearchCountDemo(twitterCtx);
            //SearchDatesDemo(twitterCtx);
            //SearchEntriesQueryDemo(twitterCtx);
            //SearchGeoCodeDemo(twitterCtx);
            //SearchGeoCodeAndShowUserDemo(twitterCtx);
            //SearchIncludingEntities(twitterCtx);
        }

        /// <summary>
        /// shows how to perform a twitter search
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTwitterDemo(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "Linq=To=Twitter" // "LINQ to Twitter" &&
                      //search.Page == 2 &&
                      //search.PageSize == 5
                select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.QueryResult);

                foreach (var entry in search.Results)
                {
                    Console.WriteLine(
                        "ID: {0}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Text);
                }
            }
        }

        /// <summary>
        /// shows how to perform a twitter search, using a Twitter Search operator
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchOperatorDemo(TwitterContext twitterCtx)
        {

            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "to:JoeMayo"
                select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.QueryResult);

                foreach (var entry in search.Results)
                {
                    Console.WriteLine(
                        "ID: {0}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Text);
                }
            }
        }

        /// <summary>
        /// shows how to perform a twitter search using SinceID
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTwitterSinceIDDemo(TwitterContext twitterCtx)
        {
            string searchTerm = "Linq to Twitter";
            ulong lastId = 9172099221;
            int pageSize = 10;

            var queryResults = from search in twitterCtx.Search
                               where search.Type == SearchType.Search &&
                                     search.Query == searchTerm.Trim() &&
                                     search.PageSize == pageSize &&
                                     search.SinceID == lastId
                               select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.QueryResult);

                foreach (var entry in search.Results)
                {
                    Console.WriteLine(
                        "ID: {0}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Text);
                }
            }
        }

        /// <summary>
        /// shows how to perform a twitter search using SinceID
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTwitterLocationDemo(TwitterContext twitterCtx)
        {
            string searchTerm = "JoeMayo";

            var queryResults = from search in twitterCtx.Search
                               where search.Type == SearchType.Search &&
                                     search.Query == searchTerm
                               select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.QueryResult);

                foreach (var entry in search.Results)
                {
                    Console.WriteLine(
                        "ID: {0}\nLocation: {1}\nSource: {2}\nContent: {3}\n",
                        entry.ID, entry.Geo, entry.Source, entry.Text);
                }
            }
        }

        /// <summary>
        /// shows how to perform a twitter search using SinceID
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTwitterLocaleDemo(TwitterContext twitterCtx)
        {
            string searchTerm = "東京大地震";
            string locale = "ja";

            var queryResults = from search in twitterCtx.Search
                               where search.Type == SearchType.Search &&
                                     search.Query == searchTerm &&
                                     search.Locale == locale
                               select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.QueryResult);

                foreach (var entry in search.Results)
                {
                    Console.WriteLine(
                        "ID: {0}\nSource: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Text);
                }
            }
        }

        /// <summary>
        /// shows how to perform a twitter search, extract status, and search the status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchAndUseStatusTwitterDemo(TwitterContext twitterCtx)
        {
            var queryResult =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "LINQ to Twitter"
                 select search)
                 .SingleOrDefault();

            foreach (var entry in queryResult.Results)
            {
                string statusID = entry.ID.ToString();

                var status =
                    (from tweet in twitterCtx.Status
                     where tweet.Type == StatusType.Show &&
                           tweet.ID == statusID
                     select tweet)
                     .SingleOrDefault();

                Console.WriteLine(
                    "ID: {0}, User: {1}\nTweet: {2}\n",
                    status.ID, status.User.Name, status.Text);
            }
        }

        /// <summary>
        /// shows how to perform a twitter search, extract status, and search the status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchByLanguageTwitterDemo(TwitterContext twitterCtx)
        {
            var filter = "Dallas";

            var queryResult =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                    search.Query == filter &&
                    search.SearchLanguage == "en"
                 select search)
                 .SingleOrDefault();

            foreach (var entry in queryResult.Results)
            {
                Console.WriteLine(
                    "ID: {0}, Source: {1}\nContent: {2}\n",
                    entry.ID, entry.Source, entry.Text);
            }
        }

        /// <summary>
        /// shows how to perform a twitter search, extract status, and search the status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchSinceDateTwitterDemo(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "Leite Moca" &&
                      search.SearchLanguage == "pt" &&
                      search.PageSize == 50 &&
                      search.Since >= DateTime.Now.Date
                select search.Results;

            var searchResult = queryResults.FirstOrDefault();

            foreach (var entry in searchResult)
            {
                Console.WriteLine(
                    "ID: {0}, Source: {1}\nContent: {2}\n",
                    entry.ID, entry.Source, entry.Text);
            }
        }

        /// <summary>
        /// Shows how to specify a source of tweets to search for
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTwitterSource(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "LINQ to Twitter source:web"
                select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.QueryResult);

                foreach (var entry in search.Results)
                {
                    Console.WriteLine(
                        "ID: {0}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Text);
                }
            }
        }

        /// <summary>
        /// Demonstrates how to use the ResultType parameter
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchWithResultType(TwitterContext twitterCtx)
        {
            var searchResults =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.ResultType == ResultType.Popular &&
                       search.Query == "Katy Perry"
                 select search)
                .FirstOrDefault();

            Console.WriteLine("\nQuery:\n" + searchResults.QueryResult);

            foreach (var entry in searchResults.Results)
            {
                Console.WriteLine(
                    "ID: {0}, Result Type: {1}\nContent: {2}\n",
                    entry.ID, entry.MetaData.ResultType, entry.Text);
            }
        }

        /// <summary>
        /// Demonstrates how to use words parameters
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchWithWordQuery(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.WordAnd == "LINQ Twitter"
                select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.QueryResult);

                foreach (var entry in search.Results)
                {
                    Console.WriteLine(
                        "ID: {0}, As of: {1}\nContent: {2}\n",
                        entry.ID, entry.CreatedAt, entry.Text);
                }
            }
        }

        /// <summary>
        /// Demonstrates how to use person parameters
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchWithPersonReferenceQuery(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.PersonReference == "JoeMayo"
                select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.QueryResult);

                foreach (var entry in search.Results)
                {
                    Console.WriteLine(
                        "ID: {0}, As of: {1}\nContent: {2}\n",
                        entry.ID, entry.CreatedAt, entry.Text);
                }
            }
        }

        /// <summary>
        /// Demonstrates how to use person parameters
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchWithPersonFromQuery(TwitterContext twitterCtx)
        {
            var queryResults =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.PersonFrom == "JoeMayo"
                 select search)
                .SingleOrDefault();

            Console.WriteLine("\nQuery:\n" + queryResults.QueryResult);

            foreach (var entry in queryResults.Results)
            {
                Console.WriteLine(
                    "ID: {0}, As of: {1}\nContent: {2}\n",
                    entry.ID, entry.CreatedAt, entry.Text);
            }
        }

        /// <summary>
        /// Demonstrates how to use the Attitude parameter
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchWithAttitudeQuery(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "Twitter" &&
                      search.Attitude == (Attitude.Negative | Attitude.Question)
                select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.QueryResult);

                foreach (var entry in search.Results)
                {
                    Console.WriteLine(
                        "ID: {0}, As of: {1}\nContent: {2}\n",
                        entry.ID, entry.CreatedAt, entry.Text);
                }
            }
        }

        /// <summary>
        /// Demonstrates how to use the WithLinks parameter
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchWithLinksQuery(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "Linq to Twitter" &&
                      search.WithLinks == true
                select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.QueryResult);

                foreach (var entry in search.Results)
                {
                    Console.WriteLine(
                        "ID: {0}, As of: {1}\nContent: {2}\n",
                        entry.ID, entry.CreatedAt, entry.Text);
                }
            }
        }

        private static void SearchCountDemo(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "Linq to Twitter"
                select search;

            var srch = queryResults.SingleOrDefault();

            Console.WriteLine("Number of references to LINQ to Twitter: " + srch.Results.Count);
        }

        private static void SearchDatesDemo(TwitterContext twitterCtx)
        {
            var result =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "LINQ to Twitter" &&
                       search.Since == DateTime.Now.AddDays(-15).Date && // remember to use Date property
                       search.Until == DateTime.Now.Date // remember to use Date property
                 select search)
                .SingleOrDefault();

            if (result != null && result.Results != null)
            {
                result.Results.ForEach(entry => Console.WriteLine("Result: {0}\n", entry.Text)); 
            }
        }

        private static void SearchEntriesQueryDemo(TwitterContext twitterCtx)
        {
            var queryResults =
                from tweet in twitterCtx.Search
                where tweet.Type == SearchType.Search &&
                tweet.Query == "Linq to Twitter"
                select tweet;


            var entries =
                (from result in queryResults
                from entry in result.Results
                select new SearchItem
                {
                    Tweet = entry.Text,
                    Date = entry.CreatedAt.Date,
                    Source = entry.Source,
                }).ToList();

            foreach (var entry in entries)
            {
                Console.WriteLine("Tweet: " + entry.Tweet);
            }

        }

        private static void SearchGeoCodeDemo(TwitterContext twitterCtx)
        {
            string geocode = "39.5485127,-104.9230675,500km"; //lat,lng,radius

            var result =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.GeoCode == geocode
                 select search)
                .SingleOrDefault();

            result.Results.ForEach(entry => Console.WriteLine("Result: {0}\n", entry.Text));
        }

        private static void SearchGeoCodeAndShowUserDemo(TwitterContext twitterCtx)
        {
            var queryResults = (from search in twitterCtx.Search
                                where search.Type == SearchType.Search &&
                                search.GeoCode == "13.0883,80.2833,30km" &&
                                search.ShowUser == true &&
                                search.PageSize == 50
                                select search).ToList();

            var entries =
                (from result in queryResults
                 from entry in result.Results
                 select new SearchItem
                 {
                     Tweet = entry.Text,
                     Date = entry.CreatedAt.Date,
                     Source = entry.Source
                 }).ToList();

            entries.ForEach(entry => Console.WriteLine("Result: {0}\n", entry.Tweet));
        }

        public class SearchItem
        {
            public string Tweet { get; set; }
            public DateTime Date { get; set; }
            public string Source { get; set; }
        };

        public class LocatedItem { }

        public class AuthorInfo
        {
            public string AuthorImage { get; set; }
            public string Url { get; set; }
            public string Name { get; set; }
        }

        public static void SearchIncludingEntities(TwitterContext twitterCtx)
        {
            var searchResults =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "@rschu" &&
                       search.IncludeEntities == true &&
                       search.PageSize == 100
                 select search)
                .SingleOrDefault();

            searchResults.Results.ForEach(entry =>
                {
                    Entities entities = entry.Entities;
                    Console.WriteLine(
                        "-- Entities --\n" +
                        "User: " + entities.UserMentions.Count + "\n" +
                        "Urls: " + entities.UrlMentions.Count + "\n" +
                        "Hash: " + entities.HashTagMentions.Count + "\n" +
                        "Media: " + entities.MediaMentions.Count + "\n");
                });
        }
    }
}
