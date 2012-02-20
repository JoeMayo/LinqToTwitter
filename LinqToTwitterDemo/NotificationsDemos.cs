using System;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows notifications demos
    /// </summary>
    public class NotificationsDemos
    {
        /// <summary>
        /// Run all notifications related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            //EnableNotificationsDemo(twitterCtx);
            //EnableNotificationsWithScreenNameDemo(twitterCtx);
            //EnableNotificationsWithUserIDDemo(twitterCtx);
            //DisableNotificationsDemo(twitterCtx);
        }

        /// <summary>
        /// Shows how to do a Notifications Follow
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void EnableNotificationsDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.EnableNotifications("15411837", null, null);

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// Shows how to do a Notifications Follow
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void EnableNotificationsWithScreenNameDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.EnableNotifications(null, null, "JoeMayo");

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// Shows how to do a Notifications Follow
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void EnableNotificationsWithUserIDDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.EnableNotifications(null, "15411837", null);

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// Shows how to do a Notifications Leave
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DisableNotificationsDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.DisableNotifications("15411837", null, null);

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }
    }
}
