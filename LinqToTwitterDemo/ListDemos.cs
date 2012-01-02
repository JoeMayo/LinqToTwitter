using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using System.Reflection;
using System.Net;

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
            GetListsDemo(twitterCtx);
            //IsListSubscribedDemo(twitterCtx);
            //GetListSubscribersDemo(twitterCtx);
            //IsListMemberDemo(twitterCtx);
            //GetListMembersDemo(twitterCtx);
            //GetListSubscriptionsDemo(twitterCtx);
            //GetListMembershipsDemo(twitterCtx);
            //GetListStatusesDemo(twitterCtx);
            //ShowListDemo(twitterCtx);
            //CreateListDemo(twitterCtx);
            //UpdateListDemo(twitterCtx);
            //DeleteListDemo(twitterCtx);
            //AddMemberToListDemo(twitterCtx);
            //AddMemberRangeToListWithScreenNamesDemo(twitterCtx);
            //AddMemberRangeToListWithUserIDsDemo(twitterCtx);
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
            var screenNames = new List<string>
            {
                "JoeMayo",
                "Linq2Tweeter"
            };

            List list = twitterCtx.AddMemberRangeToList(null, "linq", null, "Linq2Tweeter", screenNames);

            foreach (var user in list.Users)
            {
                Console.WriteLine(user.Name);
            }
        }

        private static void AddMemberRangeToListWithUserIDsDemo(TwitterContext twitterCtx)
        {
            var userIds = new List<ulong>
            {
                15411837,
                16761255
            };

            List list = twitterCtx.AddMemberRangeToList(null, "test", null, "Linq2Tweeter", userIds);

            foreach (var user in list.Users)
            {
                Console.WriteLine(user.Name);
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
            List list = twitterCtx.UnsubscribeFromList(null, "twitterhq", null, "JoeMayo");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how a user can subscribe to a list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void SubscribeToListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.SubscribeToList(null, "dotnettwittterdevs", null,  "JoeMayo");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how to remove a member from a list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DeleteMemberFromListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.DeleteMemberFromList(null, "Linq2Tweeter", null, "test", null, "Linq2Tweeter");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how to add a member to a list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void AddMemberToListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.AddMemberToList(null, "Linq2Tweeter",  null, "test", null, "Linq2Tweeter");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how to delete a list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void DeleteListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.DeleteList(null, "test-5", null, "Linq2Tweeter");

            Console.WriteLine("List Name: {0}, Description: {1}",
                list.Name, list.Description);
        }

        /// <summary>
        /// Shows how to modify an existing list
        /// </summary>
        /// <param name="twitterCtx">TwitterContext</param>
        private static void UpdateListDemo(TwitterContext twitterCtx)
        {
            List list = twitterCtx.UpdateList(null, "test", null, "Linq2Tweeter", "public", "This is a test2");

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
        private static void ShowListDemo(TwitterContext twitterCtx)
        {
            var requestedList =
                (from list in twitterCtx.List
                 where list.Type == ListType.Show &&
                       list.OwnerScreenName == "JoeMayo" && // user who owns list
                       list.Slug == "dotnettwittterdevs" // list name
                 select list)
                .FirstOrDefault();

            Console.WriteLine("List Name: {0}, Description: {1}, # Users: {2}",
                requestedList.Name, requestedList.Description, requestedList.Users.Count());
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
                       list.OwnerScreenName == "JoeMayo" &&
                       list.Slug == "dotnettwittterdevs" // name of list to get statuses for
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
                       list.OwnerScreenName == "JoeMayo" &&
                       list.Slug == "dotnettwittterdevs"
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
                         list.ScreenName == "MichaelJordan" &&
                         list.OwnerScreenName == "Linq2Tweeter" &&
                         list.Slug == "linq"
                    select list)
                    .FirstOrDefault();

                // list will have only one user matching ID in query
                var user = subscribedList.Users.First();

                Console.WriteLine("User: {0} is a member of List: {1}",
                    user.Name, subscribedList.ListID);
            }
            // whenever user is not subscribed to the specified list, Twitter
            // returns an HTTP 404 Not Found, response, which results in a
            // .NET exception.  LINQ to Twitter intercepts the HTTP exception
            // and wraps it in a TwitterQueryResponse where you can read the
            // error message from Twitter via the Response property, shown below.
            catch (TargetInvocationException ex)
            {
                // because of reflection, the original error gets wrapped inside a TargetInvocationException
                var twitterQryEx = ex.InnerException as TwitterQueryException;
                if (twitterQryEx == null) throw;

                // TwitterQueryException will always reference the original WebException, so the check is redundant but doesn't hurt
                var webEx = twitterQryEx.InnerException as WebException;
                if (webEx == null) throw twitterQryEx;

                // The response holds data from Twitter
                var webResponse = webEx.Response as HttpWebResponse;
                if (webResponse == null) throw twitterQryEx;


                if (webResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(
                        "HTTP Status Code: {0}. Response from Twitter: {1}",
                        webEx.Response.Headers["Status"],
                        twitterQryEx.Response.Error); 
                }
                else
                {
                    throw twitterQryEx;
                }
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
                       list.Slug == "dotnettwittterdevs" &&
                       list.OwnerScreenName == "JoeMayo"
                 select list)
                 .First();

            foreach (var user in lists.Users)
            {
                Console.WriteLine("Subscriber: " + user.Name);
            }
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
                Console.WriteLine("ID: {0}  Slug: {1} Description: {2}",
                    list.ListIDResult, list.SlugResult, list.Description);
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
                          list.Slug == "dotnettwittterdevs" &&
                          list.OwnerScreenName == "JoeMayo"
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
            catch (TargetInvocationException ex)
            {
                // because of reflection, the original error gets wrapped inside a TargetInvocationException
                var twitterQryEx = ex.InnerException as TwitterQueryException;
                if (twitterQryEx == null) throw;

                // TwitterQueryException will always reference the original WebException, so the check is redundant but doesn't hurt
                var webEx = twitterQryEx.InnerException as WebException;
                if (webEx == null) throw twitterQryEx;

                // The response holds data from Twitter
                var webResponse = webEx.Response as HttpWebResponse;
                if (webResponse == null) throw twitterQryEx;


                if (webResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(
                        "HTTP Status Code: {0}. Response from Twitter: {1}",
                        webEx.Response.Headers["Status"],
                        twitterQryEx.Response.Error);
                }
                else
                {
                    throw twitterQryEx;
                }
            }
        }

        #endregion
    }
}
