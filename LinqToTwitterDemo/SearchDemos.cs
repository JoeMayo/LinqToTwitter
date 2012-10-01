using System;
using System.Linq;

using LinqToTwitter;

namespace LinqToTwitterDemo
{
    public class SearchDemos
    {
        public static void Run(TwitterContext twitterCtx)
        {
            BasicSearchSample(twitterCtx);
            //AsyncSearchSample(twitterCtx);
        }

        static void BasicSearchSample(TwitterContext twitterCtx)
        {
            var srch =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "LINQ To Twitter" &&
                       search.Count == 4
                 select search)
                .Single();

            Console.WriteLine("\nQuery: {0}\n", srch.SearchMetaData.Query);
            srch.Statuses.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                    entry.ID, entry.Source, entry.Text));

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
                    Exception ex = resp.Error;
                    // handle error
                    throw ex;
                }

                Search srch = resp.State.First();
                Console.WriteLine("\nQuery: {0}\n", srch.SearchMetaData.Query);

                srch.Statuses.ForEach(entry =>
                    Console.WriteLine(
                        "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                        entry.ID, entry.Source, entry.Text));

                Console.WriteLine("\n More Search demos can be downloaded from LINQ to Twitter's on-line samples at http://linqtotwitter.codeplex.com/wikipage?title=LINQ%20to%20Twitter%20Samples&referringTitle=Home");
            });
        }
    }
}
