using System;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LinqToTwitter
{
    public static class ListExtensions
    {
        public const string ListIDOrSlugParam = "ListIdOrSlug";
        public const string OwnerIDOrOwnerScreenNameParam = "OwnerIdOrOwnerScreenName";
        public const string UserIDOrScreenNameParam = "UserIdOrScreenName";

        /// <summary>
        /// Creates a new list
        /// </summary>
        /// <param name="listName">name of list</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <returns>List info for new list</returns>
        public static List CreateList(this TwitterContext ctx, string listName, string mode, string description)
        {
            return CreateList(ctx, listName, mode, description, null);
        }

        /// <summary>
        /// Creates a new list
        /// </summary>
        /// <param name="listName">name of list</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for new list</returns>
        public static List CreateList(this TwitterContext ctx, string listName, string mode, string description, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listName))
            {
                throw new ArgumentException("listName is required.", "listName");
            }

            var createUrl = ctx.BaseUrl + "lists/create.json";

            var reqProc = new ListRequestProcessor<List>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    createUrl,
                    new Dictionary<string, string>
                    {
                        { "name", listName },
                        { "mode", mode },
                        { "description", description }
                    },
                    reqProc);

            List results = reqProc.ProcessActionResult(resultsJson, ListAction.Create);
            return results;
        }

        /// <summary>
        /// Modifies an existing list
        /// </summary>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <returns>List info for modified list</returns>
        public static List UpdateList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName, string mode, string description)
        {
            return UpdateList(ctx, listID, slug, ownerID, ownerScreenName, mode, description, null);
        }

        /// <summary>
        /// Modifies an existing list
        /// </summary>
        /// <param name="listID">ID of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for modified list</returns>
        public static List UpdateList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName, string mode, string description, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var updateListUrl = ctx.BaseUrl + "lists/update.json";

            var reqProc = new ListRequestProcessor<List>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    updateListUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                        { "mode", mode },
                        { "description", description }
                    },
                    reqProc);

            List results = reqProc.ProcessActionResult(resultsJson, ListAction.Update);
            return results;
        }

        /// <summary>
        /// Deletes an existing list
        /// </summary>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for deleted list</returns>
        public static List DeleteList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName)
        {
            return DeleteList(ctx, listID, slug, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Deletes an existing list
        /// </summary>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for deleted list</returns>
        public static List DeleteList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("listID is required.", ListIDOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var deleteUrl = ctx.BaseUrl + "lists/destroy.json";

            var reqProc = new ListRequestProcessor<List>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    deleteUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName }
                    },
                    reqProc);

            List results = reqProc.ProcessActionResult(resultsJson, ListAction.Delete);
            return results;
        }

        /// <summary>
        /// Adds a user as a list member.
        /// </summary>
        /// <param name="userID">ID of user to add to list.</param>
        /// <param name="screenName">ScreenName of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list member added to.</returns>
        public static List AddMemberToList(this TwitterContext ctx, string userID, string screenName, string listID, string slug, string ownerID, string ownerScreenName)
        {
            return AddMemberToList(ctx, userID, screenName, listID, slug, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Adds a user as a list member
        /// </summary>
        /// <param name="userID">ID of user to add to list.</param>
        /// <param name="screenName">ScreenName of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list member added to</returns>
        public static List AddMemberToList(this TwitterContext ctx, string userID, string screenName, string listID, string slug, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(userID) && string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either userID or screenName is required.", UserIDOrScreenNameParam);
            }

            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var addMemberUrl = ctx.BaseUrl + "lists/members/create.json";

            var reqProc = new ListRequestProcessor<List>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    addMemberUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName },
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    },
                    reqProc);

            List results = reqProc.ProcessActionResult(resultsJson, ListAction.AddMember);
            return results;
        }

        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="screenNames">List of user screen names to be list members.</param>
        /// <returns>List info for list members added to.</returns>
        public static List AddMemberRangeToList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName, List<string> screenNames)
        {
            return AddMemberRangeToList(ctx, listID, slug, ownerID, ownerScreenName, screenNames, null);
        }

        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="screenNames">List of user screen names to be list members. (max 100)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list members added to.</returns>
        public static List AddMemberRangeToList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName, List<string> screenNames, Action<TwitterAsyncResponse<List>> callback)
        {
            if (screenNames == null || screenNames.Count == 0)
            {
                throw new ArgumentException("screenNames is required. Check to see if the argument is null or the List<string> is empty.", "screenNames");
            }

            if (screenNames != null && screenNames.Count > 100)
            {
                throw new ArgumentException("Max screenNames is 100 at a time.", "screenNames");
            }

            return AddMemberRangeToList(ctx, listID, slug, ownerID, ownerScreenName, null, screenNames, callback);
        }

        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="userIDs">List of user IDs to be list members. (max 100)</param>
        /// <returns>List info for list members added to.</returns>
        public static List AddMemberRangeToList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName, List<ulong> userIDs)
        {
            return AddMemberRangeToList(ctx, listID, slug, ownerID, ownerScreenName, userIDs, null);
        }

        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="userIDs">List of user IDs to be list members. (max 100)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list members added to.</returns>
        public static List AddMemberRangeToList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName, List<ulong> userIDs, Action<TwitterAsyncResponse<List>> callback)
        {
            if (userIDs == null || userIDs.Count == 0)
            {
                throw new ArgumentException("userIDs is required. Check to see if the argument is null or the List<ulong> is empty.", "userIDs");
            }

            if (userIDs != null && userIDs.Count > 100)
            {
                throw new ArgumentException("Max user IDs is 100 at a time.", "userIDs");
            }

            return AddMemberRangeToList(ctx, listID, slug, ownerID, ownerScreenName, userIDs, null, callback);
        }

        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="userIDs">List of user IDs to be list members. (max 100)</param>
        /// <param name="screenNames">List of user screen names to be list members. (max 100)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list members added to.</returns>
        static List AddMemberRangeToList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName, IEnumerable<ulong> userIDs, List<string> screenNames, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var addMemberRangeUrl = ctx.BaseUrl + "lists/members/create_all.json";

            var reqProc = new ListRequestProcessor<List>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    addMemberRangeUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                        { "user_id", userIDs == null ? null : string.Join(",", userIDs.Select(id => id.ToString(CultureInfo.InvariantCulture)).ToArray()) },                        
                        { "screen_name", screenNames == null ? null : string.Join(",", screenNames.ToArray()) }
                    },
                    reqProc);

            List results = reqProc.ProcessActionResult(resultsJson, ListAction.AddMember);
            return results;
        }

        /// <summary>
        /// Removes a user as a list member
        /// </summary>
        /// <param name="userID">ID of user to add to list.</param>
        /// <param name="screenName">ScreenName of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list member removed from</returns>
        public static List DeleteMemberFromList(this TwitterContext ctx, string userID, string screenName, string listID, string slug, string ownerID, string ownerScreenName)
        {
            return DeleteMemberFromList(ctx, userID, screenName, listID, slug, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Removes a user as a list member
        /// </summary>
        /// <param name="userID">ID of user to add to list.</param>
        /// <param name="screenName">ScreenName of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async callback</param>
        /// <returns>List info for list member removed from</returns>
        public static List DeleteMemberFromList(this TwitterContext ctx, string userID, string screenName, string listID, string slug, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(userID) && string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either userID or screenName is required.", UserIDOrScreenNameParam);
            }

            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var deleteUrl = ctx.BaseUrl + "lists/members/destroy.json";

            var reqProc = new ListRequestProcessor<List>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    deleteUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName },
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    },
                    reqProc);

            List results = reqProc.ProcessActionResult(resultsJson, ListAction.DeleteMember);
            return results;
        }

        /// <summary>
        /// Adds a user as a list subscriber
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscribed to</returns>
        public static List SubscribeToList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName)
        {
            return SubscribeToList(ctx, listID, slug, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Adds a user as a list subscriber
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list subscribed to</returns>
        public static List SubscribeToList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var subscribeUrl = ctx.BaseUrl + "lists/subscribers/create.json";

            var reqProc = new ListRequestProcessor<List>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    subscribeUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    },
                    reqProc);

            List results = reqProc.ProcessActionResult(resultsJson, ListAction.Subscribe);
            return results;
        }

        /// <summary>
        /// Removes a user as a list subscriber
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscription removed from</returns>
        public static List UnsubscribeFromList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName)
        {
            return UnsubscribeFromList(ctx, listID, slug, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Removes a user as a list subscriber
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list subscription removed from</returns>
        public static List UnsubscribeFromList(this TwitterContext ctx, string listID, string slug, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);
            }

            if (!string.IsNullOrEmpty(slug) && string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var unsubscribeUrl = ctx.BaseUrl + "lists/subscribers/destroy.json";

            var reqProc = new ListRequestProcessor<List>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    unsubscribeUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    },
                    reqProc);

            List results = reqProc.ProcessActionResult(resultsJson, ListAction.Unsubscribe);
            return results;
        }

        /// <summary>
        /// Deletes membership for a comma-separated list of users
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="userIds">Comma-separated list of user IDs of users to remove from list membership.</param>
        /// <param name="screenNames">Comma-separated list of screen names of users to remove from list membership.</param>
        /// <param name="ownerID">ID of users who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscription removed from</returns>
        public static List DestroyAllFromList(this TwitterContext ctx, string listID, string slug, string userIds, string screenNames, string ownerID, string ownerScreenName)
        {
            return DestroyAllFromList(ctx, listID, slug, userIds, screenNames, ownerID, ownerScreenName, null);
        }

        /// <summary>
        /// Deletes membership for a comma-separated list of users
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="userIds">Comma-separated list of user IDs of users to remove from list membership.</param>
        /// <param name="screenNames">Comma-separated list of screen names of users to remove from list membership.</param>
        /// <param name="ownerID">ID of users who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>List info for list subscription removed from</returns>
        public static List DestroyAllFromList(this TwitterContext ctx, string listID, string slug, string userIds, string screenNames, string ownerID, string ownerScreenName, Action<TwitterAsyncResponse<List>> callback)
        {
            if (string.IsNullOrEmpty(listID) && string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);
            }

            if (string.IsNullOrEmpty(listID) && !string.IsNullOrEmpty(slug) && 
                string.IsNullOrEmpty(ownerID) && string.IsNullOrEmpty(ownerScreenName))
            {
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);
            }

            var destroyAllUrl = ctx.BaseUrl + "lists/members/destroy_all.json";

            var reqProc = new ListRequestProcessor<List>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    destroyAllUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "user_id", userIds == null ? null : userIds.Replace(" ", "") },
                        { "screen_name", screenNames == null ? null : screenNames.Replace(" ", "") },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    },
                    reqProc);

            List results = reqProc.ProcessActionResult(resultsJson, ListAction.DestroyAll);
            return results;
        }
    }
}
