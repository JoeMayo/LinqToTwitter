using System;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows blocks demos
    /// </summary>
    public class BlocksDemos
    {
        /// <summary>
        /// Run all blocks related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            //CreateBlock(twitterCtx);
            //DestroyBlock(twitterCtx);
            //BlockExistsDemo(twitterCtx);
            //BlockIDsDemo(twitterCtx);
            //BlockBlockingDemo(twitterCtx);
            BlockBlockingProjectionDemo(twitterCtx);
        }

        #region Block Demos

        /// <summary>
        /// shows how to unblock a user
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void DestroyBlock(TwitterContext twitterCtx)
        {
            var user = twitterCtx.DestroyBlock("JoeMayo");

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// Shows how to block a user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void CreateBlock(TwitterContext twitterCtx)
        {
            var user = twitterCtx.CreateBlock("JoeMayo");

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// shows how to get a list of users that are being blocked
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void BlockBlockingDemo(TwitterContext twitterCtx)
        {
            var block =
                (from blockItem in twitterCtx.Blocks
                 where blockItem.Type == BlockingType.Blocking
                 select blockItem)
                 .FirstOrDefault();

            block.Users.ForEach(
                user => Console.WriteLine("User, {0} is blocked.", user.Name));
        }


        /// <summary>
        /// shows how to get a list of users that are being blocked via custom projection
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void BlockBlockingProjectionDemo(TwitterContext twitterCtx)
        {
            var blockedUsers =
                (from blockItem in twitterCtx.Blocks
                 where blockItem.Type == BlockingType.Blocking
                 select blockItem.Users)
                .FirstOrDefault();

            blockedUsers.ForEach(
                user => Console.WriteLine("User, {0} is blocked.", user.Name));
        }

        /// <summary>
        /// shows how to get a list of IDs of the users being blocked
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void BlockIDsDemo(TwitterContext twitterCtx)
        {
            var result =
                (from blockItem in twitterCtx.Blocks
                 where blockItem.Type == BlockingType.IDS
                 select blockItem)
                 .SingleOrDefault();

            result.IDs.ForEach(block => Console.WriteLine("ID: {0}", block));
        }

        /// <summary>
        /// shows how to see if a specific user is being blocked
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void BlockExistsDemo(TwitterContext twitterCtx)
        {
            try
            {
                var block =
                    (from blockItem in twitterCtx.Blocks
                     where blockItem.Type == BlockingType.Exists &&
                           blockItem.ScreenName == "JoeMayo"
                     select blockItem)
                     .FirstOrDefault();

                Console.WriteLine("User, {0} is blocked.", block.User.Name);
            }
            catch (TwitterQueryException tqe)
            {
                // Twitter returns HTTP 404 when user is not blocked
                // An HTTP error generates an exception, 
                // which is why User Not Blocked is handled this way
                Console.WriteLine("User not blocked. Twitter Response: " + tqe.Response.Error);
            }
        }

        #endregion
    }
}
