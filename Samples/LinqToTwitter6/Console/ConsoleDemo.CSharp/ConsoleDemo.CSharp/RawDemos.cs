using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
{
    class RawDemos
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
                        Console.WriteLine("\n\tSearching Recent Tweets...\n");
                        await PerformRecentSearchRawAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tSearching Standard Tweets...\n");
                        await PerformStandardSearchRawAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tTweeting...");
                        await TweetRawAsync(twitterCtx);
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
            Console.WriteLine("\nRaw Demos - Please select:\n");

            Console.WriteLine("\t 0. Perform Recent Search (Query)");
            Console.WriteLine("\t 1. Perform Standard Search (Query)");
            Console.WriteLine("\t 2. Update Status (Command)");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task PerformRecentSearchRawAsync(TwitterContext twitterCtx)
        {
            string unencodedStatus = "JoeMayo";
            string encodedStatus = Uri.EscapeDataString(unencodedStatus);
            string queryString = "tweets/search/recent?query=" + encodedStatus;

            string previousBaseUrl = twitterCtx.BaseUrl;
            twitterCtx.BaseUrl = "https://api.twitter.com/2/";

            var rawResult =
                await
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == queryString
                 select raw)
                .SingleOrDefaultAsync();

            if (rawResult != null)
                Console.WriteLine(
                    "Response from Twitter: \n\n" + rawResult.Response);

            twitterCtx.BaseUrl = previousBaseUrl;
        }

        static async Task PerformStandardSearchRawAsync(TwitterContext twitterCtx)
        {
            string unencodedStatus = "LINQ to Twitter";
            string encodedStatus = Uri.EscapeDataString(unencodedStatus);
            string queryString = "search/tweets.json?q=" + encodedStatus;

            var rawResult =
                await
                (from raw in twitterCtx.RawQuery
                 where raw.QueryString == queryString
                 select raw)
                .SingleOrDefaultAsync();

            if (rawResult != null)
                Console.WriteLine(
                    "Response from Twitter: \n\n" + rawResult.Response);
        }

        static async Task TweetRawAsync(TwitterContext twitterCtx)
        {
            string status = 
                "Testing LINQ to Twitter Raw Interface - " + 
                DateTime.Now.ToString() + " #Linq2Twitter";
            var parameters = new Dictionary<string, string>
            {
                { "status", status }
            };

            string queryString = "/statuses/update.json";

            string result = 
                await twitterCtx.ExecuteRawAsync(
                    queryString, parameters, HttpMethod.Post);

            if (result != null)
                Console.WriteLine(
                    "\nResponse from update status: \n\n\t" + result);
        }
    }
}
