using System;
using System.Linq;

using LinqToTwitter;

namespace LinqToTwitterDemo
{
    public class SearchDemos
    {
        public static void Run(TwitterContext twitterCtx)
        {
            string geocode_melb = "37.7833,144.9667,60km";


            var srch =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.Query == "place:247f43d441defc03"
                 select search)
                .Single();

            Console.WriteLine("\nQuery: {0}\n", srch.QueryResult);
            srch.Results.ForEach(entry =>
                Console.WriteLine(
                    "ID: {0, -15}, Source: {1}\nContent: {2}\n",
                    entry.ID, entry.Source, entry.Text));

            Console.WriteLine("\n More Search demos can be downloaded from LINQ to Twitter's on-line samples at http://linqtotwitter.codeplex.com/wikipage?title=LINQ%20to%20Twitter%20Samples&referringTitle=Home");
        }
    }
}
