using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using System.IO;
using LinqToTwitter.Common;
using System.Collections.Generic;
using System.Net;

namespace ConsoleDemo.CSharp
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
                        Console.WriteLine("\n\tGetting Stream Rules...\n");
                        await GetStreamRulesAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tValidating Stream Rules...\n");
                        await ValidateRulesAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tAdding Stream Rules...\n");
                        await AddRulesAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tDeleting Stream Rules...\n");
                        await DeleteRulesAsync(twitterCtx);
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
            Console.WriteLine("\t 3. Validate Rules");
            Console.WriteLine("\t 4. Add Rules");
            Console.WriteLine("\t 5. Delete Rules");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task DoFilterStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");

            int retries = 3;
            int count = 0;
            var cancelTokenSrc = new CancellationTokenSource();

            do
            {
                try
                {
                    await
                        (from strm in twitterCtx.Streaming
                                                .WithCancellation(cancelTokenSrc.Token)
                         where strm.Type == StreamingType.Filter &&
                               strm.TweetFields == TweetField.AllFieldsExceptPermissioned
                         select strm)
                        .StartAsync(async strm =>
                        {
                            await HandleStreamResponse(strm);

                            if (count++ >= 5)
                                cancelTokenSrc.Cancel();
                        });

                    retries = 0;
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
                    retries--;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Stream cancelled.");
                    retries = 0;
                }
                catch (TwitterQueryException tqe) when (tqe.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    int millisecondsToDelay = 1000 * (4 - retries);
                    retries--;

                    string message = retries >= 0 ?
                        $"Tried to reconnect too quickly. Delaying for {millisecondsToDelay} milliseconds..."
                        :
                        "Too many retries. Stopping query.";

                    Console.WriteLine(message);

                    await Task.Delay(millisecondsToDelay);
                }
            } while (retries > 0);
        }

        static async Task DoSampleStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");

            int retries = 3;
            int count = 0;
            var cancelTokenSrc = new CancellationTokenSource();

            do
            {
                try
                {
                    await
                        (from strm in twitterCtx.Streaming
                                                .WithCancellation(cancelTokenSrc.Token)
                         where strm.Type == StreamingType.Sample
                         select strm)
                        .StartAsync(async strm =>
                        {
                            await HandleStreamResponse(strm);

                            if (count++ >= 10)
                                cancelTokenSrc.Cancel();
                        });

                    retries = 0;
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
                    retries--;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Stream cancelled.");
                    retries = 0;
                }
                catch (TwitterQueryException tqe) when (tqe.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    int millisecondsToDelay = 1000 * (4 - retries);
                    retries--;

                    string message = retries >= 0 ?
                        $"Tried to reconnect too quickly. Delaying for {millisecondsToDelay} milliseconds..."
                        :
                        "Too many retries. Stopping query.";

                    Console.WriteLine(message);

                    await Task.Delay(millisecondsToDelay);
                } 
            } while (retries > 0);
        }

        static async Task<int> HandleStreamResponse(StreamContent strm)
        {
            if (strm.HasError)
            {
                Console.WriteLine($"Error during streaming: {strm.ErrorMessage}");
            }
            else
            {
                Tweet? tweet = strm?.Entity?.Tweet;
                if (tweet != null)
                    Console.WriteLine($"\n{tweet.CreatedAt}, Tweet ID: {tweet.ID}, Tweet Text: {tweet.Text}");
            }

            return await Task.FromResult(0);
        }

        static async Task GetStreamRulesAsync(TwitterContext twitterCtx)
        {
            Streaming? streaming =
                await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Rules
                 select strm)
                .SingleOrDefaultAsync();

            Console.WriteLine("\nRules: \n");

            if (streaming?.Rules != null)
                streaming.Rules.ForEach(rule =>
                    Console.WriteLine(
                        $"\nID:    {rule.ID}" +
                        $"\nValue: {rule.Value}" +
                        $"\nTag:   {rule.Tag}"));
            else
                Console.WriteLine("No entries found.");
        }

        static async Task ValidateRulesAsync(TwitterContext twitterCtx)
        {
            var rules = new List<StreamingAddRule>
            {
                new StreamingAddRule { Tag = "memes with media", Value = "meme has:images" },
                new StreamingAddRule { Tag = "cats with media", Value = "cat has:media" }
            };

            Streaming? result = await twitterCtx.AddStreamingFilterRulesAsync(rules, isValidateOnly: true);

            if (result?.Meta?.Summary != null)
            {
                StreamingMeta meta = result.Meta;
                Console.WriteLine($"\nSent: {meta.Sent}");

                StreamingMetaSummary summary = meta.Summary;

                Console.WriteLine($"Created:  {summary.Created}");
                Console.WriteLine($"!Created: {summary.NotCreated}");
            }

            if (result?.Errors != null && result.HasErrors)
                result.Errors.ForEach(error =>
                    Console.WriteLine(
                        $"\nTitle: {error.Title}" +
                        $"\nValue: {error.Value}" +
                        $"\nID:    {error.ID}" +
                        $"\nType:  {error.Type}"));
        }

        static async Task AddRulesAsync(TwitterContext twitterCtx)
        {
            var rules = new List<StreamingAddRule>
            {
                new StreamingAddRule { Tag = "funny things", Value = "meme" },
                new StreamingAddRule { Tag = "happy cats with media", Value = "cat has:media -grumpy" }
            };

            Streaming? result = await twitterCtx.AddStreamingFilterRulesAsync(rules);

            StreamingMeta? meta = result?.Meta;

            if (meta?.Summary != null)
            {
                Console.WriteLine($"\nSent: {meta.Sent}");

                StreamingMetaSummary summary = meta.Summary;

                Console.WriteLine($"Created:  {summary.Created}");
                Console.WriteLine($"!Created: {summary.NotCreated}");
            }

            if (result?.Errors != null && result.HasErrors)
                result.Errors.ForEach(error => 
                    Console.WriteLine(
                        $"\nTitle: {error.Title}" +
                        $"\nValue: {error.Value}" +
                        $"\nID:    {error.ID}" +
                        $"\nType:  {error.Type}"));
        }

        static async Task DeleteRulesAsync(TwitterContext twitterCtx)
        {
            var ruleIds = new List<string>
            {
                "1165037377523306498",
                "1165037377523306499"
            };

            Streaming? result = await twitterCtx.DeleteStreamingFilterRulesAsync(ruleIds);

            if (result?.Meta?.Summary != null)
            {
                StreamingMeta meta = result.Meta;
                Console.WriteLine($"\nSent: {meta.Sent}");

                StreamingMetaSummary summary = meta.Summary;

                Console.WriteLine($"Deleted:  {summary.Deleted}");
                Console.WriteLine($"!Deleted: {summary.NotDeleted}");
            }

            if (result?.Errors != null && result.HasErrors)
                result.Errors.ForEach(error =>
                    Console.WriteLine(
                        $"\nTitle: {error.Title}" +
                        $"\nValue: {error.Value}" +
                        $"\nID:    {error.ID}" +
                        $"\nType:  {error.Type}"));
        }
    }
}
