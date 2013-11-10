using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using LitJson;

namespace Linq2TwitterDemos_Console
{
    class StreamingDemos
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
            Console.WriteLine();
            Console.WriteLine("\t Q. Return to Main menu");
        }

        private static async Task DoFilterStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Filter &&
                       strm.Track == "twitter"
                 select strm)
                .StartAsync(async strm =>
                {
                    Console.WriteLine(strm.Content + "\n");

                    if (count++ >= 5)
                        strm.CloseStream();
                });
        }
  
        static async Task DoSampleStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Sample
                 select strm)
                .StartAsync(async strm =>
                {
                    Console.WriteLine(strm.Content + "\n");

                    if (count++ >= 5)
                    {
                        strm.CloseStream();
                    }
                });
        }
  
        static async Task DoUserStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.User
                 select strm)
                .StartAsync(async strm =>
                {
                    string message = string.IsNullOrEmpty(strm.Content) ? "Keep-Alive" : strm.Content;
                    Console.WriteLine((count + 1).ToString() + ". " + DateTime.Now + ": " + message + "\n");

                    if (count++ == 5)
                        strm.CloseStream();
                });
        }
  
        static async Task DoSiteStreamAsync(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Site &&
                       strm.Follow == "15411837,16761255"
                 select strm)
                .StartAsync(async strm =>
                {
                    string message = string.IsNullOrEmpty(strm.Content) ? "Keep-Alive" : strm.Content;
                    Console.WriteLine((count + 1).ToString() + ". " + DateTime.Now + ": " + message + "\n");

                    if (count++ == 5)
                        strm.CloseStream();
                });
        }
  
        static async Task DoControlStreamAsync(TwitterContext twitterCtx)
        {
            var evt = new ManualResetEventSlim(false);
            string streamID = string.Empty;

            Task.Run(async () =>
            {
                Console.WriteLine("\nStreamed Content: \n");
                int count = 0;

                await
                    (from strm in twitterCtx.Streaming
                     where strm.Type == StreamingType.Site &&
                           strm.Follow == "15411837,16761255"
                     select strm)
                    .StartAsync(async strm =>
                    {
                        Console.WriteLine(strm.Content + "\n");

                        var json = JsonMapper.ToObject(strm.Content);
                        var jsonDict = json as IDictionary<string, JsonData>;

                        if (jsonDict != null && jsonDict.ContainsKey("control"))
                        {
                            streamID = json["control"]["control_uri"].ToString().Replace("/1.1/site/c/", "");
                            evt.Set();
                        }

                        if (count++ == 5)
                            strm.CloseStream();
                    });
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

            var ctrlStrmInfo =
                (from strm in twitterCtx.ControlStream
                 where strm.Type == ControlStreamType.Info &&
                       strm.StreamID == streamID
                 select strm)
                .SingleOrDefault();

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

            PrintUserInfo(twitterCtx, streamID);

            ControlStream csAdd = await twitterCtx.AddSiteStreamUserAsync(new List<ulong> { 16761255 }, streamID);
            Console.WriteLine("Command Response: " + csAdd.CommandResponse);
            Console.WriteLine("\nAfter Adding a User: ");
            PrintUserInfo(twitterCtx, streamID);

            ControlStream csRemove = await twitterCtx.RemoveSiteStreamUserAsync(new List<ulong> { 16761255 }, streamID);
            Console.WriteLine("Command Response: " + csRemove.CommandResponse);
            Console.WriteLine("\nAfter Removing a User: ");
            PrintUserInfo(twitterCtx, streamID);
        }

        static async Task PrintUserInfo(TwitterContext twitterCtx, string streamID)
        {
            var ctrlStrm =
                await
                (from strm in twitterCtx.ControlStream
                 where strm.Type == ControlStreamType.Info &&
                       strm.StreamID == streamID
                 select strm)
                .SingleOrDefaultAsync();

            Console.WriteLine("\nInfo Details:\n");

            ControlStreamInfo info = ctrlStrm.Info;
            foreach (var user in info.Users)
                Console.WriteLine("User ID: {0}, Name: {1}", user.UserID, user.Name);

            Console.WriteLine();
        }

    }
}
