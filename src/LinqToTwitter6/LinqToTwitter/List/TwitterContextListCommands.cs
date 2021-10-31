using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
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
        /// <param name="name">name of list</param>
        /// <param name="description">list description</param>
        /// <param name="isPrivate">true or false</param>
        /// <returns>List info for new list - <see cref="ListResponse"/></returns>
        public async Task<ListResponse?> CreateListAsync(string name, string description, bool isPrivate, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = name ?? throw new ArgumentException($"{nameof(name)} is required.", nameof(name));

            var url = $"{BaseUrl2}lists";

            var postData = new Dictionary<string, string>();
            var postObj = new ListCreateOrUpdateRequest 
            {
                Name = name,
                Description = description,
                Private = isPrivate,
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            ListResponse? result = JsonSerializer.Deserialize<ListResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Modifies an existing list.
        /// </summary>
        /// <param name="id">ID of list</param>
        /// <param name="name">name of list</param>
        /// <param name="description">list description</param>
        /// <param name="isPrivate">true or false</param>
        /// <returns><see cref="ListResponseData.Updated"/> true or false</returns>
        public async Task<ListResponse?> UpdateListAsync(string id, string name, string description, bool isPrivate, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = id ?? throw new ArgumentException($"{nameof(id)} is required.", nameof(id));

            var url = $"{BaseUrl2}lists/{id}";

            var postData = new Dictionary<string, string>();
            var postObj = new ListCreateOrUpdateRequest
            {
                Name = name,
                Description = description,
                Private = isPrivate,
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Put.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            ListResponse? result = JsonSerializer.Deserialize<ListResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Deletes an existing list.
        /// </summary>
        /// <param name="id">ID of list</param>
        /// <returns><see cref="ListResponseData.Deleted"/> true or false</returns>
        public async Task<ListResponse?> DeleteListAsync(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = id ?? throw new ArgumentException($"{nameof(id)} is required.", nameof(id));

            var url = $"{BaseUrl2}lists/{id}";

            var postData = new Dictionary<string, string>();
            var postObj = new ListDeleteRequest
            {
                ID = id
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            ListResponse? result = JsonSerializer.Deserialize<ListResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Adds a user as a list member.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="userID">ID of user to add to list.</param>
        /// <returns><see cref="ListResponse.Data"/> confirms add</returns>
        public async Task<ListResponse?> AddMemberToListAsync(string listID, string userID, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = listID ?? throw new ArgumentException($"{nameof(listID)} is required.", nameof(listID));
            _ = userID ?? throw new ArgumentException($"{nameof(userID)} is required.", nameof(userID));

            var url = $"{BaseUrl2}lists/{listID}/members";

            var postData = new Dictionary<string, string>();
            var postObj = new ListMemberRequest
            {
                UserID = userID
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            ListResponse? result = JsonSerializer.Deserialize<ListResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Removes a user as a list member.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="userID">ID of user to remove from list.</param>
        /// <returns><see cref="ListResponse.Data"/> confirms delete</returns>
        public async Task<ListResponse?> DeleteMemberFromListAsync(string listID, string userID, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = listID ?? throw new ArgumentException($"{nameof(listID)} is required.", nameof(listID));
            _ = userID ?? throw new ArgumentException($"{nameof(userID)} is required.", nameof(userID));

            var url = $"{BaseUrl2}lists/{listID}/members/{userID}";

            var postData = new Dictionary<string, string>();
            var postObj = new ListMemberRequest
            {
                UserID = userID
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            ListResponse? result = JsonSerializer.Deserialize<ListResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Adds a user as a list follower.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="userID">ID of user to follow list.</param>
        /// <returns><see cref="ListResponse.Data"/> confirms add</returns>
        public async Task<ListResponse?> AddFollowerToListAsync(string listID, string userID, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = listID ?? throw new ArgumentException($"{nameof(listID)} is required.", nameof(listID));
            _ = userID ?? throw new ArgumentException($"{nameof(userID)} is required.", nameof(userID));

            var url = $"{BaseUrl2}users/{userID}/followed_lists";

            var postData = new Dictionary<string, string>();
            var postObj = new ListFollowOrPinRequest
            {
                ListID = listID
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            ListResponse? result = JsonSerializer.Deserialize<ListResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Removes a user as a list follower.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="userID">ID of user to unfollow from list.</param>
        /// <returns><see cref="ListResponse.Data"/> confirms delete</returns>
        public async Task<ListResponse?> DeleteFollowerFromListAsync(string listID, string userID, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = listID ?? throw new ArgumentException($"{nameof(listID)} is required.", nameof(listID));
            _ = userID ?? throw new ArgumentException($"{nameof(userID)} is required.", nameof(userID));

            var url = $"{BaseUrl2}users/{userID}/followed_lists/{listID}";

            var postData = new Dictionary<string, string>();
            var postObj = new ListFollowOrPinRequest
            {
                ListID = listID
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            ListResponse? result = JsonSerializer.Deserialize<ListResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Pins a list for a user.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="userID">ID of user to follow list.</param>
        /// <returns><see cref="ListResponse.Data"/> confirms add</returns>
        public async Task<ListResponse?> PinListAsync(string listID, string userID, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = listID ?? throw new ArgumentException($"{nameof(listID)} is required.", nameof(listID));
            _ = userID ?? throw new ArgumentException($"{nameof(userID)} is required.", nameof(userID));

            var url = $"{BaseUrl2}users/{userID}/pinned_lists";

            var postData = new Dictionary<string, string>();
            var postObj = new ListFollowOrPinRequest
            {
                ListID = listID
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            ListResponse? result = JsonSerializer.Deserialize<ListResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Unpins a list for a user.
        /// </summary>
        /// <param name="listID">ID of list.</param>
        /// <param name="userID">ID of user to unfollow from list.</param>
        /// <returns><see cref="ListResponse.Data"/> confirms delete</returns>
        public async Task<ListResponse?> UnpinListAsync(string listID, string userID, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = listID ?? throw new ArgumentException($"{nameof(listID)} is required.", nameof(listID));
            _ = userID ?? throw new ArgumentException($"{nameof(userID)} is required.", nameof(userID));

            var url = $"{BaseUrl2}users/{userID}/pinned_lists/{listID}";

            var postData = new Dictionary<string, string>();
            var postObj = new ListFollowOrPinRequest
            {
                ListID = listID
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            ListResponse? result = JsonSerializer.Deserialize<ListResponse>(RawResult);

            return result;
        }
    }
}
