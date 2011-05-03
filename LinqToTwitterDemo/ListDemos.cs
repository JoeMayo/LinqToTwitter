using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;

namespace LinqToTwitterDemo
{
    /// <summary>
    /// Shows list demos
    /// </summary>
    public class ListDemos
    {
        /// <summary>
        /// Run all list related demos
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        public static void Run(TwitterContext twitterCtx)
        {
            //GetListsDemo(twitterCtx);
            //IsListSubscribedDemo(twitterCtx);
            //GetListSubscribersDemo(twitterCtx);
            //IsListMemberDemo(twitterCtx);
            GetListMembersDemo(twitterCtx);
            //GetListSubscriptionsDemo(twitterCtx);
            //GetListMembershipsDemo(twitterCtx);
            //GetListStatusesDemo(twitterCtx);
            //GetListDemo(twitterCtx);
            //CreateListDemo(twitterCtx);
            //UpdateListDemo(twitterCtx);
            //DeleteListDemo(twitterCtx);
            //AddMemberToListDemo(twitterCtx);
            //AddMemberRangeToListWithScreenNamesDemo(twitterCtx);
            //AddMemberRangeToListWithUserIDsDemo(twitterCtx);
            //AddMemberRangeToListWithTooManyIDsDemo(twitterCtx);
            //DeleteMemberFromListDemo(twitterCtx);
            //SubscribeToListDemo(twitterCtx);
            //UnsubscribeFromListDemo(twitterCtx);
            //ListSortDemo(twitterCtx);
            //GetAllSubscribedListsDemo(twitterCtx);
        }

        private static void GetAllSubscribedListsDemo(TwitterContext twitterCtx)
        {
            var lists =
                (from list in twitterCtx.List
                 where list.Type == ListType.All &&
                       list.ScreenName == "JoeMayo"
                 select list)
                .ToList();

            lists.ForEach(list => Console.WriteLine("Slug: " + list.Slug));
        }

        private static void AddMemberRangeToListWithScreenNamesDemo(TwitterContext twitterCtx)
        {
            var members = new List<string>
            {
                "JoeMayo",
                "Mayoster"
            };

            List list = twitterCtx.AddMemberRangeToList("Linq2Tweeter", "linq", members);

            foreach (var user in list.Users)
            {
                Console.WriteLine(user.Name);
            }
        }

        private static void AddMemberRangeToListWithUserIDsDemo(TwitterContext twitterCtx)
        {
            var members = new List<ulong>
            {
                15411837,
                45714308
            };

            List list = twitterCtx.AddMemberRangeToList("Linq2Tweeter", "linq", members);

            foreach (var user in list.Users)
            {
                Console.WriteLine(user.Name);
            }
        }

        private static void AddMemberRangeToListWithTooManyIDsDemo(TwitterContext twitterCtx)
        {
            var members = new List<ulong>(Enumerable.Range(1, 101).Select(id => (ulong)id));

            try
            {
                List list = twitterCtx.AddMemberRangeToList("Linq2Tweeter", "linq", members);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("This is what we expect: \n" + ae.ToString());
            }
        }

        #region List Demos

        /// <summary>
        /// Shows how to get a list and sort it
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void ListSortDemo(TwitterContext twitterCtx)
        {
            var lists =
                from list in twitterCtx.List
                where list.Type == ListType.Lists &&
                      list.ScreenName == "Linq2Tweeter"
                orderby list.Name
                select list;

            foreach (var list in lists)
            {
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
            }
        }

        /// <summary>
        /// Shows how a user can unsubscribe from a list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UnsubscribeFromListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.UnsubscribeFromList("Linq2Tweeter", "linq");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how a user can subscribe to a list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SubscribeToListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.SubscribeToList("Linq2Tweeter", "linq");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how to remove a member from a list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DeleteMemberFromListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.DeleteMemberFromList("Linq2Tweeter", "linq", "15411837");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how to add a member to a list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void AddMemberToListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.AddMemberToList("Linq2Tweeter", "linq", "15411837");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how to delete a list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DeleteListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.DeleteList("44351713");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how to modify an existing list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.UpdateList("Linq2Tweeter", "44351713", "test2", "public", "This is a test2");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how to create a new list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void CreateListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.CreateList("test", "public", "This is a test");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how to get information for a specific list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetListDemo(TwitterContext twitterCtx)
        {
            var requestedList =
                (from list in twitterCtx.List
                 where list.Type == ListType.List &&
                       list.ScreenName == "Linq2Tweeter" && // user to get memberships for
                       list.ID == "mvc" // ID of list
                 select list)
                 .FirstOrDefault();

            Console.WriteLine("List Name: {0}, Description: {1}",
                requestedList.Name, requestedList.Description);
        }

        /// <summary>
        /// Gets a list of statuses for specified list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetListStatusesDemo(TwitterContext twitterCtx)
        {
            var statusList =
                (from list in twitterCtx.List
                 where list.Type == ListType.Statuses &&
                       list.ScreenName == "Linq2Tweeter" &&
                       list.ListID == "3897016" // ID of list to get statuses for
                 select list)
                 .First();

            foreach (var status in statusList.Statuses)
            {
                Console.WriteLine("User: {0}, Status: {1}",
                    status.User.Name, status.Text);
            }
        }

        /// <summary>
        /// Gets a list of memberships for a user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetListMembershipsDemo(TwitterContext twitterCtx)
        {
            var lists =
                from list in twitterCtx.List
                where list.Type == ListType.Memberships &&
                      list.ScreenName == "JoeMayo" // user to get memberships for
                select list;

            foreach (var list in lists)
            {
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
            }
        }

        /// <summary>
        /// Gets a list of subscriptions for a user
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetListSubscriptionsDemo(TwitterContext twitterCtx)
        {
            var lists =
                from list in twitterCtx.List
                where list.Type == ListType.Subscriptions &&
                      list.ScreenName == "Linq2Tweeter" // user to get subscriptions for
                select list;

            foreach (var list in lists)
            {
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
            }
        }

        /// <summary>
        /// Gets a list of members of a list
        /// </summary>
        /// <param name="twitterCtx">Twitter Context</param>
        private static void GetListMembersDemo(TwitterContext twitterCtx)
        {
            var lists =
                (from list in twitterCtx.List
                 where list.Type == ListType.Members &&
                       list.ScreenName == "Linq2Tweeter" &&
                       list.ListID == "3897006" // ID of list
                 select list)
                 .First();

            foreach (var user in lists.Users)
            {
                Console.WriteLine("Member: " + user.Name);
            }
        }

        /// <summary>
        /// Sees if user is a member of specified list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void IsListMemberDemo(TwitterContext twitterCtx)
        {
            try
            {
                var subscribedList =
                   (from list in twitterCtx.List
                    where list.Type == ListType.IsMember &&
                         list.ScreenName == "Linq2Tweeter" &&
                         list.ID == "15411837" && // ID of user
                         list.ListID == "3897006" // ID of list
                    select list)
                    .FirstOrDefault();

                // list will have only one user matching ID in query
                var user = subscribedList.Users.First();

                Console.WriteLine("User: {0} is a member of List: {1}",
                    user.Name, subscribedList.ListID);
            }
            // whenever user is not subscribed to the specified list, Twitter
            // returns an HTTP 404, Not Found, response, which results in a
            // .NET exception.  LINQ to Twitter intercepts the HTTP exception
            // and wraps it in a TwitterQueryResponse where you can read the
            // error message from Twitter via the Response property, shown below.
            catch (TwitterQueryException tqe)
            {
                Console.WriteLine(
                    "User is not a member of List. Response from Twitter: " +
                    tqe.Response.Error);
            }
        }

        /// <summary>
        /// Gets a list of subscribers for specified list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetListSubscribersDemo(TwitterContext twitterCtx)
        {
            var lists =
                (from list in twitterCtx.List
                 where list.Type == ListType.Subscribers &&
                       list.ScreenName == "Linq2Tweeter" &&
                       list.ListID == "3897016" // ID of list
                 select list)
                 .First();

            foreach (var user in lists.Users)
            {
                Console.WriteLine("Subscriber: " + user.Name);
            }
        }

        private class ListDetails
        {
            public string Name { get; set; }

            public string Description { get; set; }
        }

        /// <summary>
        /// Gets lists that user created
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void GetListsDemo(TwitterContext twitterCtx)
        {
            var lists =
                (from list in twitterCtx.List
                 where list.Type == ListType.Lists &&
                       list.ScreenName == "Linq2Tweeter"
                 select list)
                .ToList();

            foreach (var list in lists)
            {
                Console.WriteLine("List Name: {0}, Description: {1}",
                    list.Name, list.Description);
            }
        }

        /// <summary>
        /// Sees if user is subscribed to specified list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void IsListSubscribedDemo(TwitterContext twitterCtx)
        {
            try
            {
                var subscribedList =
                   (from list in twitterCtx.List
                    where list.Type == ListType.IsSubscribed &&
                         list.ScreenName == "Linq2Tweeter" &&
                         list.ID == "15411837" && // ID of user
                         list.ListID == "3897016" // ID of list
                    select list)
                    .FirstOrDefault();

                // list will have only one user matching ID in query
                var user = subscribedList.Users.First();

                Console.WriteLine("User: {0} is subscribed to List: {1}",
                    user.Name, subscribedList.ListID);
            }
            // whenever user is not subscribed to the specified list, Twitter
            // returns an HTTP 404, Not Found, response, which results in a
            // .NET exception.  LINQ to Twitter intercepts the HTTP exception
            // and wraps it in a TwitterQueryResponse where you can read the
            // error message from Twitter via the Response property, shown below.
            catch (TwitterQueryException tqe)
            {
                Console.WriteLine(
                    "User is not subscribed to List. Response from Twitter: " +
                    tqe.Response.Error);
            }
        }

        #endregion
    }
}
