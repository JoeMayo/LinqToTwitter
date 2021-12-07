using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;
using LinqToTwitter.Common;

namespace ConsoleDemo.CSharp
{
    class ListDemos
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
                        Console.WriteLine("\n\tLooking up list...\n");
                        await GetListAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tGetting owned lists...\n");
                        await GetOwnedListsAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tGetting meberships...\n");
                        await GetListMembershipsAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tGetting members...\n");
                        await GetListMembersAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tGetting followed lists...\n");
                        await GetListsFollowedAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tGetting list followers...\n");
                        await GetListFollowersAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\n\tGetting pinned lists...\n");
                        await GetPinnedListsAsync(twitterCtx);
                        break;
                    case '7':
                        Console.WriteLine("\n\tGetting list tweets...\n");
                        await GetListTweetsAsync(twitterCtx);
                        break;
                    case '8':
                        Console.WriteLine("\n\tDeleting membership...\n");
                        await DeleteMemberFromListAsync(twitterCtx);
                        break;
                    case '9':
                        Console.WriteLine("\n\tAdding follower...\n");
                        await AddFollowerToListAsync(twitterCtx);
                        break;
                    case 'a':
                    case 'A':
                        Console.WriteLine("\n\tDeleting follower...\n");
                        await DeleteFollowerFromListAsync(twitterCtx);
                        break;
                    case 'b':
                    case 'B':
                        Console.WriteLine("\n\tAdding member...\n");
                        await AddMemberToListAsync(twitterCtx);
                        break;
                    case 'c':
                    case 'C':
                        Console.WriteLine("\n\tDeleting list...\n");
                        await DeleteListAsync(twitterCtx);
                        break;
                    case 'd':
                    case 'D':
                        Console.WriteLine("\n\tUpdating list...\n");
                        await UpdateListAsync(twitterCtx);
                        break;
                    case 'e':
                    case 'E':
                        Console.WriteLine("\n\tCreating list...\n");
                        await CreateListAsync(twitterCtx);
                        break;
                    case 'f':
                    case 'F':
                        Console.WriteLine("\n\tPinning list...\n");
                        await PinListAsync(twitterCtx);
                        break;
                    case 'g':
                    case 'G':
                        Console.WriteLine("\n\tUnpinning list...\n");
                        await UnpinListAsync(twitterCtx);
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
            Console.WriteLine("\nList Demos - Please select:\n");

            Console.WriteLine("\t 0. Get Lists by ID");
            Console.WriteLine("\t 1. Get Lists Owned By User");
            Console.WriteLine("\t 2. Get List Memberships");
            Console.WriteLine("\t 3. Get List Members");
            Console.WriteLine("\t 4. Get Lists Followed");
            Console.WriteLine("\t 5. Get List Followers");
            Console.WriteLine("\t 6. Get Pinned Lists");
            Console.WriteLine("\t 7. Get List Tweets");
            Console.WriteLine("\t 8. Delete List Membership");
            Console.WriteLine("\t 9. Add Follower to List");
            Console.WriteLine("\t A. DeleteFollower from List");
            Console.WriteLine("\t B. Add Member to List");
            Console.WriteLine("\t C. Delete List");
            Console.WriteLine("\t D. Update List");
            Console.WriteLine("\t E. Create List");
            Console.WriteLine("\t F. Pin List");
            Console.WriteLine("\t G. Unpin List");

            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task GetListAsync(TwitterContext twitterCtx)
        {
            const string ListID = "898994036043689985";

            var response =
                await
                    (from lst in twitterCtx.List
                     where lst.Type == ListType.Lookup &&
                           lst.ListID == ListID
                     select lst)
                    .SingleOrDefaultAsync();

            List? list = response?.Lists?.FirstOrDefault();
            if (list != null)
                Console.WriteLine(
                    $"ID: {list.ID}, Name: {list.Name}");
        }

        static async Task GetOwnedListsAsync(TwitterContext twitterCtx)
        {
            const string UserID = "15411837";

            var response =
                await
                    (from lst in twitterCtx.List
                     where lst.Type == ListType.Owned &&
                           lst.UserID == UserID &&
                           lst.Expansions == ExpansionField.OwnerID &&
                           lst.ListFields == ListField.AllFields &&
                           lst.UserFields == UserField.AllFields &&
                           lst.MaxResults == 2
                     select lst)
                    .SingleOrDefaultAsync();

            List? list = response?.Lists?.FirstOrDefault();
            if (list != null)
                Console.WriteLine(
                    $"ID: {list.ID}, Description: {list.Description}");
        }

        static async Task GetListMembershipsAsync(TwitterContext twitterCtx)
        {
            const string UserID = "15411837";

            string? pageToken = string.Empty;
            do
            {
                var response =
                    await
                    (from list in twitterCtx.List
                     where list.Type == ListType.Member &&
                           list.UserID == UserID &&
                           list.ListFields == ListField.AllFields &&
                           list.PaginationToken == pageToken
                     select list)
                    .SingleOrDefaultAsync();

                List<List>? lists = response?.Lists;

                if (lists != null)
                    lists.ForEach(list =>
                        Console.WriteLine(
                            "List Name: {0}, Description: {1}",
                            list.Name, list.Description));

                pageToken = response?.Meta?.NextToken;

            } while (pageToken is not null);
        }

        static async Task GetListMembersAsync(TwitterContext twitterCtx)
        {
            const string ListID = "898994036043689985";

            var response =
                await
                (from list in twitterCtx.TwitterUser
                 where list.Type == UserType.ListMembers &&
                       list.ListID == ListID
                 select list)
                .SingleOrDefaultAsync();

            if (response != null && response.Users != null)
                response.Users.ForEach(user =>
                    Console.WriteLine("Member: " + user.Name));
        }

        static async Task GetListsFollowedAsync(TwitterContext twitterCtx)
        {
            const string UserID = "15411837";

            var response =
                await
                    (from lst in twitterCtx.List
                     where lst.Type == ListType.Following &&
                           lst.UserID == UserID &&
                           lst.Expansions == ExpansionField.OwnerID &&
                           lst.ListFields == ListField.AllFields &&
                           lst.UserFields == UserField.AllFields &&
                           lst.MaxResults == 2
                     select lst)
                    .SingleOrDefaultAsync();

            List<List>? lists = response?.Lists;

            if (lists != null)
                lists.ForEach(list =>
                    Console.WriteLine(
                        "List Name: {0}, Description: {1}",
                        list.Name, list.Description));
        }

        static async Task GetListFollowersAsync(TwitterContext twitterCtx)
        {
            const string ListID = "898994036043689985";

            var response =
                await
                (from list in twitterCtx.TwitterUser
                 where list.Type == UserType.ListFollowers &&
                       list.ListID == ListID
                 select list)
                .SingleOrDefaultAsync();

            if (response != null && response.Users != null)
                response.Users.ForEach(user =>
                    Console.WriteLine("Follower: " + user.Name));
        }

        static async Task GetPinnedListsAsync(TwitterContext twitterCtx)
        {
            const string UserID = "15411837";

            var response =
                await
                    (from lst in twitterCtx.List
                     where lst.Type == ListType.Pinned &&
                           lst.UserID == UserID &&
                           lst.Expansions == ExpansionField.OwnerID &&
                           lst.ListFields == ListField.AllFields &&
                           lst.UserFields == UserField.AllFields
                     select lst)
                    .SingleOrDefaultAsync();

            List<List>? lists = response?.Lists;

            if (lists != null)
                lists.ForEach(list =>
                    Console.WriteLine(
                        "List Name: {0}, Description: {1}",
                        list.Name, list.Description));
        }

        static async Task GetListTweetsAsync(TwitterContext twitterCtx)
        {
            const string ListID = "898994036043689985";

            string? pageToken = string.Empty;
            do
            {
                var response =
                    await
                    (from list in twitterCtx.Tweets
                     where list.Type == TweetType.List &&
                           list.ListID == ListID &&
                           list.PaginationToken == pageToken
                     select list)
                    .SingleOrDefaultAsync();

                List<Tweet>? tweets = response?.Tweets;

                if (tweets != null)
                    tweets.ForEach(tweet =>
                        Console.WriteLine(
                            $"\nID: {tweet.ID}" +
                            $"\nTweet: {tweet.Text}"));

                pageToken = response?.Meta?.NextToken;

            } while (pageToken is not null);
        }

        static async Task DeleteMemberFromListAsync(TwitterContext twitterCtx)
        {
            string listID = "0";
            string userID = "Linq2Twitr";

            ListResponse? list =
                await twitterCtx.DeleteMemberFromListAsync(listID, userID);

            if (list?.Data is not null)
                Console.WriteLine("Is Member: {0}", list.Data.IsMember);
        }

        static async Task AddFollowerToListAsync(TwitterContext twitterCtx)
        {
            string listID = "0";
            string userID = "Linq2Twitr";

            ListResponse? list =
                await twitterCtx.AddFollowerToListAsync(listID, userID);

            if (list?.Data is not null)
                Console.WriteLine("Following: {0}", list.Data.Following);
        }

        static async Task DeleteFollowerFromListAsync(TwitterContext twitterCtx)
        {
            string listID = "0";
            string userID = "Linq2Twitr";

            ListResponse? list =
                await twitterCtx.DeleteFollowerFromListAsync(listID, userID);

            if (list?.Data is not null)
                Console.WriteLine("Following: {0}", list.Data.Following);
        }

        static async Task AddMemberToListAsync(TwitterContext twitterCtx)
        {
            ListResponse? list =
                await twitterCtx.AddMemberToListAsync("Linq2Twitr", "0");

            if (list?.Data is not null)
                Console.WriteLine("List ID: {0}, Name: {1}",
                    list.Data.ID, list.Data.Name);
        }

        static async Task DeleteListAsync(TwitterContext twitterCtx)
        {
            string listID = "0";

            ListResponse? list =
                await twitterCtx.DeleteListAsync(listID);

            if (list?.Data is not null)
                Console.WriteLine("Is Deleted: {0}", list.Data.Deleted);
        }

        static async Task UpdateListAsync(TwitterContext twitterCtx)
        {
            string listID = "0";

            ListResponse? list =
                await twitterCtx.UpdateListAsync(listID, "linq-to-twitter", "Test List", isPrivate: false);

            if (list?.Data is not null)
                Console.WriteLine("List ID: {0}, Name: {1}",
                    list.Data.ID, list.Data.Name);
        }

        static async Task CreateListAsync(TwitterContext twitterCtx)
        {
            ListResponse? list =
                await twitterCtx.CreateListAsync("linq-to-twitter", "This is a test", isPrivate: true);

            if (list?.Data is not null)
                Console.WriteLine("List ID: {0}, Name: {1}",
                    list.Data.ID, list.Data.Name);
        }

        static async Task PinListAsync(TwitterContext twitterCtx)
        {
            string listID = "0";
            string userID = "Linq2Twitr";

            ListResponse? list =
                await twitterCtx.PinListAsync(listID, userID);

            if (list?.Data is not null)
                Console.WriteLine("Pinned: {0}", list.Data.Pinned);
        }

        static async Task UnpinListAsync(TwitterContext twitterCtx)
        {
            string listID = "0";
            string userID = "Linq2Twitr";

            ListResponse? list =
                await twitterCtx.UnpinListAsync(listID, userID);

            if (list?.Data is not null)
                Console.WriteLine("Pinned: {0}", list.Data.Pinned);
        }
    }
}
