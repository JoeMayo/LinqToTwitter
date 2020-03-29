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
            //BlockListDemo(twitterCtx);
            BlockIDsDemo(twitterCtx);
        }

        /// <summary>
        /// shows how to unblock a user
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void DestroyBlock(TwitterContext twitterCtx)
        {
            Console.Write("User Screen Name to Unblock: ");
            string userName = Console.ReadLine();

            var user = twitterCtx.DestroyBlock(0, userName, true);

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// Shows how to block a user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void CreateBlock(TwitterContext twitterCtx)
        {
            Console.Write("User Screen Name to Block: ");
            string userName = Console.ReadLine();

            var user = twitterCtx.CreateBlock(0, userName, true);

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
                 where blockItem.Type == BlockingType.List
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
                 where blockItem.Type == BlockingType.List
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
                 where blockItem.Type == BlockingType.Ids
                 select blockItem)
                .SingleOrDefault();

            result.IDs.ForEach(block => Console.WriteLine("ID: {0}", block));
        }

        /// <summary>
        /// shows how to see if a specific user is being blocked
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void BlockListDemo(TwitterContext twitterCtx)
        {
            var blocks =
                (from blockItem in twitterCtx.Blocks
                 where blockItem.Type == BlockingType.List
                 select blockItem)
                .FirstOrDefault();

            foreach (var user in blocks.Users)
            {
                Console.WriteLine("User, {0} is blocked.", user.Name);
            }
        }
    }
}
