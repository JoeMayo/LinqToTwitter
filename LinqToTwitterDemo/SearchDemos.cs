using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //SearchTwitterDemo(twitterCtx);
            //SearchOperatorDemo(twitterCtx);
            //SearchTwitterSinceIDDemo(twitterCtx);
            //SearchTwitterLocationDemo(twitterCtx);
            //SearchTwitterLocaleDemo(twitterCtx);
            //SearchAndUseStatusTwitterDemo(twitterCtx);
            //SearchByLanguageTwitterDemo(twitterCtx);
            //SearchTwitterSource(twitterCtx);
            //ExceedSearchRateLimitDemo(twitterCtx);
            //SearchWithResultType(twitterCtx);
            //SearchWithWordQuery(twitterCtx);
            //SearchWithPersonQuery(twitterCtx);
            //SearchWithAttitudeQuery(twitterCtx);
            SearchWithLinksQuery(twitterCtx);
        }

        #region Search Demos

        /// <summary>
        /// shows how to perform a twitter search
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTwitterDemo(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "LINQ to Twitter" &&
                      search.Page == 2 &&
                      search.PageSize == 5
                select search;

            foreach (var search in queryResults)
            {
                // here, you can see that properties are named
                // from the perspective of atom feed elements
                // i.e. the query string is called Title
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Content);
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
                // here, you can see that properties are named
                // from the perspective of atom feed elements
                // i.e. the query string is called Title
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Content);
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
                // here, you can see that properties are named
                // from the perspective of atom feed elements
                // i.e. the query string is called Title
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Content);
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
                // here, you can see that properties are named
                // from the perspective of atom feed elements
                // i.e. the query string is called Title
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}\nLocation: {1}\nSource: {2}\nContent: {3}\n",
                        entry.ID, entry.Location, entry.Source, entry.Content);
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
                // here, you can see that properties are named
                // from the perspective of atom feed elements
                // i.e. the query string is called Title
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}\nLanguage: {1}\nSource: {2}\nContent: {3}\n",
                        entry.ID, entry.Language, entry.Source, entry.Content);
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

            foreach (var entry in queryResult.Entries)
            {
                var statusID = entry.ID.Substring(entry.ID.LastIndexOf(":") + 1);

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

            foreach (var entry in queryResult.Entries)
            {
                Console.WriteLine(
                    "ID: {0}, Source: {1}, Language: {2}\nContent: {3}\n",
                    entry.ID, entry.Source, entry.Language, entry.Content);
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
                // here, you can see that properties are named
                // from the perspective of atom feed elements
                // i.e. the query string is called Title
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Content);
                }
            }
        }

        /// <summary>
        /// Demonstrates how to use the ResultType parameter
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchWithResultType(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.ResultType == ResultType.Mixed &&
                      search.Query == "Didi Benami"
                select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, Result Type: {1}\nContent: {2}\n",
                        entry.ID, entry.ResultType, entry.Content);
                }
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
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, As of: {1}\nContent: {2}\n",
                        entry.ID, entry.Updated, entry.Content);
                }
            }
        }

        /// <summary>
        /// Demonstrates how to use person parameters
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchWithPersonQuery(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.PersonReference == "JoeMayo"
                select search;

            foreach (var search in queryResults)
            {
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, As of: {1}\nContent: {2}\n",
                        entry.ID, entry.Updated, entry.Content);
                }
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
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, As of: {1}\nContent: {2}\n",
                        entry.ID, entry.Updated, entry.Content);
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
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, As of: {1}\nContent: {2}\n",
                        entry.ID, entry.Updated, entry.Content);
                }
            }
        }

        ///// <summary>
        ///// shows how to handle response when you exceed Search query rate limits
        ///// </summary>
        ///// <param name="twitterCtx"></param>
        //private static void ExceedSearchRateLimitDemo(TwitterContext twitterCtx)
        //{
        //    //
        //    // WARNING: This is for Test/Demo purposes only; 
        //    //          it makes many queries to Twitter in
        //    //          a very short amount of time, which
        //    //          has a negative impact on the service.
        //    //
        //    //          The only reason it is here is to test
        //    //          that LINQ to Twitter responds appropriately
        //    //          to Search rate limits.
        //    //

        //    var queryResults =
        //        from search in twitterCtx.Search
        //        where search.Type == SearchType.Search &&
        //              search.Query == "Testing Search Rate Limit Results"
        //        select search;

        //    try
        //    {
        //        // set to a sufficiently high number to force the HTTP 503 response
        //        // -- assumes you have the bandwidth to exceed
        //        //    limit, which you might not have
        //        var searchesToPerform = 5;

        //        for (int i = 0; i < searchesToPerform; i++)
        //        {
        //            foreach (var search in queryResults)
        //            {
        //                // here, you can see that properties are named
        //                // from the perspective of atom feed elements
        //                // i.e. the query string is called Title
        //                Console.WriteLine("\n#{0}. Query:{1}\n", i+1, search.Title);

        //                foreach (var entry in search.Entries)
        //                {
        //                    Console.WriteLine(
        //                        "ID: {0}, Source: {1}\nContent: {2}\n",
        //                        entry.ID, entry.Source, entry.Content);
        //                }
        //            } 
        //        }
        //    }
        //    catch (TwitterQueryException tqe)
        //    {
        //        if (tqe.HttpError == "503")
        //        {
        //            Console.WriteLine("HTTP Error: {0}", tqe.HttpError);
        //            Console.WriteLine("You've exceeded rate limits for search.");
        //            Console.WriteLine("Please retry in {0} seconds.", twitterCtx.RetryAfter);
        //        }
        //    }

        //    Console.WriteLine("\nComplete.");
        //}

        #endregion
    }
}
