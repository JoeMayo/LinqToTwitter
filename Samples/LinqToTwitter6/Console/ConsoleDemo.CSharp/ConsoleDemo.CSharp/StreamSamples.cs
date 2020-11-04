using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using System.IO;
using LinqToTwitter.Common;

namespace Linq2TwitterDemos_Console
{
    class StreamDemos
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
                        Console.WriteLine("\n\tShowing Filter Stream...\n");
                        await DoFilterStreamAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tShowing Sample Stream...\n");
                        await DoSampleStreamAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tShowing Stream Rules...\n");
                        await GetStreamRulesAsync(twitterCtx);
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
            Console.WriteLine("\nStreaming Demos - Please select:\n");

            Console.WriteLine("\t 0. Filter Stream");
            Console.WriteLine("\t 1. Sample Stream");
            Console.WriteLine("\t 2. Stream Rules");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task DoFilterStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;
            var cancelTokenSrc = new CancellationTokenSource();

            try
            {
                await
                    (from strm in twitterCtx.Streaming
                                            .WithCancellation(cancelTokenSrc.Token)
                     where strm.Type == StreamingType.Filter
                     select strm)
                    .StartAsync(async strm =>
                    {
                        await HandleStreamResponse(strm);

                        if (count++ >= 5)
                            cancelTokenSrc.Cancel();
                    });
            }
            catch (IOException ex)
            {
                // Twitter might have closed the stream,
                // which they do sometimes. You should
                // restart the stream, but be sure to
                // read Twitter documentation on stream
                // back-off strategies to prevent your
                // app from being blocked.
                Console.WriteLine(ex.ToString());
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }

        static async Task DoSampleStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;
            var cancelTokenSrc = new CancellationTokenSource();

            try
            {
                await
                    (from strm in twitterCtx.Streaming.WithCancellation(cancelTokenSrc.Token)
                     where strm.Type == StreamingType.Sample
                     select strm)
                    .StartAsync(async strm =>
                    {
                        await HandleStreamResponse(strm);

                        if (count++ >= 10)
                            cancelTokenSrc.Cancel();
                    });
            }
            catch (IOException ex)
            {
                // Twitter might have closed the stream,
                // which they do sometimes. You should
                // restart the stream, but be sure to
                // read Twitter documentation on stream
                // back-off strategies to prevent your
                // app from being blocked.
                Console.WriteLine(ex.ToString());
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }

        static async Task<int> HandleStreamResponse(StreamContent strm)
        {
            if (strm.HasError)
            {
                Console.WriteLine($"Error during streaming: {strm.ErrorMessage}");
            }
            else
            {
                Tweet? tweet = strm.Entity;
                if (tweet != null)
                    Console.WriteLine($"Tweet ID: {tweet.ID}, Tweet Text: {tweet.Text}");
            }

            return await Task.FromResult(0);
        }

        static async Task GetStreamRulesAsync(TwitterContext twitterCtx)
        {
            Streaming? streaming =
                await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Rules &&
                       strm.Ids == "100,150"
                 select strm)
                .SingleOrDefaultAsync();

            if (streaming?.Rules != null)
                streaming.Rules.ForEach(rule =>
                    Console.WriteLine(
                        $"\nID:    {rule.ID}" +
                        $"\nValue: {rule.Value}" +
                        $"\nTag:   {rule.Tag}"));
            else
                Console.WriteLine("No entries found.");
        }
    }
}
