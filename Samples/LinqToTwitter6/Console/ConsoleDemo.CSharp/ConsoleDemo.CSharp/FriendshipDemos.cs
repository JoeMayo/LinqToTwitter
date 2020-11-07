using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;

namespace Linq2TwitterDemos_Console
{
    class FriendshipDemos
    {
        internal static async Task RunAsync(TwitterContext twitterCtx)
        {
            char key;

            do
            {
                ShowMenu();

                key = Console.ReadKey(true).KeyChar;

                switch (key)
                {
                    case '0':
                        Console.WriteLine("\n\tShowing friends...\n");
                        await ShowFriendsAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tLooking up user ids...\n");
                        await LookupUserIDsAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tGetting incoming...\n");
                        await IncomingFriendshipsAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tGetting Outgoing...\n");
                        await OutgoingFriendshipsAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tShowing no retweet IDs...\n");
                        await NoRetweetIDsAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tGetting friends list...\n");
                        await FriendsListAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\n\tGetting followers list...\n");
                        await FollowersListAsync(twitterCtx);
                        break;
                    case '7':
                        Console.WriteLine("\n\tShowing followers ids...\n");
                        await ShowFollowerIDsAsync(twitterCtx);
                        break;
                    case '8':
                        Console.WriteLine("\n\tShowing friend ids...\n");
                        await ShowFriendIDsAsync(twitterCtx);
                        break;
                    case '9':
                        Console.WriteLine("\n\tCreating friendship...\n");
                        await CreateFriendshipAsync(twitterCtx);
                        break;
                    case 'a':
                    case 'A':
                        Console.WriteLine("\n\tUnfollowing...\n");
                        await DestroyFriendshipAsync(twitterCtx);
                        break;
                    case 'b':
                    case 'B':
                        Console.WriteLine("\n\tUpdating friend settings...\n");
                        await UpdateFreindshipSettingsAsync(twitterCtx);
                        break;
                    case 'q':
                    case 'Q':
                        Console.WriteLine("\nReturning...\n");
                        break;
                    default:
                        Console.WriteLine(key + " is unknown");
                        break;
                }

            } while (char.ToUpper(key) != 'Q');
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nFriendship Demos - Please select:\n");

            Console.WriteLine("\t 0. Show Friends");
            Console.WriteLine("\t 1. Lookup Friendships");
            Console.WriteLine("\t 2. Incoming Friendships");
            Console.WriteLine("\t 3. Outgoing Friendships");
            Console.WriteLine("\t 4. No Retweet IDs");
            Console.WriteLine("\t 5. Friends List");
            Console.WriteLine("\t 6. Followers List");
            Console.WriteLine("\t 7. Follower IDs");
            Console.WriteLine("\t 8. Friend IDs");
            Console.WriteLine("\t 9. Create Friendship");
            Console.WriteLine("\t A. Delete Friendship");
            Console.WriteLine("\t B. Update Friendship Settings");
            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task ShowFriendsAsync(TwitterContext twitterCtx)
        {
            var friendship =
                await
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.Show &&
                       friend.SourceScreenName == "Linq2Twitr" &&
                       friend.TargetScreenName == "JoeMayo"
                 select friend)
                .SingleOrDefaultAsync();

            if (friendship != null && 
                friendship.SourceRelationship != null && 
                friendship.TargetRelationship != null)
            {
                Console.WriteLine(
                        "\nJoeMayo follows LinqToTweeter: " +
                        friendship.SourceRelationship.FollowedBy +
                        "\nLinqToTweeter follows JoeMayo: " +
                        friendship.TargetRelationship.FollowedBy); 
            }
        }

        static async Task LookupUserIDsAsync(TwitterContext twitterCtx)
        {
            var relationships =
                await
                (from look in twitterCtx.Friendship
                 where look.Type == FriendshipType.Lookup &&
                       look.UserID == "15411837,16761255"
                 select look.Relationships)
                .SingleOrDefaultAsync();

            if (relationships != null)
                relationships.ForEach(rel => 
                    Console.WriteLine(
                        "Relationship to " + rel.ScreenName + 
                        ", is Following: " + rel.Following + 
                        ", Followed By: " + rel.FollowedBy));
        }

        static async Task IncomingFriendshipsAsync(TwitterContext twitterCtx)
        {
            var request =
                await
                (from req in twitterCtx.Friendship
                 where req.Type == FriendshipType.Incoming
                 select req)
                .SingleOrDefaultAsync();

            if (request != null && 
                request.IDInfo != null && 
                request.IDInfo.IDs != null)
            {
                request.IDInfo.IDs.ForEach(req => Console.WriteLine(req));
            }
        }

        static async Task OutgoingFriendshipsAsync(TwitterContext twitterCtx)
        {
            var request =
                await
                (from req in twitterCtx.Friendship
                 where req.Type == FriendshipType.Outgoing
                 select req)
                .SingleOrDefaultAsync();

            if (request != null &&
                request.IDInfo != null &&
                request.IDInfo.IDs != null)
            {
                request.IDInfo.IDs.ForEach(req => Console.WriteLine(req));
            }
        }

        static async Task NoRetweetIDsAsync(TwitterContext twitterCtx)
        {
            var friendship =
                await
                (from friend in twitterCtx.Friendship
                 where friend.Type == FriendshipType.NoRetweetIDs
                 select friend)
                .SingleOrDefaultAsync();

            if (friendship != null && 
                friendship.IDInfo != null && 
                friendship.IDInfo.IDs != null)
            {
                var ids =
                    (from id in friendship.IDInfo.IDs
                     select id.ToString())
                    .ToArray();

                Console.WriteLine("\nIDs: " + string.Join(",", ids)); 
            }
        }

        static async Task FriendsListAsync(TwitterContext twitterCtx)
        {
            Friendship friendship;
            long cursor = -1;
            do
            {
                friendship =
                    await
                    (from friend in twitterCtx.Friendship
                     where friend.Type == FriendshipType.FriendsList &&
                           friend.ScreenName == "JoeMayo" &&
                           friend.Cursor == cursor &&
                           friend.Count == 200
                     select friend)
                    .SingleOrDefaultAsync();

                if (friendship != null && 
                    friendship.Users != null && 
                    friendship.CursorMovement != null)
                {
                    cursor = friendship.CursorMovement.Next;

                    friendship.Users.ForEach(friend =>
                        Console.WriteLine(
                            "ID: {0} Name: {1}",
                            friend.UserIDResponse, friend.ScreenNameResponse)); 
                }

            } while (cursor != 0);
        }

        static async Task FollowersListAsync(TwitterContext twitterCtx)
        {
            Friendship friendship;
            long cursor = -1;
            do
            {
                try
                {
                    friendship =
                        await
                        (from friend in twitterCtx.Friendship
                         where friend.Type == FriendshipType.FollowersList &&
                               friend.ScreenName == "JoeMayo" &&
                               friend.Cursor == cursor
                         select friend)
                        .SingleOrDefaultAsync();
                }
                catch (TwitterQueryException tqe)
                {
                    Console.WriteLine(tqe.ToString());
                    break;
                }

                if (friendship != null && friendship.Users != null)
                {
                    cursor = friendship.CursorMovement.Next;

                    friendship.Users.ForEach(friend =>
                        Console.WriteLine(
                            "ID: {0} Name: {1}",
                            friend.UserIDResponse, friend.ScreenNameResponse));
                }

            } while (cursor != 0);
        }

        static async Task ShowFollowerIDsAsync(TwitterContext twitterCtx)
        {
            Friendship followers;
            long cursor = -1;
            do
            {
                try
                {
                    followers =
                        await
                        (from follower in twitterCtx.Friendship
                         where follower.Type == FriendshipType.FollowerIDs &&
                               follower.UserID == "15411837" &&
                               follower.Cursor == cursor &&
                               follower.Count == 500
                         select follower)
                        .SingleOrDefaultAsync();
                }
                catch (TwitterQueryException tqe)
                {
                    Console.WriteLine(tqe.ToString());
                    break;
                }

                if (followers != null && 
                    followers.IDInfo != null && 
                    followers.IDInfo.IDs != null)
                {
                    cursor = followers.CursorMovement.Next;

                    followers.IDInfo.IDs.ForEach(id =>
                        Console.WriteLine("Follower ID: " + id)); 
                }

            } while (cursor != 0);
        }

        static async Task ShowFriendIDsAsync(TwitterContext twitterCtx)
        {
            Friendship friendList;
            long cursor = -1;
            do
            {
                try
                {
                    friendList =
                        await
                        (from friend in twitterCtx.Friendship
                         where friend.Type == FriendshipType.FriendIDs &&
                               friend.ScreenName == "JoeMayo" &&
                               friend.Cursor == cursor
                         select friend)
                        .SingleOrDefaultAsync();
                }
                catch (TwitterQueryException tqe)
                {
                    Console.WriteLine(tqe.ToString());
                    break;
                }

                if (friendList != null &&
                    friendList.IDInfo != null &&
                    friendList.IDInfo.IDs != null)
                {
                    cursor = friendList.CursorMovement.Next;

                    friendList.IDInfo.IDs.ForEach(id =>
                        Console.WriteLine("Follower ID: " + id));
                }

            } while (cursor != 0);
        }

        static async Task CreateFriendshipAsync(TwitterContext twitterCtx)
        {
            var user = await twitterCtx.CreateFriendshipAsync("JoeMayo", true);

            if (user != null && user.Status != null)
                Console.WriteLine(
                    "User Name: {0}, Status: {1}",
                    user.Name,
                    user.Status.Text);
        }

        static async Task DestroyFriendshipAsync(TwitterContext twitterCtx)
        {
            var user = await twitterCtx.DestroyFriendshipAsync("Linq2Twitr");

            if (user != null && user.Status != null)
                Console.WriteLine(
                    "User Name: {0}, Status: {1}",
                    user.Name,
                    user.Status.Text);
        }

        static async Task UpdateFreindshipSettingsAsync(TwitterContext twitterCtx)
        {
            Friendship friend = 
                await twitterCtx.UpdateFriendshipSettingsAsync(
                    "Linq2Twitr", true, true);

            if (friend != null && friend.SourceRelationship != null)
                Console.WriteLine(
                    "Settings for {0} are: Can Retweet is {1} " +
                    "and Can Send Device Notifications is {2}",
                    friend.SourceRelationship.ScreenName,
                    friend.SourceRelationship.RetweetsWanted,
                    friend.SourceRelationship.NotificationsEnabled);
        }
    }
}
