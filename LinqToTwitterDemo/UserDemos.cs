using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows user demos
    /// </summary>
    public class UserDemos
    {
        /// <summary>
        /// Run all user related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            //UserShowWithIDQueryDemo(twitterCtx);
            //UserShowWithScreenNameQueryDemo(twitterCtx);
            UserShowForAuthenticatedUser(twitterCtx);
            //UserFriendsQueryDemo(twitterCtx);
            //UserFriendsWithCursorQueryDemo(twitterCtx);
            //UsersLookupDemo(twitterCtx);
            //UserSearchDemo(twitterCtx);        
            //UserFollowersQueryDemo(twitterCtx);
            //UserFollowersWithCursorsQueryDemo(twitterCtx);
            //GetAllFollowersQueryDemo(twitterCtx);
            //UserSuggestedCategoriesListQueryDemo(twitterCtx);
            //UserSuggestedCategoriesListWithLangQueryDemo(twitterCtx);
            //UsersInSuggestedCategoryQueryDemo(twitterCtx);
            //UserShowLoggedInUserQueryDemo(twitterCtx);
            //VerifiedAndGeoEnabledDemo(twitterCtx);
            //CategoryStatusDemo(twitterCtx);
        }

        private static void CategoryStatusDemo(TwitterContext twitterCtx)
        {
            var catTweets =
                (from user in twitterCtx.User
                 where user.Type == UserType.CategoryStatus &&
                       user.Slug == "Technology"
                 select user)
                .ToList();

            Console.WriteLine("Tweets from Suggested Users in Technology Category: \n");

            catTweets.ForEach(tweet => 
                Console.WriteLine(
                    "User: {0}\nTweet: {1}\n", 
                    tweet.Name, 
                    tweet.Status == null ?
                        "<Tweet not available.>" : 
                        tweet.Status.Text));
        }

        /// <summary>
        /// Shows how to search for a user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserSearchDemo(TwitterContext twitterCtx)
        {
            var user =
                (from usr in twitterCtx.User
                 where usr.Type == UserType.Search &&
                       usr.Query == "Joe Mayo"
                 select usr)
                 .FirstOrDefault();

            Console.WriteLine("User Name: " + user.Name);
        }

        /// <summary>
        /// Shows how to perform a lookup of specified user details
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void UsersLookupDemo(TwitterContext twitterCtx)
        {
            var users =
                (from user in twitterCtx.User
                 where user.Type == UserType.Lookup &&
                       user.ScreenName == "JoeMayo,LinqToTweeter"
                 select user)
                 .ToList();

            users.ForEach(user => Console.WriteLine("Name: " + user.Name));
        }

        /// <summary>
        /// shows how to query for users in a suggested category
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UsersInSuggestedCategoryQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                (from tweet in twitterCtx.User
                 where tweet.Type == UserType.Category &&
                       tweet.Slug == "funny"
                 select tweet)
                 .ToList();

            users.ForEach(
                user => Console.WriteLine("User: " + user.Name));
        }

        /// <summary>
        /// shows how to query suggested categories
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserSuggestedCategoriesListQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Categories
                select tweet;

            var user = users.SingleOrDefault();

            user.Categories.ForEach(
                cat => Console.WriteLine("Category: " + cat.Name));
        }

        /// <summary>
        /// shows how to query suggested categories
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserSuggestedCategoriesListWithLangQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Categories &&
                      tweet.Lang == "it"
                select tweet;

            var user = users.SingleOrDefault();

            user.Categories.ForEach(
                cat => Console.WriteLine("Category: " + cat.Name));
        }

        /// <summary>
        /// shows how to query authenticated user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserShowForAuthenticatedUser(TwitterContext twitterCtx)
        {
            string screenName = twitterCtx.AuthorizedClient.ScreenName;

            var user =
                (from usr in twitterCtx.User
                 where usr.Type == UserType.Show &&
                       usr.ScreenName == screenName
                 select usr)
                .SingleOrDefault();

            var name = user.Name;
            var lastStatus = user.Status == null ? "No Status" : user.Status.Text;

            Console.WriteLine("\nName: {0}, Last Tweet: {1}\n", name, lastStatus);
        }

        /// <summary>
        /// shows how to query users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserShowWithScreenNameQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Show &&
                      tweet.ScreenName == "JoeMayo"
                select tweet;

            var user = users.SingleOrDefault();

            var name = user.Name;
            var lastStatus = user.Status == null ? "No Status" : user.Status.Text;

            Console.WriteLine();
            Console.WriteLine("Name: {0}, Last Tweet: {1}\n", name, lastStatus);
        }

        /// <summary>
        /// shows how to query users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserShowLoggedInUserQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from usr in twitterCtx.User
                where usr.Type == UserType.Show &&
                      usr.ScreenName == twitterCtx.UserName
                select usr;

            var user = users.SingleOrDefault();

            var name = user.Name;
            var lastStatus = user.Status == null ? "No Status" : user.Status.Text;

            Console.WriteLine();
            Console.WriteLine("Name: {0}, Last Tweet: {1}\n", name, lastStatus);
        }

        /// <summary>
        /// Uses LINQ to Twitter to discover password to 1st Chirp conference
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserShowWithIDQueryDemo(TwitterContext twitterCtx)
        {
            var user =
                (from tweet in twitterCtx.User
                 where tweet.Type == UserType.Show &&
                      tweet.ID == "6253282"
                 select tweet)
                 .SingleOrDefault();

            Console.WriteLine(
                "The password to Chirp is: {0}",
                user.Identifier.ScreenName);
        }

        /// <summary>
        /// shows how to query friends of a specified user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserFriendsQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Friends &&
                      tweet.ID == "JoeMayo" // <-- user to get friends for
                select tweet;

            foreach (var user in users)
            {
                var status =
                    user.Protected || user.Status == null ?
                        "Status Unavailable" :
                        user.Status.Text;

                Console.WriteLine(
                    "ID: {0}, Name: {1}\nLast Tweet: {2}\n",
                    user.Identifier.ID, user.Identifier.ScreenName, status);
            }
        }

        /// <summary>
        /// shows how to check the verified and geoenabled tags for users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void VerifiedAndGeoEnabledDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Friends &&
                      tweet.ID == "15411837" // <-- user to get friends for
                select tweet;

            foreach (var user in users)
            {
                var status =
                    user.Protected || user.Status == null ?
                        "Status Unavailable" :
                        user.Status.Text;

                Console.WriteLine(
                        "ID: {0}, Verified: {1}, GeoEnabled: {2}, Name: {3}\nLast Tweet: {4}\n",
                        user.ID, user.Verified, user.GeoEnabled, user.Name, status);
            }
        }

        /// <summary>
        /// shows how to query friends of a specified user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserFriendsWithCursorQueryDemo(TwitterContext twitterCtx)
        {
            int pageNumber = 1;
            string nextCursor = "-1";
            while (!string.IsNullOrEmpty(nextCursor) && nextCursor != "0")
            {
                var users =
                     (from user in twitterCtx.User
                      where user.Type == UserType.Friends &&
                            user.ID == "15411837" &&
                            user.Cursor == nextCursor
                      select user)
                      .ToList();

                Console.WriteLine(
                    "Page #" + pageNumber + " has " + users.Count + " users.");

                nextCursor = users[0].CursorMovement.Next;
                pageNumber++;
            }
        }

        /// <summary>
        /// shows how to query users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserFollowersQueryDemo(TwitterContext twitterCtx)
        {
            var users =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Followers &&
                      tweet.ID == "15411837"
                select tweet;

            foreach (var user in users)
            {
                var status =
                    user.Protected || user.Status == null ?
                        "Status Unavailable" :
                        user.Status.Text;

                Console.WriteLine(
                        "Name: {0}, Last Tweet: {1}\n",
                        user.Name, status);
            }
        }

        /// <summary>
        /// shows how to query users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserFollowersWithCursorsQueryDemo(TwitterContext twitterCtx)
        {
            int pageNumber = 1;
            string nextCursor = "-1";
            while (!string.IsNullOrEmpty(nextCursor) && nextCursor != "0")
            {
                var users =
                    (from user in twitterCtx.User
                     where user.Type == UserType.Followers &&
                           user.ID == "15411837" &&
                           user.Cursor == nextCursor
                     select user)
                     .ToList();

                Console.WriteLine(
                    "Page #" + pageNumber + " has " + users.Count + " users.");

                nextCursor = users[0].CursorMovement.Next;
                pageNumber++;
            }
        }

        /// <summary>
        /// shows how to query all followers
        /// </summary>
        /// <remarks>
        /// uses the Page property because Twitter doesn't
        /// return all followers in a single call; you
        /// must page through results until you get all
        /// </remarks>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetAllFollowersQueryDemo(TwitterContext twitterCtx)
        {
            //
            // Paging has been deprecated for Friends and Followers
            // Please use cursors instead
            //

            var followerList = new List<User>();

            List<User> followers = new List<User>();
            int pageNumber = 1;

            do
            {
                followers.Clear();

                followers =
                    (from follower in twitterCtx.User
                     where follower.Type == UserType.Followers &&
                           follower.ScreenName == "JoeMayo" &&
                           follower.Page == pageNumber
                     select follower)
                     .ToList();

                pageNumber++;
                followerList.AddRange(followers);
            }
            while (followers.Count > 0);

            Console.WriteLine("\nFollowers: \n");

            foreach (var user in followerList)
            {
                var status =
                    user.Protected || user.Status == null ?
                        "Status Unavailable" :
                        user.Status.Text;

                Console.WriteLine(
                        "Name: {0}, Last Tweet: {1}\n",
                        user.Name, status);
            }

            Console.WriteLine("\nFollower Count: {0}\n", followerList.Count);
        }
    }
}
