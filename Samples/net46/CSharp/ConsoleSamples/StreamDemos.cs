using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using LitJson;
using System.IO;

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
                        Console.WriteLine("\n\tReading Rx Stream...\n");
                        await DoRxObservableStreamAsync(twitterCtx);
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
            Console.WriteLine("\t 2. Reactive Stream");
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
                     where strm.Type == StreamingType.Filter &&
                           strm.Track == "twitter" &&
                           strm.TweetMode == TweetMode.Extended
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
                     where strm.Type == StreamingType.Sample &&
                           strm.TweetMode == TweetMode.Extended
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

        static async Task DoRxObservableStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;
            var cancelTokenSrc = new CancellationTokenSource();

            try
            {
                var observable =
                    await
                        (from strm in twitterCtx.Streaming
                                                .WithCancellation(cancelTokenSrc.Token)
                         where strm.Type == StreamingType.Filter &&
                               strm.Track == "twitter" &&
                               strm.TweetMode == TweetMode.Extended
                         select strm)
                        .ToObservableAsync();

                observable.Subscribe(
                    async strm =>
                    {
                        await HandleStreamResponse(strm);

                        if (count++ >= 5)
                            cancelTokenSrc.Cancel();
                    },
                    ex => Console.WriteLine(ex.ToString()),
                    () => Console.WriteLine("Completed"));
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Stream cancelled.");
            }
        }

        static async Task PrintUserInfoAsync(TwitterContext twitterCtx, string streamID)
        {
            var ctrlStrm =
                await
                (from strm in twitterCtx.ControlStream
                 where strm.Type == ControlStreamType.Info &&
                       strm.StreamID == streamID
                 select strm)
                .SingleOrDefaultAsync();

            if (ctrlStrm != null && 
                ctrlStrm.Info != null && 
                ctrlStrm.Info.Users != null)
            {
                Console.WriteLine("\nInfo Details:\n");

                ControlStreamInfo info = ctrlStrm.Info;
                foreach (var user in info.Users)
                    Console.WriteLine("User ID: {0}, Name: {1}", user.UserID, user.Name);

                Console.WriteLine();
            } 
        }

        static async Task<int> HandleStreamResponse(StreamContent strm)
        {
            switch (strm.EntityType)
            {
                case StreamEntityType.Control:
                    var control = strm.Entity as Control;
                    Console.WriteLine("Control URI: {0}", control.URL);
                    break;
                case StreamEntityType.Delete:
                    var delete = strm.Entity as Delete;
                    Console.WriteLine("Delete - User ID: {0}, Status ID: {1}", delete.UserID, delete.StatusID);
                    break;
                case StreamEntityType.DirectMessage:
                    var dm = strm.Entity as DirectMessage;
                    Console.WriteLine("Direct Message - Sender: {0}, Text: {1}", dm.Sender, dm.Text);
                    break;
                case StreamEntityType.Disconnect:
                    var disconnect = strm.Entity as Disconnect;
                    Console.WriteLine("Disconnect - {0}", disconnect.Reason);
                    break;
                case StreamEntityType.Event:
                    var evt = strm.Entity as Event;
                    Console.WriteLine("Event - Event Name: {0}", evt.EventName);
                    break;
                case StreamEntityType.ForUser:
                    var user = strm.Entity as ForUser;
                    Console.WriteLine("For User - User ID: {0}, # Friends: {1}", user.UserID, user.Friends.Count);
                    break;
                case StreamEntityType.FriendsList:
                    var friends = strm.Entity as FriendsList;
                    Console.WriteLine("Friends List - # Friends: {0}", friends.Friends.Count);
                    break;
                case StreamEntityType.GeoScrub:
                    var scrub = strm.Entity as GeoScrub;
                    Console.WriteLine("GeoScrub - User ID: {0}, Up to Status ID: {1}", scrub.UserID, scrub.UpToStatusID);
                    break;
                case StreamEntityType.Limit:
                    var limit = strm.Entity as Limit;
                    Console.WriteLine("Limit - Track: {0}", limit.Track);
                    break;
                case StreamEntityType.Stall:
                    var stall = strm.Entity as Stall;
                    Console.WriteLine("Stall - Code: {0}, Message: {1}, % Full: {2}", stall.Code, stall.Message, stall.PercentFull);
                    break;
                case StreamEntityType.Status:
                    var status = strm.Entity as Status;
                    Console.WriteLine("Status - @{0}: {1}", status.User.ScreenNameResponse, status.Text ?? status.FullText);
                    break;
                case StreamEntityType.StatusWithheld:
                    var statusWithheld = strm.Entity as StatusWithheld;
                    Console.WriteLine("Status Withheld - Status ID: {0}, # Countries: {1}", statusWithheld.StatusID, statusWithheld.WithheldInCountries.Count);
                    break;
                case StreamEntityType.TooManyFollows:
                    var follows = strm.Entity as TooManyFollows;
                    Console.WriteLine("Too Many Follows - Message: {0}", follows.Message);
                    break;
                case StreamEntityType.UserWithheld:
                    var userWithheld = strm.Entity as UserWithheld;
                    Console.WriteLine("User Withheld - User ID: {0}, # Countries: {1}", userWithheld.UserID, userWithheld.WithheldInCountries.Count);
                    break;
                case StreamEntityType.ParseError:
                    var unparsedJson = strm.Entity as string;
                    Console.WriteLine("Parse Error - {0}", unparsedJson);
                    break;
                case StreamEntityType.Unknown:
                default:
                    Console.WriteLine("Unknown - " + strm.Content + "\n");
                    break;
            }

            return await Task.FromResult(0);
        }
    }
}
