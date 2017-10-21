using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        public const string ListIDOrSlugParam = "ListIdOrSlug";
        public const string OwnerIDOrOwnerScreenNameParam = "OwnerIdOrOwnerScreenName";
        public const string UserIDOrScreenNameParam = "UserIdOrScreenName";

        /// <summary>
        /// Creates a new list.
        /// </summary>
        /// <param name="listName">name of list</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <returns>List info for new list</returns>
        public async Task<List> CreateListAsync(string listName, string mode, string description, CancellationToken cancelToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(listName))
                throw new ArgumentException("listName is required.", "listName");

            var createUrl = BaseUrl + "lists/create.json";

            var reqProc = new ListRequestProcessor<List>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<List>(
                    HttpMethod.Post.ToString(),
                    createUrl,
                    new Dictionary<string, string>
                    {
                        { "name", listName },
                        { "mode", mode },
                        { "description", description }
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, ListAction.Create);
        }

        /// <summary>
        /// Modifies an existing list.
        /// </summary>
        /// <param name="listID">ID of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <param name="mode">public or private</param>
        /// <param name="description">list description</param>
        /// <returns>List info for modified list</returns>
        public async Task<List> UpdateListAsync(ulong listID, string slug, string name, ulong ownerID, string ownerScreenName, string mode, string description, CancellationToken cancelToken = default(CancellationToken))
        {
            if (listID == 0 && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && ownerID == 0 && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var updateListUrl = BaseUrl + "lists/update.json";

            var reqProc = new ListRequestProcessor<List>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<List>(
                    HttpMethod.Post.ToString(),
                    updateListUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID.ToString() },
                        { "slug", slug },
                        { "owner_id", ownerID.ToString() },
                        { "owner_screen_name", ownerScreenName },
                        { "mode", mode },
                        { "description", description },
                        { "name", name }
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, ListAction.Update);
        }

        /// <summary>
        /// Deletes an existing list.
        /// </summary>
        /// <param name="listID">ID or slug of list</param>
        /// <param name="slug">name of list</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for deleted list</returns>
        public async Task<List> DeleteListAsync(ulong listID, string slug, ulong ownerID, string ownerScreenName, CancellationToken cancelToken = default(CancellationToken))
        {
            if (listID == 0 && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("listID is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && ownerID == 0 && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If you specify a Slug, you must also specify either OwnerID or OwnerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var deleteUrl = BaseUrl + "lists/destroy.json";

            var reqProc = new ListRequestProcessor<List>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<List>(
                    HttpMethod.Post.ToString(),
                    deleteUrl,
                    new Dictionary<string, string>
                    {
                        { "list_id", listID.ToString() },
                        { "slug", slug },
                        { "owner_id", ownerID.ToString() },
                        { "owner_screen_name", ownerScreenName }
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, ListAction.Delete);
        }
        
        /// <summary>
        /// Adds a user as a list member.
        /// </summary>
        /// <param name="userID">ID of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list member added to</returns>
        public async Task<List> AddMemberToListAsync(ulong userID, ulong listID, string slug, ulong ownerID, string ownerScreenName, CancellationToken cancelToken = default(CancellationToken))
        {
            return await AddMemberToListAsync(userID, null, listID, slug, ownerID, ownerScreenName, cancelToken).ConfigureAwait(false);
        }
        
        /// <summary>
        /// Adds a user as a list member.
        /// </summary>
        /// <param name="screenName">ScreenName of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list member added to</returns>
        public async Task<List> AddMemberToListAsync(string screenName, ulong listID, string slug, ulong ownerID, string ownerScreenName, CancellationToken cancelToken = default(CancellationToken))
        {
            return await AddMemberToListAsync(0, screenName, listID, slug, ownerID, ownerScreenName, cancelToken).ConfigureAwait(false);
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
        /// <returns>List info for list member added to</returns>
        async Task<List> AddMemberToListAsync(ulong userID, string screenName, ulong listID, string slug, ulong ownerID, string ownerScreenName, CancellationToken cancelToken = default(CancellationToken))
        {
            if (userID == 0 && string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("Either userID or screenName is required.", UserIDOrScreenNameParam);

            if (listID == 0 && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && ownerID == 0 && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var addMemberUrl = BaseUrl + "lists/members/create.json";

            var parameters = new Dictionary<string, string>();

            if (listID != 0)
                parameters.Add("list_id", listID.ToString());
            if (!string.IsNullOrWhiteSpace(slug))
                parameters.Add("slug", slug);
            if (userID != 0)
                parameters.Add("user_id", userID.ToString());
            if (!string.IsNullOrWhiteSpace(screenName))
                parameters.Add("screen_name", screenName);
            if (ownerID != 0)
                parameters.Add("owner_id", ownerID.ToString());
            if (!string.IsNullOrWhiteSpace(ownerScreenName))
                parameters.Add("owner_screen_name", ownerScreenName);

            var reqProc = new ListRequestProcessor<List>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<List>(HttpMethod.Post.ToString(), addMemberUrl, parameters, cancelToken).ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, ListAction.AddMember);
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
        public async Task<List> AddMemberRangeToListAsync(ulong listID, string slug, ulong ownerID, string ownerScreenName, List<string> screenNames, CancellationToken cancelToken = default(CancellationToken))
        {
            if (screenNames == null || screenNames.Count == 0)
                throw new ArgumentException("screenNames is required. Check to see if the argument is null or the List<string> is empty.", "screenNames");

            if (screenNames != null && screenNames.Count > 100)
                throw new ArgumentException("Max screenNames is 100 at a time.", "screenNames");

            return await AddMemberRangeToListAsync(listID, slug, ownerID, ownerScreenName, userIDs: null, screenNames: screenNames, cancelToken: cancelToken).ConfigureAwait(false);
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
        public async Task<List> AddMemberRangeToListAsync(ulong listID, string slug, ulong ownerID, string ownerScreenName, List<ulong> userIDs, CancellationToken cancelToken = default(CancellationToken))
        {
            if (userIDs == null || userIDs.Count == 0)
                throw new ArgumentException("userIDs is required. Check to see if the argument is null or the List<ulong> is empty.", "userIDs");

            if (userIDs != null && userIDs.Count > 100)
                throw new ArgumentException("Max user IDs is 100 at a time.", "userIDs");

            return await AddMemberRangeToListAsync(listID, slug, ownerID, ownerScreenName, userIDs, screenNames: null, cancelToken: cancelToken).ConfigureAwait(false);
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
        async Task<List> AddMemberRangeToListAsync(ulong listID, string slug, ulong ownerID, string ownerScreenName, IEnumerable<ulong> userIDs, List<string> screenNames, CancellationToken cancelToken = default(CancellationToken))
        {
            if (listID == 0 && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && ownerID == 0 && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var addMemberRangeUrl = BaseUrl + "lists/members/create_all.json";

            var reqProc = new ListRequestProcessor<List>();

            var parameters = new Dictionary<string, string>();

            if (listID != 0)
                parameters.Add("list_id", listID.ToString());
            if (!string.IsNullOrWhiteSpace(slug))
                parameters.Add("slug", slug);
            if (userIDs != null && userIDs.Any())
                parameters.Add("user_id", string.Join(",", userIDs.Select(id => id.ToString(CultureInfo.InvariantCulture)).ToArray()));
            if (screenNames != null && screenNames.Any())
                parameters.Add("screen_name", string.Join(",", screenNames));
            if (ownerID != 0)
                parameters.Add("owner_id", ownerID.ToString());
            if (!string.IsNullOrWhiteSpace(ownerScreenName))
                parameters.Add("owner_screen_name", ownerScreenName);

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<List>(HttpMethod.Post.ToString(), addMemberRangeUrl, parameters, cancelToken).ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, ListAction.AddMember);
        }

        /// <summary>
        /// Removes a user as a list member.
        /// </summary>
        /// <param name="userID">ID of user to add to list.</param>
        /// <param name="screenName">ScreenName of user to add to list.</param>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list member removed from</returns>
        public async Task<List> DeleteMemberFromListAsync(ulong userID, string screenName, ulong listID, string slug, ulong ownerID, string ownerScreenName, CancellationToken cancelToken = default(CancellationToken))
        {
            if (userID == 0 && string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("Either userID or screenName is required.", UserIDOrScreenNameParam);

            if (listID == 0 && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && ownerID == 0 && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var deleteUrl = BaseUrl + "lists/members/destroy.json";

            var reqProc = new ListRequestProcessor<List>();

            var parameters = new Dictionary<string, string>();

            if (listID != 0)
                parameters.Add("list_id", listID.ToString());
            if (!string.IsNullOrWhiteSpace(slug))
                parameters.Add("slug", slug);
            if (userID != 0)
                parameters.Add("user_id", userID.ToString());
            if (!string.IsNullOrWhiteSpace(screenName))
                parameters.Add("screen_name", screenName);
            if (ownerID != 0)
                parameters.Add("owner_id", ownerID.ToString());
            if (!string.IsNullOrWhiteSpace(ownerScreenName))
                parameters.Add("owner_screen_name", ownerScreenName);

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<List>(HttpMethod.Post.ToString(), deleteUrl, parameters, cancelToken).ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, ListAction.DeleteMember);
        }

        /// <summary>
        /// Adds a user as a list subscriber.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to add to.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscribed to</returns>
        public async Task<List> SubscribeToListAsync(ulong listID, string slug, ulong ownerID, string ownerScreenName, CancellationToken cancelToken = default(CancellationToken))
        {
            if (listID == 0 && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && ownerID == 0 && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var subscribeUrl = BaseUrl + "lists/subscribers/create.json";

            var reqProc = new ListRequestProcessor<List>();

            var parameters = new Dictionary<string, string>();

            if (listID != 0)
                parameters.Add("list_id", listID.ToString());
            if (!string.IsNullOrWhiteSpace(slug))
                parameters.Add("slug", slug);
            if (ownerID != 0)
                parameters.Add("owner_id", ownerID.ToString());
            if (!string.IsNullOrWhiteSpace(ownerScreenName))
                parameters.Add("owner_screen_name", ownerScreenName);

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<List>(HttpMethod.Post.ToString(), subscribeUrl, parameters, cancelToken).ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, ListAction.Subscribe);
        }

        /// <summary>
        /// Removes a user as a list subscriber.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="ownerID">ID of user who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscription removed from</returns>
        public async Task<List> UnsubscribeFromListAsync(ulong listID, string slug, ulong ownerID, string ownerScreenName, CancellationToken cancelToken = default(CancellationToken))
        {
            if (listID == 0 && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (!string.IsNullOrWhiteSpace(slug) && ownerID == 0 && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            var unsubscribeUrl = BaseUrl + "lists/subscribers/destroy.json";

            var reqProc = new ListRequestProcessor<List>();

            var parameters = new Dictionary<string, string>();

            if (listID != 0)
                parameters.Add("list_id", listID.ToString());
            if (!string.IsNullOrWhiteSpace(slug))
                parameters.Add("slug", slug);
            if (ownerID != 0)
                parameters.Add("owner_id", ownerID.ToString());
            if (!string.IsNullOrWhiteSpace(ownerScreenName))
                parameters.Add("owner_screen_name", ownerScreenName);

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<List>(HttpMethod.Post.ToString(), unsubscribeUrl, parameters, cancelToken).ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, ListAction.Unsubscribe);
        }
                
        /// <summary>
        /// Deletes membership for a comma-separated list of users.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="userIds">List of user IDs of users to remove from list membership.</param>
        /// <param name="ownerID">ID of users who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscription removed from</returns>
        public async Task<List> DeleteMemberRangeFromListAsync(ulong listID, string slug, List<ulong> userIDs, ulong ownerID, string ownerScreenName, CancellationToken cancelToken = default(CancellationToken))
        {
            return await DeleteMemberRangeFromListAsync(listID, slug, userIDs, null, ownerID, ownerScreenName, cancelToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes membership for a comma-separated list of users.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="screenNames">List of screen names of users to remove from list membership.</param>
        /// <param name="ownerID">ID of users who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscription removed from</returns>
        public async Task<List> DeleteMemberRangeFromListAsync(ulong listID, string slug, List<string> screenNames, ulong ownerID, string ownerScreenName, CancellationToken cancelToken = default(CancellationToken))
        {
            return await DeleteMemberRangeFromListAsync(listID, slug, null, screenNames, ownerID, ownerScreenName, cancelToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes membership for a comma-separated list of users.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="slug">Name of list to remove from.</param>
        /// <param name="userIds">List of user IDs of users to remove from list membership.</param>
        /// <param name="screenNames">List of screen names of users to remove from list membership.</param>
        /// <param name="ownerID">ID of users who owns the list.</param>
        /// <param name="ownerScreenName">Screen name of user who owns the list.</param>
        /// <returns>List info for list subscription removed from</returns>
        async Task<List> DeleteMemberRangeFromListAsync(ulong listID, string slug, List<ulong> userIDs, List<string> screenNames, ulong ownerID, string ownerScreenName, CancellationToken cancelToken = default(CancellationToken))
        {
            if (listID == 0 && string.IsNullOrWhiteSpace(slug))
                throw new ArgumentException("Either listID or slug is required.", ListIDOrSlugParam);

            if (listID == 0 && !string.IsNullOrWhiteSpace(slug) && 
                ownerID == 0 && string.IsNullOrWhiteSpace(ownerScreenName))
                throw new ArgumentException("If using slug, you must also provide either ownerID or ownerScreenName.", OwnerIDOrOwnerScreenNameParam);

            if ((userIDs != null && userIDs.Count > 100) || 
                (screenNames != null && screenNames.Count > 100))
                throw new ArgumentException("You can only remove 100 members at a Time.", "userIDs");

            var destroyAllUrl = BaseUrl + "lists/members/destroy_all.json";

            var reqProc = new ListRequestProcessor<List>();

            var parameters = new Dictionary<string, string>();

            if (listID != 0)
                parameters.Add("list_id", listID.ToString());
            if (!string.IsNullOrWhiteSpace(slug))
                parameters.Add("slug", slug);
            if (userIDs != null && userIDs.Any())
                parameters.Add("user_id", string.Join(",", userIDs.Select(id => id.ToString(CultureInfo.InvariantCulture)).ToArray()));
            if (screenNames != null && screenNames.Any())
                parameters.Add("screen_name", string.Join(",", screenNames));
            if (ownerID != 0)
                parameters.Add("owner_id", ownerID.ToString());
            if (!string.IsNullOrWhiteSpace(ownerScreenName))
                parameters.Add("owner_screen_name", ownerScreenName);

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<List>(HttpMethod.Post.ToString(), destroyAllUrl, parameters, cancelToken).ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, ListAction.DestroyAll);
        }
    }
}
