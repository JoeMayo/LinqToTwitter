using System;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows how to handle errors with LINQ to Twitter
    /// </summary>
    public class ErrorHandlingDemos
    {
        /// <summary>
        /// Run all error handling related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            HandleQueryExceptionDemo(twitterCtx);
            //HandleSideEffectExceptionDemo(twitterCtx);
            //HandleSideEffectWithFilePostExceptionDemo(twitterCtx);
            //HandleTimeoutErrors(twitterCtx);
        }

        /// <summary>
        /// shows how to handle a timeout error
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleTimeoutErrors(TwitterContext twitterCtx)
        {
            // force an unreasonable timeout (1 millisecond)
            twitterCtx.Timeout = 1;

            var searchResponse =
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search
                 select search)
                .SingleOrDefault();

            try
            {
                if (searchResponse != null)
                    searchResponse.Statuses.ForEach(
                        tweet => Console.WriteLine(
                            "User Name: {0}, Tweet: {1}",
                            tweet.User.Identifier.ScreenName,
                            tweet.Text));
            }
            catch (TwitterQueryException tqEx)
            {
                // use your logging and handling logic here

                // notice how the WebException is wrapped as the
                // inner exception of the TwitterQueryException
                Console.WriteLine(tqEx.InnerException.Message);
            }
        }

        /// <summary>
        /// shows how to handle a TwitterQueryException with a side-effect causing a file post
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleSideEffectWithFilePostExceptionDemo(TwitterContext twitterCtx)
        {
            // force the error by supplying bad credentials
            twitterCtx.AuthorizedClient = new PinAuthorizer();

            try
            {
                var user = twitterCtx.UpdateAccountImage(@"C:\Users\jmayo\Pictures\JoeTwitter.jpg", true);
            }
            catch (TwitterQueryException tqe)
            {
                // log it to the console
                Console.WriteLine(
                    "\nHTTP Error Code: {0}\nError: {1}\nRequest: {2}\n",
                    tqe.HttpError,
                    tqe.Response.Error,
                    tqe.Response.Request);
            }
        }

        /// <summary>
        /// shows how to handle a TwitterQueryException with a side-effect
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleSideEffectExceptionDemo(TwitterContext twitterCtx)
        {
            // force the error by supplying bad credentials
            twitterCtx.AuthorizedClient = new PinAuthorizer();

            try
            {
                var status = twitterCtx.UpdateStatus("Test from LINQ to Twitter - 5/2/09");
            }
            catch (TwitterQueryException tqe)
            {
                // log it to the console
                Console.WriteLine(
                    "\nHTTP Error Code: {0}\nError: {1}\nRequest: {2}\n",
                    tqe.HttpError,
                    tqe.Response.Error,
                    tqe.Response.Request);
            }
        }

        /// <summary>
        /// shows how to handle a TwitterQueryException with a query
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void HandleQueryExceptionDemo(TwitterContext twitterCtx)
        {
            // force the error by supplying bad credentials
            twitterCtx.AuthorizedClient = new PinAuthorizer();

            try
            {
                var statuses =
                        from status in twitterCtx.Status
                        where status.Type == StatusType.Mentions
                        select status;

                var statusList = statuses.ToList();
            }
            catch (TwitterQueryException tqe)
            {
                // log it to the console
                Console.WriteLine(
                    "\nHTTP Error Code: {0}\nError: {1}\nRequest: {2}\n",
                    tqe.HttpError,
                    tqe.Response.Error,
                    tqe.Response.Request);
            }
        }
    }
}
