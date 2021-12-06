using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                    //case '2':
                    //    Console.WriteLine("\n\tGetting members...\n");
                    //    await GetListMembershipsAsync(twitterCtx);
                    //    break;
                    //case '3':
                    //    Console.WriteLine("\n\tGetting subscribers...\n");
                    //    await GetListSubscribersAsync(twitterCtx);
                    //    break;
                    //case '4':
                    //    Console.WriteLine("\n\tChecking user subscription...\n");
                    //    await ShowIsListSubscriberAsync(twitterCtx);
                    //    break;
                    //case '5':
                    //    Console.WriteLine("\n\tChecing user membership...\n");
                    //    await ShowIsListMemberAsync(twitterCtx);
                    //    break;
                    //case '6':
                    //    Console.WriteLine("\n\tGetting members...\n");
                    //    await GetListMembersAsync(twitterCtx);
                    //    break;
                    //case '7':
                    //    Console.WriteLine("\n\tShowing details...\n");
                    //    await ShowListDetailsAsync(twitterCtx);
                    //    break;
                    //case '8':
                    //    Console.WriteLine("\n\tGetting subscriptions...\n");
                    //    await GetListSubscriptionsAsync(twitterCtx);
                    //    break;
                    //case '9':
                    //    Console.WriteLine("\n\tGetting ownership...\n");
                    //    await GetOwnershipsAsync(twitterCtx);
                    //    break;
                    //case 'a':
                    //case 'A':
                    //    Console.WriteLine("\n\tDeleting membership...\n");
                    //    await DeleteMemberFromListAsync(twitterCtx);
                    //    break;
                    //case 'b':
                    //case 'B':
                    //    Console.WriteLine("\n\tAdding follower...\n");
                    //    await AddFollowerToListAsync(twitterCtx);
                    //    break;
                    //case 'c':
                    //case 'C':
                    //    Console.WriteLine("\n\tDeleting follower...\n");
                    //    await DeleteFollowerFromListAsync(twitterCtx);
                    //    break;
                    //case 'd':
                    //case 'D':
                    //    Console.WriteLine("\n\tAdding member...\n");
                    //    await AddMemberToListAsync(twitterCtx);
                    //    break;
                    //case 'e':
                    //case 'E':
                    //    Console.WriteLine("\n\tDeleting list...\n");
                    //    await DeleteListAsync(twitterCtx);
                    //    break;
                    //case 'f':
                    //case 'F':
                    //    Console.WriteLine("\n\tUpdating list...\n");
                    //    await UpdateListAsync(twitterCtx);
                    //    break;
                    //case 'g':
                    //case 'G':
                    //    Console.WriteLine("\n\tCreating list...\n");
                    //    await CreateListAsync(twitterCtx);
                    //    break;
                    //case 'h':
                    //case 'H':
                    //    Console.WriteLine("\n\tPinning list...\n");
                    //    await PinListAsync(twitterCtx);
                    //    break;
                    //case 'i':
                    //case 'I':
                    //    Console.WriteLine("\n\tUnpinning list...\n");
                    //    await UnpinListAsync(twitterCtx);
                    //    break;
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
            Console.WriteLine("\t 3. Get List Subscribers");
            Console.WriteLine("\t 4. Check User Subscription");
            Console.WriteLine("\t 5. Check User Membership");
            Console.WriteLine("\t 6. Get List Members");
            Console.WriteLine("\t 7. Show List Details.");
            Console.WriteLine("\t 8. Get List Subscriptions");
            Console.WriteLine("\t 9. Get List Ownership");
            Console.WriteLine("\t A. Delete List Membership");
            Console.WriteLine("\t B. Add Follower to List");
            Console.WriteLine("\t C. DeleteFollower from List");
            Console.WriteLine("\t D. Add Member to List");
            Console.WriteLine("\t E. Delete List");
            Console.WriteLine("\t F. Update List");
            Console.WriteLine("\t G. Create List");
            Console.WriteLine("\t H. Pin List");
            Console.WriteLine("\t I. Unpin List");

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
    }
}
