using System;
using System.Linq;
using System.Net;
using System.Threading;
using LinqToTwitter;
using LitJson;
using System.Collections;
using System.Collections.Generic;

namespace LinqToTwitterDemo
{
    public class StreamingDemo
    {
        public static void Run(TwitterContext twitterCtx)
        {
            SamplesDemo(twitterCtx);
            //FilterDemo(twitterCtx);
            //UserStreamDemo(twitterCtx);
            //UserStreamWithTimeoutDemo(twitterCtx);
            //SiteStreamDemo(twitterCtx);
            //ControlStreamsFollowersDemo(twitterCtx);
            //ControlStreamsInfoDemo(twitterCtx);
            //ControlStreamsAddRemoveDemo(twitterCtx);
        }

        static void FilterDemo(TwitterContext twitterCtx)
        {
            twitterCtx.AuthorizedClient.UseCompression = false;
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.Streaming
             where strm.Type == StreamingType.Filter &&
                   strm.Track == "twitter,JoeMayo,linq2twitter,microsoft,google,oracle"
             select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status != TwitterErrorStatus.Success)
                {
                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 2)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
        }

        static void SamplesDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.Streaming
             where strm.Type == StreamingType.Sample
             select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status == TwitterErrorStatus.RequestProcessingException)
                {
                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 10)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
        }

        static void UserStreamDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            // the user stream is for whoever is authenticated
            // via the Authenticator passed to TwitterContext
            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.User
             select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status == TwitterErrorStatus.RequestProcessingException)
                {
                    WebException wex = strm.Error as WebException;
                    if (wex != null && wex.Status == WebExceptionStatus.ConnectFailure)
                    {
                        Console.WriteLine(wex.Message + " You might want to reconnect.");
                    }

                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                string message = string.IsNullOrEmpty(strm.Content) ? "Keep-Alive" : strm.Content;
                Console.WriteLine((count + 1).ToString() + ". " + DateTime.Now + ": " + message + "\n");

                if (count++ == 25)
                {
                    Console.WriteLine("Demo is ending. Closing stream...");
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
        }

        static void UserStreamWithTimeoutDemo(TwitterContext twitterCtx)
        {
            twitterCtx.AuthorizedClient.UseCompression = false;
            twitterCtx.ReadWriteTimeout = 3000;
            StreamContent strmCont = null;

            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            // the user stream is for whoever is authenticated
            // via the Authenticator passed to TwitterContext
            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.User
             select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status != TwitterErrorStatus.Success)
                {
                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                strmCont = strm;
                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 25)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();

            while (strmCont == null)
            {
                Console.WriteLine("Waiting on stream to initialize.");

                Thread.Sleep(10000);
            }

            Console.WriteLine("Stream is initialized. Now closing...");
            strmCont.CloseStream();
        }

        static void SiteStreamDemo(TwitterContext twitterCtx)
        {
            twitterCtx.AuthorizedClient.UseCompression = false;
            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.Site &&
                   //strm.With == "followings" &&
                   strm.Follow == "15411837"/*, "16761255"*/
             select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status == TwitterErrorStatus.RequestProcessingException)
                {
                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                Console.WriteLine(strm.Content + "\n");

                if (count++ >= 10)
                {
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();
        }

        static void ControlStreamsFollowersDemo(TwitterContext twitterCtx)
        {
            var evt = new ManualResetEventSlim(false);
            string streamID = string.Empty;

            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.Site &&
                 //strm.With == "followings" &&
                   strm.Follow == "15411837"/*, "16761255"*/
             select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status == TwitterErrorStatus.RequestProcessingException)
                {
                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                Console.WriteLine(strm.Content + "\n");

                var json = JsonMapper.ToObject(strm.Content);
                var jsonDict = json as IDictionary<string, JsonData>;

                if (jsonDict != null && jsonDict.ContainsKey("control"))
                {
                    streamID = json["control"]["control_uri"].ToString().Replace("/1.1/site/c/", "");
                    evt.Set();
                }

                if (count++ >= 10)
                {
                    Console.WriteLine("Closing stream...");
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();

            evt.Wait();
            Console.WriteLine("Follower Details:\n");
            var ctrlStrm =
                (from strm in twitterCtx.ControlStream
                 where strm.Type == ControlStreamType.Followers &&
                       strm.UserID == 15411837 &&
                       strm.StreamID == streamID
                 select strm)
                .SingleOrDefault();

            ControlStreamFollow follow = ctrlStrm.Follow;
            ControlStreamUser user = follow.User;
            List<ulong> friends = follow.Friends;
            Cursors cursors = follow.Cursors;

            Console.WriteLine("User ID: " + user.UserID);
            Console.WriteLine("User Name: " + user.Name);
            Console.WriteLine("Can DM: " + user.DM);
            friends.ForEach(friend => Console.WriteLine(friend));
            Console.WriteLine("Prev Cursor: " + cursors.Previous);
            Console.WriteLine("Next Cursor: " + cursors.Next);
        }

        static void ControlStreamsInfoDemo(TwitterContext twitterCtx)
        {
            var evt = new ManualResetEventSlim(false);
            string streamID = string.Empty;

            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.Site &&
                 //strm.With == "followings" &&
                   strm.Follow == "15411837"/*, "16761255"*/
             select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status == TwitterErrorStatus.RequestProcessingException)
                {
                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                Console.WriteLine(strm.Content + "\n");

                var json = JsonMapper.ToObject(strm.Content);
                var jsonDict = json as IDictionary<string, JsonData>;

                if (jsonDict != null && jsonDict.ContainsKey("control"))
                {
                    streamID = json["control"]["control_uri"].ToString().Replace("/1.1/site/c/", "");
                    evt.Set();
                }

                if (count++ >= 10)
                {
                    Console.WriteLine("Closing stream...");
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();

            evt.Wait();
            Console.WriteLine("Info Details:\n");
            var ctrlStrm =
                (from strm in twitterCtx.ControlStream
                 where strm.Type == ControlStreamType.Info &&
                       strm.StreamID == streamID
                 select strm)
                .SingleOrDefault();

            ControlStreamInfo info = ctrlStrm.Info;
            ControlStreamUser user = info.Users.First();
            Console.WriteLine("User ID: " + user.UserID);
            Console.WriteLine("User Name: " + user.Name);
            Console.WriteLine("Can DM: " + user.DM);
            Console.WriteLine("Delimited: " + info.Delimited);
            Console.WriteLine("Include Followings Acitity: " + info.IncludeFollowingsActivity);
            Console.WriteLine("Include User Changes: " + info.IncludeUserChanges);
            Console.WriteLine("Replies: " + info.Replies);
            Console.WriteLine("With: " + info.With);
        }

        static void ControlStreamsAddRemoveDemo(TwitterContext twitterCtx)
        {
            var evt = new ManualResetEventSlim(false);
            string streamID = string.Empty;

            Console.WriteLine("\nStreamed Content: \n");
            int count = 0;

            (from strm in twitterCtx.UserStream
             where strm.Type == UserStreamType.Site &&
                 //strm.With == "followings" &&
                   strm.Follow == "15411837"/*, "16761255"*/
             select strm)
            .StreamingCallback(strm =>
            {
                if (strm.Status == TwitterErrorStatus.RequestProcessingException)
                {
                    Console.WriteLine(strm.Error.ToString());
                    return;
                }

                Console.WriteLine(strm.Content + "\n");

                var json = JsonMapper.ToObject(strm.Content);
                var jsonDict = json as IDictionary<string, JsonData>;

                if (jsonDict != null && jsonDict.ContainsKey("control"))
                {
                    streamID = json["control"]["control_uri"].ToString().Replace("/1.1/site/c/", "");
                    evt.Set();
                }

                if (count++ >= 10)
                {
                    Console.WriteLine("Closing stream...");
                    strm.CloseStream();
                }
            })
            .SingleOrDefault();

            evt.Wait();

            Console.WriteLine("\nInitial Stream Users: ");
            PrintUserInfo(twitterCtx, streamID);

            ControlStream csAdd = twitterCtx.AddSiteStreamUser(new List<ulong> { 16761255 }, streamID);
            Console.WriteLine("Command Response: " + csAdd.CommandResponse);
            Console.WriteLine("\nAfter Adding a User: ");
            PrintUserInfo(twitterCtx, streamID);

            ControlStream csRemove = twitterCtx.RemoveSiteStreamUser(new List<ulong> { 15411837 }, streamID);
            Console.WriteLine("Command Response: " + csAdd.CommandResponse);
            Console.WriteLine("\nAfter Removing a User: ");
            PrintUserInfo(twitterCtx, streamID);
        }

        static void PrintUserInfo(TwitterContext twitterCtx, string streamID)
        {
            var ctrlStrm =
                (from strm in twitterCtx.ControlStream
                 where strm.Type == ControlStreamType.Info &&
                       strm.StreamID == streamID
                 select strm)
                .SingleOrDefault();

            Console.WriteLine("\nInfo Details:\n");

            ControlStreamInfo info = ctrlStrm.Info;
            foreach (var user in info.Users)
            {
                Console.WriteLine("User ID: {0}, Name: {1}", user.UserID, user.Name); 
            }

            Console.WriteLine();
        }
    }
}
