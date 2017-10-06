using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Linq2TwitterDemos_Console
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
                        Console.WriteLine("\n\tGetting Lists...\n");
                        await GetListsForUserAsync(twitterCtx);
                        break;
                    case '1':
                        Console.WriteLine("\n\tGetting statuses...\n");
                        await GetListStatusesAsync(twitterCtx);
                        break;
                    case '2':
                        Console.WriteLine("\n\tGetting members...\n");
                        await GetListMembershipsAsync(twitterCtx);
                        break;
                    case '3':
                        Console.WriteLine("\n\tGetting subscribers...\n");
                        await GetListSubscribersAsync(twitterCtx);
                        break;
                    case '4':
                        Console.WriteLine("\n\tChecking user subscription...\n");
                        await ShowIsListSubscriberAsync(twitterCtx);
                        break;
                    case '5':
                        Console.WriteLine("\n\tChecing user membership...\n");
                        await ShowIsListMemberAsync(twitterCtx);
                        break;
                    case '6':
                        Console.WriteLine("\n\tGetting members...\n");
                        await GetListMembersAsync(twitterCtx);
                        break;
                    case '7':
                        Console.WriteLine("\n\tShowing details...\n");
                        await ShowListDetailsAsync(twitterCtx);
                        break;
                    case '8':
                        Console.WriteLine("\n\tGetting subscriptions...\n");
                        await GetListSubscriptionsAsync(twitterCtx);
                        break;
                    case '9':
                        Console.WriteLine("\n\tGetting ownership...\n");
                        await GetOwnershipsAsync(twitterCtx);
                        break;
                    case 'a':
                    case 'A':
                        Console.WriteLine("\n\tDeleting membership...\n");
                        await DeleteMemberFromListAsync(twitterCtx);
                        break;
                    case 'b':
                    case 'B':
                        Console.WriteLine("\n\tSubscribing...\n");
                        await SubscribeToListAsync(twitterCtx);
                        break;
                    case 'c':
                    case 'C':
                        Console.WriteLine("\n\tUnsubscribing...\n");
                        await UnsubscribeFromListAsync(twitterCtx);
                        break;
                    case 'd':
                    case 'D':
                        Console.WriteLine("\n\tAdding members...\n");
                        await AddMemberRangeToListAsync(twitterCtx);
                        break;
                    case 'e':
                    case 'E':
                        Console.WriteLine("\n\tAdding member...\n");
                        await AddMemberToListAsync(twitterCtx);
                        break;
                    case 'f':
                    case 'F':
                        Console.WriteLine("\n\tDeleting list...\n");
                        await DeleteListAsync(twitterCtx);
                        break;
                    case 'g':
                    case 'G':
                        Console.WriteLine("\n\tUpdating list...\n");
                        await UpdateListAsync(twitterCtx);
                        break;
                    case 'h':
                    case 'H':
                        Console.WriteLine("\n\tCreating list...\n");
                        await CreateListAsync(twitterCtx);
                        break;
                    case 'i':
                    case 'I':
                        Console.WriteLine("\n\tRemoving memberships...\n");
                        await DeleteMemberRangeFromListAsync(twitterCtx);
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

            Console.WriteLine("\t 0. Get Lists for User");
            Console.WriteLine("\t 1. Get List Statuses");
            Console.WriteLine("\t 2. Get List Memberships");
            Console.WriteLine("\t 3. Get List Subscribers");
            Console.WriteLine("\t 4. Check User Subscription");
            Console.WriteLine("\t 5. Check User Membership");
            Console.WriteLine("\t 6. Get List Members");
            Console.WriteLine("\t 7. Show List Details.");
            Console.WriteLine("\t 8. Get List Subscriptions");
            Console.WriteLine("\t 9. Get List Ownership");
            Console.WriteLine("\t A. Delete List Membership");
            Console.WriteLine("\t B. Subscribe to List");
            Console.WriteLine("\t C. Unsubscribe from List");
            Console.WriteLine("\t D. Add Members to List");
            Console.WriteLine("\t E. Add Member to List");
            Console.WriteLine("\t F. Delete List");
            Console.WriteLine("\t G. Update List");
            Console.WriteLine("\t H. Create List");
            Console.WriteLine("\t I. Remove Members from List");

            Console.WriteLine();
            Console.Write("\t Q. Return to Main menu");
        }

        static async Task GetListsForUserAsync(TwitterContext twitterCtx)
        {
            string screenName = "Linq2Twitr";

            var lists =
                await
                    (from list in twitterCtx.List
                     where list.Type == ListType.List &&
                           list.ScreenName == screenName
                     select list)
                    .ToListAsync();

            if (lists != null)
                lists.ForEach(list => Console.WriteLine("Slug: " + list.SlugResponse));
        }

        static async Task GetListStatusesAsync(TwitterContext twitterCtx)
        {
            string ownerScreenName = "Linq2Twitr";
            string slug = "linq";
            int maxStatuses = 30;
            int lastStatusCount = 0;
            // last tweet processed on previous query
            ulong sinceID = 204251866668871681; 
            ulong maxID;
            int count = 10;
            var statusList = new List<Status>();

            // only count
            var listResponse =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Statuses &&
                       list.OwnerScreenName == ownerScreenName &&
                       list.Slug == slug &&
                       list.Count == count
                 select list)
                .SingleOrDefaultAsync();

            if (listResponse != null && listResponse.Statuses != null)
            {
                List<Status> newStatuses = listResponse.Statuses;
                // first tweet processed on current query
                maxID = newStatuses.Min(status => status.StatusID) - 1; 
                statusList.AddRange(newStatuses);

                do
                {
                    // now add sinceID and maxID
                    listResponse =
                        await
                        (from list in twitterCtx.List
                         where list.Type == ListType.Statuses &&
                               list.OwnerScreenName == ownerScreenName &&
                               list.Slug == slug &&
                               list.Count == count &&
                               list.SinceID == sinceID &&
                               list.MaxID == maxID
                         select list)
                        .SingleOrDefaultAsync();

                    if (listResponse == null)
                        break;

                    newStatuses = listResponse.Statuses;
                    // first tweet processed on current query
                    maxID = newStatuses.Min(status => status.StatusID) - 1; 
                    statusList.AddRange(newStatuses);

                    lastStatusCount = newStatuses.Count;
                }
                while (lastStatusCount != 0 && statusList.Count < maxStatuses);

                for (int i = 0; i < statusList.Count; i++)
                {
                    Status status = statusList[i];

                    Console.WriteLine("{0, 4}. [{1}] User: {2}\nStatus: {3}",
                        i + 1, status.StatusID, status.User.Name, status.Text);
                }
            }
        }

        static async Task GetListMembershipsAsync(TwitterContext twitterCtx)
        {
            var lists =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Memberships &&
                       list.ScreenName == "JoeMayo" // user to get memberships for
                 select list)
                .ToListAsync();

            if (lists != null)
                lists.ForEach(list =>
                    Console.WriteLine(
                        "List Name: {0}, Description: {1}",
                        list.Name, list.Description));
        }

        static async Task GetListSubscribersAsync(TwitterContext twitterCtx)
        {
            var subscriberList =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Subscribers &&
                       list.Slug == "linq" &&
                       list.OwnerScreenName == "Linq2Twitr"
                 select list)
                .SingleOrDefaultAsync();

            if (subscriberList != null && subscriberList.Users != null)
                subscriberList.Users.ForEach(user =>
                    Console.WriteLine("Subscriber: " + user.Name));
        }

        static async Task ShowIsListSubscriberAsync(TwitterContext twitterCtx)
        {
            try
            {
                var subscribedList =
                    await
                    (from list in twitterCtx.List
                     where list.Type == ListType.IsSubscriber &&
                           list.ScreenName == "JoeMayo" &&
                           list.Slug == "linq" &&
                           list.OwnerScreenName == "Linq2Twitr"
                     select list)
                    .SingleOrDefaultAsync();

                if (subscribedList != null && subscribedList.Users != null)
                {
                    // list will have only one user matching ID in query
                    var user = subscribedList.Users.First();

                    Console.WriteLine("User: {0} is subscribed to List: {1}",
                        user.Name, subscribedList.ListID); 
                }
            }
            // whenever user is not subscribed to the specified list, Twitter
            // returns an HTTP 404, Not Found, response.  LINQ to Twitter 
            // intercepts the HTTP response and wraps it in a TwitterQueryException 
            // where you can read the error message from Twitter via the Message property.
            catch (TwitterQueryException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(
                        "HTTP Status Code: {0}. Twitter Error Code: {1}, Twitter Message: {2}",
                        ex.StatusCode.ToString(),
                        ex.ErrorCode,
                        ex.Message);
                }
                else
                {
                    throw ex;
                }
            }
        }

        static async Task ShowIsListMemberAsync(TwitterContext twitterCtx)
        {
            try
            {
                var subscribedList =
                    await
                    (from list in twitterCtx.List
                     where list.Type == ListType.IsMember &&
                           list.ScreenName == "JoeMayo" &&
                           list.OwnerScreenName == "Linq2Twitr" &&
                           list.Slug == "linq"
                     select list)
                    .SingleOrDefaultAsync();

                if (subscribedList != null && subscribedList.Users != null)
                {
                    // list will have only one user matching ID in query
                    var user = subscribedList.Users.First();

                    Console.WriteLine("User: {0} is a member of List: {1}",
                        user.Name, subscribedList.ListID); 
                }
            }
            // whenever user is not a member of the specified list, Twitter
            // returns an HTTP 404, Not Found, response.  LINQ to Twitter 
            // intercepts the HTTP response and wraps it in a TwitterQueryException 
            // where you can read the error message from Twitter via the Message property.
            catch (TwitterQueryException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(
                        "HTTP Status Code: {0}. Twitter Error Code: {1}, Twitter Message: {2}",
                        ex.StatusCode.ToString(),
                        ex.ErrorCode,
                        ex.Message);
                }
                else
                {
                    throw ex;
                }
            }
        }

        static async Task GetListMembersAsync(TwitterContext twitterCtx)
        {
            var lists =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Members &&
                       list.OwnerScreenName == "Linq2Twitr" &&
                       list.Slug == "linq" &&
                       list.SkipStatus == true
                 select list)
                .SingleOrDefaultAsync();

            if (lists != null && lists.Users != null)
                lists.Users.ForEach(user =>
                    Console.WriteLine("Member: " + user.Name));
        }

        static async Task ShowListDetailsAsync(TwitterContext twitterCtx)
        {
            var requestedList =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Show &&
                       list.OwnerScreenName == "Linq2Twitr" &&
                       list.Slug == "linq"
                 select list)
                .SingleOrDefaultAsync();

            if (requestedList != null)
                Console.WriteLine(
                    "List Name: {0}, Description: {1}, # Users: {2}",
                    requestedList.Name, 
                    requestedList.Description, 
                    requestedList.Users.Count());
        }

        static async Task GetListSubscriptionsAsync(TwitterContext twitterCtx)
        {
            var lists =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Subscriptions &&
                       list.ScreenName == "Linq2Twitr"
                 select list)
                .ToListAsync();

            if (lists != null)
                lists.ForEach(list =>
                    Console.WriteLine(
                        "List Name: {0}, Description: {1}",
                        list.Name, list.Description));
        }

        static async Task GetOwnershipsAsync(TwitterContext twitterCtx)
        {
            var lists =
                await
                (from list in twitterCtx.List
                 where list.Type == ListType.Ownerships &&
                       list.ScreenName == "Linq2Twitr"
                 select list)
                .ToListAsync();

            if (lists != null)
                lists.ForEach(list =>
                    Console.WriteLine(
                        "ID: {0}  Slug: {1} Description: {2}",
                        list.ListIDResponse, 
                        list.SlugResponse, 
                        list.Description));
        }

        static async Task DeleteMemberFromListAsync(TwitterContext twitterCtx)
        {
            string ownerScreenName = "Linq2Twitr";

            List list = 
                await twitterCtx.DeleteMemberFromListAsync(
                    0, "Linq2Twitr", 0, "testDemo", 0, ownerScreenName);

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
        }

        static async Task SubscribeToListAsync(TwitterContext twitterCtx)
        {
            string ownerScreenName = "Linq2Twitr";

            List list = 
                await twitterCtx.SubscribeToListAsync(
                    0, "testDemo", 0, ownerScreenName);

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
        }

        static async Task UnsubscribeFromListAsync(TwitterContext twitterCtx)
        {
            string ownerScreenName = "Linq2Twitr";

            List list = 
                await twitterCtx.UnsubscribeFromListAsync(
                    0, "testDemo", 0, ownerScreenName);

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
        }

        static async Task AddMemberRangeToListAsync(TwitterContext twitterCtx)
        {
            string ownerScreenName = "Linq2Twitr";
            var screenNames = new List<string>
            {
                "JoeMayo",
                "Linq2Twitr"
            };

            List list = 
                await twitterCtx.AddMemberRangeToListAsync(
                    0, "testDemo", 0, ownerScreenName, screenNames);

            if (list != null && list.Users != null)
                list.Users.ForEach(user => Console.WriteLine(user.Name));
        }

        static async Task AddMemberToListAsync(TwitterContext twitterCtx)
        {
            string ownerScreenName = "Linq2Twitr";

            List list = 
                await twitterCtx.AddMemberToListAsync(
                    "Linq2Twitr", 0, "testDemo", 0, ownerScreenName);

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
        }

        static async Task DeleteListAsync(TwitterContext twitterCtx)
        {
            ulong listID = 0;

            List list = 
                await twitterCtx.DeleteListAsync(
                    listID, "testDemo", 0, "Linq2Twitr");

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
        }

        static async Task UpdateListAsync(TwitterContext twitterCtx)
        {
            string ownerScreenName = "Linq2Twitr";
            ulong listID = 0;

            List list = 
                await twitterCtx.UpdateListAsync(
                    listID, "testDemo", "Test List", 0, 
                    ownerScreenName, "public", "This is a test2");

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
        }

        static async Task CreateListAsync(TwitterContext twitterCtx)
        {
            List list = 
                await twitterCtx.CreateListAsync(
                    "testDemo", "public", "This is a test");

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
        }

        static async Task DeleteMemberRangeFromListAsync(TwitterContext twitterCtx)
        {
            string ownerScreenName = "Linq2Twitr";
            var screenNames = new List<string> { "JoeMayo", "mp2kmag" };

            List list = 
                await twitterCtx.DeleteMemberRangeFromListAsync(
                    0, "testDemo", screenNames, 0, ownerScreenName);

            if (list != null)
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
        }
    }
}
