using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
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
        public async Task<List> CreateListAsync(string listName, string mode, string description)
        {
            if (string.IsNullOrWhiteSpace(listName))
                throw new ArgumentException("listName is required.", "listName");

            var createUrl = BaseUrl + "lists/create.json";

            var reqProc = new ListRequestProcessor<List>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<List>(
                    createUrl,
                    new Dictionary<string, string>
                    {
                        { "name", listName },
                        { "mode", mode },
                        { "description", description }
                    });

            return reqProc.ProcessActionResult(resultsJson, ListAction.Create);
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
        /// <returns>List info for modified list</returns>
        public async Task<List> UpdateListAsync(string listID, string slug, string name, string ownerID, string ownerScreenName, string mode, string description)
        {
            if (string.IsNullOrWhiteSpace(listID) && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && string.IsNullOrWhiteSpace(ownerID) && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var updateListUrl = BaseUrl + "lists/update.json";

            var reqProc = new ListRequestProcessor<List>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<List>(
                    updateListUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                        { "mode", mode },
                        { "description", description },
                        { "name", name }
                    });

            return reqProc.ProcessActionResult(resultsJson, ListAction.Update);
        }

        /// <summary>
        /// Deletes an existing list
        /// </summary>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for deleted list</returns>
        public async Task<List> DeleteListAsync(string listID, string slug, string ownerID, string ownerScreenName)
        {
            if (string.IsNullOrWhiteSpace(listID) && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("listID is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && string.IsNullOrWhiteSpace(ownerID) && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var deleteUrl = BaseUrl + "lists/destroy.json";

            var reqProc = new ListRequestProcessor<List>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<List>(
                    deleteUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName }
                    });

            return reqProc.ProcessActionResult(resultsJson, ListAction.Delete);
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
        /// <returns>List info for list member added to</returns>
        public async Task<List> AddMemberToListAsync(string userID, string screenName, string listID, string slug, string ownerID, string ownerScreenName)
        {
            if (string.IsNullOrWhiteSpace(userID) && string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("Either userID or screenName is required.", UserIDOrScreenNameParam);

            if (string.IsNullOrWhiteSpace(listID) && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && string.IsNullOrWhiteSpace(ownerID) && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var addMemberUrl = BaseUrl + "lists/members/create.json";

            var reqProc = new ListRequestProcessor<List>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<List>(
                    addMemberUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName },
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    });

            return reqProc.ProcessActionResult(resultsJson, ListAction.AddMember);
        }

        /// <summary>
        /// Adds a list of users to a list.
        /// </summary>
        /// <param name="listID">ID of List.</param>
        /// <param name="slug">List name.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="screenNames">List of user screen names to be list members. (max 100)</param>
        /// <returns>List info for list members added to.</returns>
        public async Task<List> AddMemberRangeToListAsync(string listID, string slug, string ownerID, string ownerScreenName, List<string> screenNames)
        {
            if (screenNames == null || screenNames.Count == 0)
                throw new ArgumentException("screenNames is required. Check to see if the argument is null or the List<string> is empty.", "screenNames");

            if (screenNames != null && screenNames.Count > 100)
                throw new ArgumentException("Max screenNames is 100 at a time.", "screenNames");

            return await AddMemberRangeToListAsync(listID, slug, ownerID, ownerScreenName, null, screenNames);
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
        public async Task<List> AddMemberRangeToListAsync(string listID, string slug, string ownerID, string ownerScreenName, List<ulong> userIDs)
        {
            if (userIDs == null || userIDs.Count == 0)
                throw new ArgumentException("userIDs is required. Check to see if the argument is null or the List<ulong> is empty.", "userIDs");

            if (userIDs != null && userIDs.Count > 100)
                throw new ArgumentException("Max user IDs is 100 at a time.", "userIDs");

            return await AddMemberRangeToListAsync(listID, slug, ownerID, ownerScreenName, userIDs);
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
        /// <returns>List info for list members added to.</returns>
        async Task<List> AddMemberRangeToListAsync(string listID, string slug, string ownerID, string ownerScreenName, IEnumerable<ulong> userIDs, List<string> screenNames)
        {
            if (string.IsNullOrWhiteSpace(listID) && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && string.IsNullOrWhiteSpace(ownerID) && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var addMemberRangeUrl = BaseUrl + "lists/members/create_all.json";

            var reqProc = new ListRequestProcessor<List>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<List>(
                    addMemberRangeUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                        { "user_id", userIDs == null ? null : string.Join(",", userIDs.Select(id => id.ToString(CultureInfo.InvariantCulture)).ToArray()) },                        
                        { "screen_name", screenNames == null ? null : string.Join(",", screenNames.ToArray()) }
                    });

            return reqProc.ProcessActionResult(resultsJson, ListAction.AddMember);
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
        public async Task<List> DeleteMemberFromListAsync(string userID, string screenName, string listID, string slug, string ownerID, string ownerScreenName)
        {
            if (string.IsNullOrWhiteSpace(userID) && string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("Either userID or screenName is required.", UserIDOrScreenNameParam);

            if (string.IsNullOrWhiteSpace(listID) && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && string.IsNullOrWhiteSpace(ownerID) && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var deleteUrl = BaseUrl + "lists/members/destroy.json";

            var reqProc = new ListRequestProcessor<List>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<List>(
                    deleteUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName },
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    });

            return reqProc.ProcessActionResult(resultsJson, ListAction.DeleteMember);
        }

        /// <summary>
        /// Adds a user as a list subscriber
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscribed to</returns>
        public async Task<List> SubscribeToListAsync(string listID, string slug, string ownerID, string ownerScreenName)
        {
            if (string.IsNullOrWhiteSpace(listID) && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && string.IsNullOrWhiteSpace(ownerID) && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var subscribeUrl = BaseUrl + "lists/subscribers/create.json";

            var reqProc = new ListRequestProcessor<List>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<List>(
                    subscribeUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    });

            return reqProc.ProcessActionResult(resultsJson, ListAction.Subscribe);
        }

        /// <summary>
        /// Removes a user as a list subscriber
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscription removed from</returns>
        public async Task<List> UnsubscribeFromListAsync(string listID, string slug, string ownerID, string ownerScreenName)
        {
            if (string.IsNullOrWhiteSpace(listID) && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && string.IsNullOrWhiteSpace(ownerID) && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var unsubscribeUrl = BaseUrl + "lists/subscribers/destroy.json";

            var reqProc = new ListRequestProcessor<List>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<List>(
                    unsubscribeUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    });

            return reqProc.ProcessActionResult(resultsJson, ListAction.Unsubscribe);
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
        public async Task<List> DestroyAllFromListAsync(string listID, string slug, string userIds, string screenNames, string ownerID, string ownerScreenName)
        {
            if (string.IsNullOrWhiteSpace(listID) && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (string.IsNullOrWhiteSpace(listID) && !string.IsNullOrWhiteSpace(slug) && 
                string.IsNullOrWhiteSpace(ownerID) && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var destroyAllUrl = BaseUrl + "lists/members/destroy_all.json";

            var reqProc = new ListRequestProcessor<List>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<List>(
                    destroyAllUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID },
                        { "slug", slug },
                        { "user_id", userIds == null ? null : userIds.Replace(" ", "") },
                        { "screen_name", screenNames == null ? null : screenNames.Replace(" ", "") },
                        { "owner_id", ownerID },
                        { "owner_screen_name", ownerScreenName },
                    });

            return reqProc.ProcessActionResult(resultsJson, ListAction.DestroyAll);
        }
    }
}
