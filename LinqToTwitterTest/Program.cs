using System;
using System.Linq;

using LinqToTwitter;

namespace LinqToTwitterDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //
            // get user credentials and instantiate TwitterContext
            //

            Console.Write("Twitter User Name: ");
            string userName = Console.ReadLine();

            Console.Write("Twitter Password: ");
            string password = Console.ReadLine();

            // similar to DataContext (LINQ to SQL) or ObjectContext (LINQ to Entities)
            var twitterCtx = new TwitterContext(userName, password, "http://www.twitter.com/", "http://search.twitter.com/");

            //
            // status tweets
            //

            //UpdateStatusDemo(twitterCtx);
            //DestroyStatusDemo(twitterCtx);
            //UserStatusQueryDemo(twitterCtx);
            //PublicStatusQueryDemo();

            //
            // user tweets
            //

            //UserQueryDemo(twitterCtx);

            //
            // direct messages
            //

            //DirectMessageQueryDemo(twitterCtx);

            //NewDirectMessageDemo(twitterCtx);

            //DestroyDirectMessageDemo(twitterCtx);

            //
            // freindship
            //

            //FriendshipExistsDemo(twitterCtx);

            //CreateFriendshipFollowDemo(twitterCtx);
            //DestroyFriendshipDemo(twitterCtx);
            //CreateFriendshipNoDeviceUpdatesDemo(twitterCtx);

            //
            // SocialGraph
            //

            //ShowFriendsDemo(twitterCtx);
            //ShowFollowersDemo(twitterCtx);

            //
            // Search
            //

            //SearchTwitterDemo(twitterCtx);

            //
            // Favorites
            //

            //FavoritesQueryDemo(twitterCtx);
            //CreateFavoriteDemo(twitterCtx);
            //DestroyFavoriteDemo(twitterCtx);

            //
            // Notifications
            //

            //EnableNotificationsDemo(twitterCtx);
            //DisableNotificationsDemo(twitterCtx);

            Console.ReadKey();
        }


        private static void EnableNotificationsDemo(TwitterContext twitterCtx)
        {
            var userList = twitterCtx.EnableNotifications("15411837", null, null);

            var user = userList.FirstOrDefault();

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        private static void DisableNotificationsDemo(TwitterContext twitterCtx)
        {
            var userList = twitterCtx.DisableNotifications("15411837", null, null);

            var user = userList.FirstOrDefault();

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        #region Favorites Demos

        private static void DestroyFavoriteDemo(TwitterContext twitterCtx)
        {
            var statusList = twitterCtx.DestroyFavorite("1552797863");

            var status = statusList.First();

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        /// <summary>
        /// Shows how to create a Favorite
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void CreateFavoriteDemo(TwitterContext twitterCtx)
        {
            var statusList = twitterCtx.CreateFavorite("1552797863");

            var status = statusList.First();

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        /// <summary>
        /// shows how to request a favorites list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void FavoritesQueryDemo(TwitterContext twitterCtx)
        {
            var favorites =
                from fav in twitterCtx.Favorites
                where fav.Type == FavoritesType.Favorites
                select fav;

            favorites.ToList().ForEach(
                fav => Console.WriteLine(
                    "User Name: {0}, Tweet: {1}",
                    fav.User.Name, fav.Text));
        }

        #endregion

        #region Search Demos

        /// <summary>
        /// shows how to perform a twitter search
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SearchTwitterDemo(TwitterContext twitterCtx)
        {
            var queryResults =
                from search in twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "LINQ to Twitter"
                select search;

            foreach (var search in queryResults)
            {
                // here, you can see that properties are named
                // from the perspective of atom feed elements
                // i.e. the query string is called Title
                Console.WriteLine("\nQuery:\n" + search.Title);

                foreach (var entry in search.Entries)
                {
                    Console.WriteLine(
                        "ID: {0}, Content: {1}\n",
                        entry.ID, entry.Content);
                } 
            }
        }

        #endregion

        #region Followers Demos

        /// <summary>
        /// Shows how to list followers
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ShowFollowersDemo(TwitterContext twitterCtx)
        {
            var followers =
                from follower in twitterCtx.SocialGraph
                where follower.Type == SocialGraphType.Followers &&
                      follower.ID == 15411837
                select follower;

            followers.ToList().ForEach(
                follower => Console.WriteLine("Follower ID: " + follower.ID));
        }

        /// <summary>
        /// Shows how to list Friends
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ShowFriendsDemo(TwitterContext twitterCtx)
        {
            var friends =
                from friend in twitterCtx.SocialGraph
                where friend.Type == SocialGraphType.Friends
                select friend;

            friends.ToList().ForEach(
                friend => Console.WriteLine("Friend ID: " + friend.ID));
        }

        #endregion

        #region Friendship Demos

        private static void CreateFriendshipNoDeviceUpdatesDemo(TwitterContext twitterCtx)
        {
            var results = twitterCtx.CreateFriendship("LinqToTweeter", false);

            var user = results.First();

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        private static void DestroyFriendshipDemo(TwitterContext twitterCtx)
        {
            var results = twitterCtx.DestroyFriendship("LinqToTweeter");

            var user = results.First();

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        private static void CreateFriendshipFollowDemo(TwitterContext twitterCtx)
        {
            var results = twitterCtx.CreateFriendship("LinqToTweeter", true);

            var user = results.First();

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        /// <summary>
        /// shows how to show that one user follows another with Friendship Exists
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void FriendshipExistsDemo(TwitterContext twitterCtx)
        {
            var friendship =
                from friend in twitterCtx.Friendship
                where friend.Type == FriendshipType.Exists &&
                      friend.SubjectUser == "LinqToTweeter" &&
                      friend.FollowingUser == "JoeMayo"
                select friend;

            Console.WriteLine(
                "JoeMayo follows LinqToTweeter: " + 
                friendship.ToList().First().IsFriend);
        }

        #endregion

        #region Direct Message Demos

        /// <summary>
        /// shows how to delete a direct message
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DestroyDirectMessageDemo(TwitterContext twitterCtx)
        {
            var results = twitterCtx.DestroyDirectMessage("96404341");

            var result = results.FirstOrDefault();

            if (result != null)
            {
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}",
                    result.RecipientScreenName,
                    result.Text);
            }
        }

        /// <summary>
        /// shows how to send a new direct message
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void NewDirectMessageDemo(TwitterContext twitterCtx)
        {
            var results = twitterCtx.NewDirectMessage("16761255", "Direct Message Test - 4/16/09");

            var result = results.FirstOrDefault();

            if (result != null)
            {
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}",
                    result.RecipientScreenName,
                    result.Text);
            }
        }

        /// <summary>
        /// shows how to query direct messages
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DirectMessageQueryDemo(TwitterContext twitterCtx)
        {
            var directMessages =
                from tweet in twitterCtx.DirectMessage
                where tweet.Type == DirectMessageType.SentTo
                select tweet;

            directMessages.ToList().ForEach(
                dm => Console.WriteLine(
                    "Sender: {0}, Tweet: {1}",
                    dm.SenderScreenName,
                    dm.Text));
        }

        #endregion

        #region User Demos

        /// <summary>
        /// shows how to query users
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserQueryDemo(TwitterContext twitterCtx)
        {
            var userTweets =
                from tweet in twitterCtx.User
                where tweet.Type == UserType.Show &&
                      tweet.ID == "15411837"
                select tweet;

            var tweetList = userTweets.ToList();
        }

        #endregion

        #region Status Demos

        /// <summary>
        /// shows how to query status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UserStatusQueryDemo(TwitterContext twitterCtx)
        {
            Console.WriteLine();

            var statusTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.User
                    //&& tweet.ID == "945932078" // ID for Show
                      && tweet.ID == "15411837"  // ID for User
                      && tweet.Page == 1
                      && tweet.Count == 20
                      && tweet.SinceID == 931894254
                      && tweet.Since == DateTime.Now.AddMonths(-1)
                select tweet;

            //statusTweets = statusTweets.Where(tweet => tweet.Since == DateTime.Now.AddMonths(-1)); 

            foreach (var tweet in statusTweets)
            {
                Console.WriteLine(
                    "(" + tweet.ID + ")" +
                    "[" + tweet.User.ID + "]" +
                    tweet.User.Name + ", " +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            }
        }

        /// <summary>
        /// shows how to delete a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DestroyStatusDemo(TwitterContext twitterCtx)
        {
            var statusResult = twitterCtx.DestroyStatus("1539399086");

            foreach (var tweet in statusResult)
            {
                Console.WriteLine(
                    "(" + tweet.ID + ")" +
                    "[" + tweet.User.ID + "]" +
                    tweet.User.Name + ", " +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            }
        }

        /// <summary>
        /// shows how to update a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusDemo(TwitterContext twitterCtx)
        {
            //var statusResult = twitterCtx.UpdateStatus("@TwitterUser Testing LINQ to Twitter with reply", "961760788");

            //foreach (var tweet in statusResult)
            //{
            //    Console.WriteLine(
            //        "(" + tweet.ID + ")" +
            //        "[" + tweet.User.ID + "]" +
            //        tweet.User.Name + ", " +
            //        tweet.Text + ", " +
            //        tweet.CreatedAt);
            //}

            var statusResult = twitterCtx.UpdateStatus("Testing LINQ to Twitter with only status - 4/16/09");

            foreach (var tweet in statusResult)
            {
                Console.WriteLine(
                    "(" + tweet.ID + ")" +
                    "[" + tweet.User.ID + "]" +
                    tweet.User.Name + ", " +
                    tweet.Text + ", " +
                    tweet.CreatedAt);
            }
        }

        /// <summary>
        /// shows how to send a public status query
        /// </summary>
        private static void PublicStatusQueryDemo()
        {
            var twitterCtx = new TwitterContext();

            var publicTweets =
                from tweet in twitterCtx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            publicTweets.ToList().ForEach(
                tweet => Console.WriteLine(
                    "User Name: {0}, Tweet: {1}",
                    tweet.User.Name,
                    tweet.Text));
        }

        #endregion
    }
}
