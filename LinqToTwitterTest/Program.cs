using System;
using System.Linq;

using LinqToTwitter;
using System.Net;

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

            //
            // Blocks
            //

            //CreateBlock(twitterCtx);
            //DestroyBlock(twitterCtx);

            //
            // Help
            //

            //PerformHelpTest(twitterCtx);

            //
            // Account
            //

            //VerifyAccountCredentials(twitterCtx);
            //ViewRateLimitStatus(twitterCtx);
            //EndSession(twitterCtx);
            //UpdateDeliveryDevice(twitterCtx);
            //UpdateAccountColors(twitterCtx);
            //UpdateAccountImage(twitterCtx);
            //UpdateAccountBackgroundImage(twitterCtx);
            UpdateAccountInfoDemo(twitterCtx);

            Console.ReadKey();
        }

        #region Account Demos

        /// <summary>
        /// Shows how to update account profile info
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountInfoDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountProfile(
                "LtoT Test", 
                "WhoDoIWantToAggravateToday@World.com", 
                "http://linqtotwitter.codeplex.com", 
                "Anywhere In The World", 
                "Testing the LINQ to Twitter Account Profile Update.");

            Console.WriteLine(
                "Name: {0}\nEmail: {1}\nURL: {2}\nLocation: {3}\nDescription: {4}",
                user.Name, user.Email, user.URL, user.Location, user.Description);
        }

        /// <summary>
        /// Shows how to update the background image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountBackgroundImage(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountBackgroundImage(@"C:\Users\jmayo\Pictures\41NN9GPz15L__SS500_.jpg", false);

            Console.WriteLine("User Image: " + user.ProfileBackgroundImageUrl);
        }

        /// <summary>
        /// Shows how to update the image in an account
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateAccountImage(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountImage(@"C:\Users\jmayo\Pictures\JoeTwitter.jpg");

            Console.WriteLine("User Image: " + user.ProfileImageUrl);
        }

        /// <summary>
        /// Shows how to update Twitter colors
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void UpdateAccountColors(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountColors("9ae4e8", "#000000", "#0000ff", "#e0ff92", "#87bc44");

            Console.WriteLine("\nAccount Colors:\n");

            Console.WriteLine("Background:     " + user.ProfileBackgroundColor);
            Console.WriteLine("Text:           " + user.ProfileTextColor);
            Console.WriteLine("Link:           " + user.ProfileLinkColor);
            Console.WriteLine("Sidebar Fill:   " + user.ProfileSidebarFillColor);
            Console.WriteLine("Sidebar Border: " + user.ProfileSidebarBorderColor);
        }

        /// <summary>
        /// Shows how to update a device
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void UpdateDeliveryDevice(TwitterContext twitterCtx)
        {
            var user = twitterCtx.UpdateAccountDeliveryDevice(DeviceType.None);

            Console.WriteLine("Device Type: {0}", user.Notifications.ToString());
        }

        /// <summary>
        /// Shows how to end the session for the current account
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void EndSession(TwitterContext twitterCtx)
        {
            var endSessionStatus = twitterCtx.EndAccountSession();

            Console.WriteLine(
                "Request: {0}, Error: {1}", 
                endSessionStatus.Request, 
                endSessionStatus.Error);
        }

        /// <summary>
        /// Shows how to query an account's rate limit status info
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ViewRateLimitStatus(TwitterContext twitterCtx)
        {
            var accounts =
                from acct in twitterCtx.Account
                where acct.Type == AccountType.RateLimitStatus
                select acct;

            foreach (var account in accounts)
            {
                Console.WriteLine("\nRate Limit Status: \n");
                Console.WriteLine("Remaining Hits: {0}", account.RateLimitStatus.RemainingHits);
                Console.WriteLine("Hourly Limit: {0}", account.RateLimitStatus.HourlyLimit);
                Console.WriteLine("Reset Time: {0}", account.RateLimitStatus.ResetTime);
                Console.WriteLine("Reset Time in Seconds: {0}", account.RateLimitStatus.ResetTimeInSeconds);
            }
        }

        /// <summary>
        /// verifies that account credentials are correct
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void VerifyAccountCredentials(TwitterContext twitterCtx)
        {
            var accounts =
                from acct in twitterCtx.Account
                where acct.Type == AccountType.VerifyCredentials
                select acct;

            try
            {
                foreach (var account in accounts)
                {
                    Console.WriteLine("Credentials for account, {0}, are okay.", account.User.Name);
                }
            }
            catch (WebException wex)
            {
                Console.WriteLine("Twitter did not recognize the credentials. Response from Twitter: " + wex.Message);
            }
        }

        #endregion

        #region Help Demos

        /// <summary>
        /// shows how to perform a help test
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void PerformHelpTest(TwitterContext twitterCtx)
        {
            var helpResult = twitterCtx.HelpTest();

            Console.WriteLine("Test Result: " + helpResult);
        }

        #endregion

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

        #endregion

        #region Notifications Demos

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
        /// Shows how to do a Notifications Leave
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DisableNotificationsDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.DisableNotifications("15411837", null, null);

            if (user == null) return;

            Console.WriteLine("User Name: " + user.Name);
        }

        #endregion

        #region Favorites Demos

        private static void DestroyFavoriteDemo(TwitterContext twitterCtx)
        {
            var status = twitterCtx.DestroyFavorite("1552797863");

            Console.WriteLine("User: {0}, Tweet: {1}", status.User.Name, status.Text);
        }

        /// <summary>
        /// Shows how to create a Favorite
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void CreateFavoriteDemo(TwitterContext twitterCtx)
        {
            var status = twitterCtx.CreateFavorite("1552797863");

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
            var user = twitterCtx.CreateFriendship("LinqToTweeter", false);

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        private static void DestroyFriendshipDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.DestroyFriendship("LinqToTweeter");

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        private static void CreateFriendshipFollowDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.CreateFriendship("LinqToTweeter", true);

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
            var message = twitterCtx.DestroyDirectMessage("96404341");

            if (message != null)
            {
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}",
                    message.RecipientScreenName,
                    message.Text);
            }
        }

        /// <summary>
        /// shows how to send a new direct message
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void NewDirectMessageDemo(TwitterContext twitterCtx)
        {
            var message = twitterCtx.NewDirectMessage("16761255", "Direct Message Test - 4/16/09");

            if (message != null)
            {
                Console.WriteLine(
                    "Recipient: {0}, Message: {1}",
                    message.RecipientScreenName,
                    message.Text);
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
            var status = twitterCtx.DestroyStatus("1539399086");

            Console.WriteLine(
                "(" + status.ID + ")" +
                "[" + status.User.ID + "]" +
                status.User.Name + ", " +
                status.Text + ", " +
                status.CreatedAt);
        }

        /// <summary>
        /// shows how to update a status
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateStatusDemo(TwitterContext twitterCtx)
        {
            // TODO: separate into multiple methods - Joe

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

            var tweet = twitterCtx.UpdateStatus("Testing LINQ to Twitter with only status - 4/16/09");

            Console.WriteLine(
                "(" + tweet.ID + ")" +
                "[" + tweet.User.ID + "]" +
                tweet.User.Name + ", " +
                tweet.Text + ", " +
                tweet.CreatedAt);
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
