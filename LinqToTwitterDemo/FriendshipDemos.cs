using LinqToTwitter;
using System;
using System.Linq;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows friendship demos
    /// </summary>
    public class FriendshipDemos
    {
        /// <summary>
        /// Run all friendship related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            CreateFriendshipFollowDemo(twitterCtx);
            //DestroyFriendshipDemo(twitterCtx);
            //CreateFriendshipNoDeviceUpdatesDemo(twitterCtx);
            //CreateFriendshipAsyncDemo(twitterCtx);
            //FriendshipShowDemo(twitterCtx);
            //FriendshipNoRetweetIDsDemo(twitterCtx);
            //FriendshipIncomingDemo(twitterCtx);
            //FriendshipOutgoingDemo(twitterCtx);
            //FriendshipScreenNameLookupDemo(twitterCtx);
            //FriendshipUserIDLookupDemo(twitterCtx);
            //UpdateSettingsDemo(twitterCtx);
            //FriendsListDemo(twitterCtx);
            //FollowersListDemo(twitterCtx);
        }

        private static void DestroyFriendshipDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.DestroyFriendship(null, "Linq2Tweeter");

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        private static void CreateFriendshipFollowDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.CreateFriendship(null, "JoeMayo", true);

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        private static void CreateFriendshipNoDeviceUpdatesDemo(TwitterContext twitterCtx)
        {
            var user = twitterCtx.CreateFriendship(null, "JoeMayo", false);

            Console.WriteLine(
                "User Name: {0}, Status: {1}",
                user.Name,
                user.Status.Text);
        }

        private static void CreateFriendshipAsyncDemo(TwitterContext twitterCtx)
        {
            twitterCtx.CreateFriendship(null, "JoeMayo", false,
                response =>
                {
                    User usr = response.State;

                    Console.WriteLine(
                        "User Name: {0}, Status: {1}",
                        usr.Name,
                        usr.Status.Text);
                });
        }

        /// <summary>
        /// shows how to show that one user follows another with Friendship Exists
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void FriendshipShowDemo(TwitterContext twitterCtx)
        {
            var friendship =
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.Show &&
                       friend.SourceScreenName == "Linq2Tweeter" &&
                       friend.TargetScreenName == "JoeMayo"
                 select friend)
                 .First();

            Console.WriteLine(
                "\nJoeMayo follows LinqToTweeter: " + 
                friendship.SourceRelationship.FollowedBy + 
                "\nLinqToTweeter follows JoeMayo: " +
                friendship.TargetRelationship.FollowedBy);
        }

        /// <summary>
        /// shows how to get ids of user that the logged in user doesn't want retweets for
        /// </summary>
        /// <param name="twitterCtx"></param>
        private static void FriendshipNoRetweetIDsDemo(TwitterContext twitterCtx)
        {
            var friendship =
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.NoRetweetIDs
                 select friend)
                .First();

            var ids =
                (from id in friendship.IDInfo.IDs
                 select id.ToString())
                .ToArray();

            Console.WriteLine("\nIDs: " + string.Join(",", ids));
        }

        /// <summary>
        /// Shows how to check who has an incoming request to logged in user's locked account
        /// </summary>
        /// <param name="twitterCtx">twitterCtx</param>
        private static void FriendshipIncomingDemo(TwitterContext twitterCtx)
        {
            var request =
                (from req in twitterCtx.Friendship
                 where req.Type == FriendshipType.Incoming
                 select req)
                 .FirstOrDefault();

            request.IDInfo.IDs.ForEach(req => Console.WriteLine(req));
        }

        /// <summary>
        /// Shows all outgoing requests from the logged in user to locked accounts
        /// </summary>
        /// <param name="twitterCtx">twitterCtx</param>
        private static void FriendshipOutgoingDemo(TwitterContext twitterCtx)
        {
            var request =
                (from req in twitterCtx.Friendship
                 where req.Type == FriendshipType.Outgoing
                 select req)
                 .FirstOrDefault();

            request.IDInfo.IDs.ForEach(req => Console.WriteLine(req));
        }

        private static void FriendshipScreenNameLookupDemo(TwitterContext twitterCtx)
        {
            var relationships =
                (from look in twitterCtx.Friendship
                 where look.Type == FriendshipType.Lookup &&
                       look.ScreenName == "joemayo,linq2tweeter"
                 select look.Relationships)
                .SingleOrDefault();

            relationships.ForEach(rel => Console.WriteLine(
                "Relationship to " + rel.ScreenName + 
                " is Following: " + rel.Following + 
                " Followed By: " + rel.FollowedBy));
        }

        private static void FriendshipUserIDLookupDemo(TwitterContext twitterCtx)
        {
            var relationships =
                (from look in twitterCtx.Friendship
                 where look.Type == FriendshipType.Lookup &&
                       look.UserID == "15411837,16761255"
                 select look.Relationships)
                .SingleOrDefault();

            relationships.ForEach(rel => Console.WriteLine(
                "Relationship to " + rel.ScreenName + " is Following: " + rel.Following + " Followed By: " + rel.FollowedBy));
        }

        private static void UpdateSettingsDemo(TwitterContext twitterCtx)
        {
            Friendship friend = twitterCtx.UpdateFriendshipSettings("Linq2Tweeter", true, true);

            Console.WriteLine("Settings for {0} are: Can Retweet is {1} and Can Send Device Notifications is {2}",
                friend.SourceRelationship.ScreenName, 
                friend.SourceRelationship.RetweetsWanted, 
                friend.SourceRelationship.NotificationsEnabled);
        }

        private static void FriendsListDemo(TwitterContext twitterCtx)
        {
            Friendship friendship;
            string cursor = "-1";
            do
            {
                friendship =
                    (from friend in twitterCtx.Friendship
                     where friend.Type == FriendshipType.FriendsList &&
                           friend.ScreenName == "JoeMayo" &&
                           friend.Cursor == cursor                         
                     select friend)
                    .SingleOrDefault();

                cursor = friendship.CursorMovement.Next;       

                friendship.Users.ForEach(friend =>
                    Console.WriteLine(
                        "ID: {0} Name: {1}",
                        friend.Identifier.UserID, friend.Identifier.ScreenName)); 

            } while (cursor != "0");
        }

        private static void FollowersListDemo(TwitterContext twitterCtx)
        {
            var friendship =
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.FollowersList &&
                       friend.ScreenName == "JoeMayo"
                 select friend)
                .SingleOrDefault();

            friendship.Users.ForEach(friend =>
                Console.WriteLine(
                    "ID: {0} Name: {1}",
                    friend.Identifier.UserID, friend.Identifier.ScreenName));
        }
    }
}
