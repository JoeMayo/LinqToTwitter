using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using System.Collections.Generic;
using System.Diagnostics;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
{
    public class CountsDemos
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
                        Console.WriteLine("\n\tGetting recent counts...\n");
                        await DoCountsRecentAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tGetting all counts...\n");
                        await DoCountsAllAsync(twitterCtx);
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
            Console.WriteLine("\nCounts Demos - Please select:\n");

            Console.WriteLine("\t 0. Recent Counts");
            Console.WriteLine("\t 1. All Counts");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task DoCountsRecentAsync(TwitterContext twitterCtx)
        {
            string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter OR JoeMayo";
            //searchTerm = "Twitter";

            Counts? countsResponse =
                await
                (from count in twitterCtx.Counts
                 where count.Type == CountType.Recent &&
                       count.Query == searchTerm &&
                       count.Granularity == Granularity.Day
                 select count)
                .SingleOrDefaultAsync();

            if (countsResponse?.CountRanges != null)
                countsResponse.CountRanges.ForEach(range =>
                    Console.WriteLine(
                        $"\nStart: {range.Start}" +
                        $"\nEnd:   {range.End}" +
                        $"\nTweet: {range.TweetCount}"));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task DoCountsAllAsync(TwitterContext twitterCtx)
        {
            string searchTerm = "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter OR JoeMayo";
            //searchTerm = "Twitter";

            Counts? countsResponse =
                await
                (from count in twitterCtx.Counts
                 where count.Type == CountType.All &&
                       count.Query == searchTerm &&
                       count.Granularity == Granularity.Day
                 select count)
                .SingleOrDefaultAsync();

            if (countsResponse?.CountRanges != null)
                countsResponse.CountRanges.ForEach(range =>
                    Console.WriteLine(
                        $"\nStart: {range.Start}" +
                        $"\nEnd:   {range.End}" +
                        $"\nTweet: {range.TweetCount}"));
            else
                Console.WriteLine("No entries found.");
        }
    }
}
