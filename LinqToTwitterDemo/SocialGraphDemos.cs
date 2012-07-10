using System;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows social graph demos
    /// </summary>
    public class SocialGraphDemos
    {
        /// <summary>
        /// Run all social graph related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            ShowFriendsDemo(twitterCtx);
            //ShowFriendsWithCursorDemo(twitterCtx);
            //ShowFollowersDemo(twitterCtx);
            //ShowFollowersWithCursorDemo(twitterCtx);
        }

        /// <summary>
        /// Shows how to list followers
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ShowFollowersDemo(TwitterContext twitterCtx)
        {
            var followers =
                (from follower in twitterCtx.SocialGraph
                 where follower.Type == SocialGraphType.Followers &&
                       follower.UserID == 15411837ul
                 select follower)
                 .SingleOrDefault();

            followers.IDs.ForEach(id => Console.WriteLine("Follower ID: " + id));
        }

        /// <summary>
        /// Pages through a list of followers using cursors
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ShowFollowersWithCursorDemo(TwitterContext twitterCtx)
        {
            int pageNumber = 1;

            // "-1" means to begin on the first page
            string nextCursor = "-1";

            // cursor will be "0" when no more pages
            // notice that I'm checking for null/empty - don't trust data
            while (!string.IsNullOrEmpty(nextCursor) && nextCursor != "0")
            {
                var followers =
                    (from follower in twitterCtx.SocialGraph
                     where follower.Type == SocialGraphType.Followers &&
                           follower.UserID == 15411837ul &&
                           follower.Cursor == nextCursor // <-- set this to use cursors
                     select follower)
                     .FirstOrDefault();

                Console.WriteLine(
                    "Page #" + pageNumber + " has " + followers.IDs.Count + " IDs.");

                // use the cursor for the next page
                // this is not a page number, but a marker (cursor)
                // to tell Twitter which page to return
                nextCursor = followers.CursorMovement.Next;
                pageNumber++;
            }
        }

        /// <summary>
        /// Shows how to list Friends
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ShowFriendsDemo(TwitterContext twitterCtx)
        {
            var friendList =
                (from friend in twitterCtx.SocialGraph
                 where friend.Type == SocialGraphType.Friends &&
                       friend.ScreenName == "JoeMayo"
                 select friend)
                 .SingleOrDefault();

            foreach (var id in friendList.IDs)
            {
                Console.WriteLine("Friend ID: " + id);
            }
        }

        /// <summary>
        /// Pages through a list of followers using cursors
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ShowFriendsWithCursorDemo(TwitterContext twitterCtx)
        {
            int pageNumber = 1;
            string nextCursor = "-1";
            while (!string.IsNullOrEmpty(nextCursor) && nextCursor != "0")
            {
                var friends =
                    (from friend in twitterCtx.SocialGraph
                     where friend.Type == SocialGraphType.Friends &&
                           friend.ScreenName == "JoeMayo" &&
                           friend.Cursor == nextCursor
                     select friend)
                     .SingleOrDefault();

                Console.WriteLine(
                    "Page #" + pageNumber + " has " + friends.IDs.Count + " IDs.");

                nextCursor = friends.CursorMovement.Next;
                pageNumber++;
            }
        }
    }
}
