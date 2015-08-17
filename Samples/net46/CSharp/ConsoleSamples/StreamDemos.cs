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
                        Console.WriteLine("\n\tShowing User Stream...\n");
                        await DoUserStreamAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tShowing Site Stream...\n");
                        await DoSiteStreamAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tUsing Control Stream...\n");
                        await DoControlStreamAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tReading Rx Stream...\n");
                        //await DoRxObservableStreamAsync(twitterCtx);
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
            Console.WriteLine("\t 2. User Stream");
            Console.WriteLine("\t 3. Site Stream");
            Console.WriteLine("\t 4. Control Stream");
            Console.WriteLine("\t 5. Reactive Stream");
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
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
                           strm.Track == "twitter"
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
  
        static async Task DoUserStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;
            var cancelTokenSrc = new CancellationTokenSource();

            try
            {
                await
                    (from strm in twitterCtx.Streaming
                     where strm.Type == StreamingType.User
                     select strm)
                    .WithCancellation(cancelTokenSrc.Token)
                    .StartAsync(async strm =>
                    {
                        if (string.IsNullOrEmpty(strm.Content))
                            Console.WriteLine("Keep-Alive");
                        else
                            await HandleStreamResponse(strm);

                        if (count++ == 25)
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
  
        static async Task DoSiteStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;
            var cancelTokenSrc = new CancellationTokenSource();

            try
            {
                await
                    (from strm in twitterCtx.Streaming.WithCancellation(cancelTokenSrc.Token)
                     where strm.Type == StreamingType.Site &&
                           strm.Follow == "15411837,16761255"
                     select strm)
                    .StartAsync(async strm =>
                    {
                        if (string.IsNullOrEmpty(strm.Content))
                            Console.WriteLine("Keep-Alive");
                        else
                            await HandleStreamResponse(strm);

                        if (count++ == 25)
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
  
        static async Task DoControlStreamAsync(TwitterContext twitterCtx)
        {
            var evt = new ManualResetEventSlim(false);
            string streamID = string.Empty;

            await Task.Run(async () =>
            {
                Console.WriteLine("\nStreamed Content: \n");
                int count = 0;
                var cancelTokenSrc = new CancellationTokenSource();

                try
                {
                    await
                        (from strm in twitterCtx.Streaming
                         where strm.Type == StreamingType.Site &&
                               strm.Follow == "15411837,16761255"
                         select strm)
                        .WithCancellation(cancelTokenSrc.Token)
                        .StartAsync(async strm =>
                        {
                            if (string.IsNullOrEmpty(strm.Content))
                                Console.WriteLine("Keep-Alive");
                            else
                                await HandleStreamResponse(strm);

                            if (strm.EntityType == StreamEntityType.Control)
                            {
                                var control = strm.Entity as Control;
                                streamID = control.URL.Replace("/1.1/site/c/", "");
                                evt.Set();
                            }

                            if (count++ == 25)
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
            });

            evt.Wait();

            Console.WriteLine("Follower Details:\n");

            var ctrlStrmFollowers =
                await
                (from strm in twitterCtx.ControlStream
                    where strm.Type == ControlStreamType.Followers &&
                        strm.UserID == 15411837 &&
                        strm.StreamID == streamID
                    select strm)
                .SingleOrDefaultAsync();

            if (ctrlStrmFollowers != null)
            {
                ControlStreamFollow follow = ctrlStrmFollowers.Follow;
                ControlStreamUser followUser = follow.User;
                List<ulong> friends = follow.Friends;
                Cursors cursors = follow.Cursors;

                Console.WriteLine("User ID: " + followUser.UserID);
                Console.WriteLine("User Name: " + followUser.Name);
                Console.WriteLine("Can DM: " + followUser.DM);
                friends.ForEach(friend => Console.WriteLine(friend));
                Console.WriteLine("Prev Cursor: " + cursors.Previous);
                Console.WriteLine("Next Cursor: " + cursors.Next);

                Console.WriteLine("Info Details:\n");
            } 
            
            var ctrlStrmInfo =
                (from strm in twitterCtx.ControlStream
                    where strm.Type == ControlStreamType.Info &&
                        strm.StreamID == streamID
                    select strm)
                .SingleOrDefault();

            if (ctrlStrmInfo != null)
            {
                ControlStreamInfo infoUser = ctrlStrmInfo.Info;
                ControlStreamUser user = infoUser.Users.First();
                Console.WriteLine("User ID: " + user.UserID);
                Console.WriteLine("User Name: " + user.Name);
                Console.WriteLine("Can DM: " + user.DM);
                Console.WriteLine("Delimited: " + infoUser.Delimited);
                Console.WriteLine("Include Followings Acitity: " + infoUser.IncludeFollowingsActivity);
                Console.WriteLine("Include User Changes: " + infoUser.IncludeUserChanges);
                Console.WriteLine("Replies: " + infoUser.Replies);
                Console.WriteLine("With: " + infoUser.With);

                Console.WriteLine("\nInitial Stream Users: ");

                await PrintUserInfoAsync(twitterCtx, streamID);

                ControlStream csAdd = await twitterCtx.AddSiteStreamUserAsync(new List<ulong> { 16761255 }, streamID);
                Console.WriteLine("Command Response: " + csAdd.CommandResponse);
                Console.WriteLine("\nAfter Adding a User: ");
                await PrintUserInfoAsync(twitterCtx, streamID);

                ControlStream csRemove = await twitterCtx.RemoveSiteStreamUserAsync(new List<ulong> { 16761255 }, streamID);
                Console.WriteLine("Command Response: " + csRemove.CommandResponse);
                Console.WriteLine("\nAfter Removing a User: ");
                await PrintUserInfoAsync(twitterCtx, streamID);
            }
        }

        //static async Task DoRxObservableStreamAsync(TwitterContext twitterCtx)
        //{
        //    Console.WriteLine("\nStreamed Content: \n");
        //    int count = 0;
        //    var cancelTokenSrc = new CancellationTokenSource();

        //    try
        //    {
        //        var observable =
        //            await
        //                (from strm in twitterCtx.Streaming
        //                                        .WithCancellation(cancelTokenSrc.Token)
        //                 where strm.Type == StreamingType.Filter &&
        //                       strm.Track == "twitter"
        //                 select strm)
        //                .ToObservableAsync();

        //        observable.Subscribe(
        //            strm =>
        //            {
        //                HandleStreamResponse(strm);

        //                if (count++ >= 5)
        //                    cancelTokenSrc.Cancel();
        //            },
        //            ex => Console.WriteLine(ex.ToString()),
        //            () => Console.WriteLine("Completed"));
        //    }
        //    catch (OperationCanceledException)
        //    {
        //        Console.WriteLine("Stream cancelled.");
        //    }
        //}

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
                    Console.WriteLine("Status - @{0}: {1}", status.User.ScreenNameResponse, status.Text);
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
