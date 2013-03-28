using System;
using System.Linq;
using System.Net;
using System.Threading;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    public class StreamingDemo
    {
        public static void Run(TwitterContext twitterCtx)
        {
            //SamplesDemo(twitterCtx);
            //FilterDemo(twitterCtx);
            //UserStreamDemo(twitterCtx);
            //UserStreamWithTimeoutDemo(twitterCtx);
            SiteStreamDemo(twitterCtx);
        }

        private static void FilterDemo(TwitterContext twitterCtx)
        {
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

        private static void SamplesDemo(TwitterContext twitterCtx)
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

        private static void UserStreamDemo(TwitterContext twitterCtx)
        {
            twitterCtx.AuthorizedClient.UseCompression = false;
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


        private static void UserStreamWithTimeoutDemo(TwitterContext twitterCtx)
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

        private static void SiteStreamDemo(TwitterContext twitterCtx)
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
    }
}
