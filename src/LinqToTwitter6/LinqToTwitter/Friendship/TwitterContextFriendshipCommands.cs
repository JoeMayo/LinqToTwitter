using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Lets logged-in user follow another user.
        /// </summary>
        /// <param name="userID">ID of user to follow</param>
        /// <param name="follow">Receive notifications for the followed friend</param>
        /// <returns>followed friend user info</returns>
        public async Task<User?> CreateFriendshipAsync(ulong userID, bool follow, CancellationToken cancelToken = default(CancellationToken))
        {
            if (userID == 0)
                throw new ArgumentException("userID is a required parameter.", "userID");

            string destroyUrl = BaseUrl + "friendships/create.json";

            var createParams = new Dictionary<string, string>
                {
                    { "user_id", userID.ToString() }
                };

            // If follow exists in the parameter list, Twitter will
            // always treat it as true, even if the value is false;
            // Therefore, only add follow if it is true.
            if (follow)
                createParams.Add("follow", "true");

            var reqProc = new FriendshipRequestProcessor<User>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<User>(
                    HttpMethod.Post.ToString(),
                    destroyUrl,
                    createParams,
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, FriendshipAction.Create);
        }

        /// <summary>
        /// Lets logged-in user follow another user.
        /// </summary>
        /// <param name="screenName">Screen name of user to follow</param>
        /// <param name="follow">Receive notifications for the followed friend</param>
        /// <returns>followed friend user info</returns>
        public async Task<User?> CreateFriendshipAsync(string screenName, bool follow, CancellationToken cancelToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("screenName is a required parameter.", "screenName");

            string destroyUrl = BaseUrl + "friendships/create.json";

            var createParams = new Dictionary<string, string>
                {
                    { "screen_name", screenName }
                };

            // If follow exists in the parameter list, Twitter will
            // always treat it as true, even if the value is false;
            // Therefore, only add follow if it is true.
            if (follow)
                createParams.Add("follow", "true");

            var reqProc = new FriendshipRequestProcessor<User>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<User>(
                    HttpMethod.Post.ToString(),
                    destroyUrl,
                    createParams,
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, FriendshipAction.Create);
        }

        /// <summary>
        /// Lets logged-in user un-follow another user.
        /// </summary>
        /// <param name="userID">ID of user to unfollow</param>
        /// <returns>followed friend user info</returns>
        public async Task<User?> DestroyFriendshipAsync(ulong userID, CancellationToken cancelToken = default(CancellationToken))
        {
            if (userID == 0)
                throw new ArgumentException("userID is a required parameter.", "userID");

            string destroyUrl = BaseUrl + "friendships/destroy.json";

            var reqProc = new FriendshipRequestProcessor<User>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<User>(
                    HttpMethod.Post.ToString(),
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID.ToString() }
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, FriendshipAction.Destroy);
        }

        /// <summary>
        /// Lets logged-in user un-follow another user.
        /// </summary>
        /// <param name="screenName">Screen name of user to unfollow</param>
        /// <returns>followed friend user info</returns>
        public async Task<User?> DestroyFriendshipAsync(string screenName, CancellationToken cancelToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("screenName is a required parameter.", "screenName");

            string destroyUrl = BaseUrl + "friendships/destroy.json";

            var reqProc = new FriendshipRequestProcessor<User>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<User>(
                    HttpMethod.Post.ToString(),
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        { "screen_name", screenName }
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, FriendshipAction.Destroy);
        }

        /// <summary>
        /// Lets logged-in user set retweets and/or device notifications for a follower.
        /// </summary>
        /// <param name="userID">Twitter's ID for user</param>
        /// <param name="screenName">screen name of user to update</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <returns>updated friend user info</returns>
        public async Task<Friendship?> UpdateFriendshipSettingsAsync(string screenName, bool retweets, bool device, CancellationToken cancelToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentNullException("screenName", "screenName is a required parameter.");

            return await UpdateFriendshipSettingsAsync(0, screenName, retweets, device, cancelToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Lets logged-in user set retweets and/or device notifications for a follower.
        /// </summary>
        /// <param name="userID">Twitter's ID for user</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <returns>updated friend user info</returns>
        public async Task<Friendship?> UpdateFriendshipSettingsAsync(ulong userID, bool retweets, bool device, CancellationToken cancelToken = default(CancellationToken))
        {
            if (userID == 0)
                throw new ArgumentNullException("userID", "userID is a required parameter.");

            return await UpdateFriendshipSettingsAsync(0, null, retweets, device, cancelToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Lets logged-in user set retweets and/or device notifications for a follower.
        /// </summary>
        /// <param name="userID">Twitter's ID for user</param>
        /// <param name="screenName">screen name of user to update</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <returns>updated friend user info</returns>
        async Task<Friendship?> UpdateFriendshipSettingsAsync(ulong userID, string? screenName, bool retweets, bool device, CancellationToken cancelToken = default(CancellationToken))
        {
            var parms = new Dictionary<string, string>
            {
                { "retweets", retweets.ToString().ToLower() },
                { "device", device.ToString().ToLower() }
            };

            if (screenName != null) parms.Add("screen_name", screenName);
            if (userID > 0) parms.Add("user_id", userID.ToString());

            string updateUrl = BaseUrl + "friendships/update.json";

            var reqProc = new FriendshipRequestProcessor<Friendship>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<Friendship>(
                    HttpMethod.Post.ToString(),
                    updateUrl,
                    parms,
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, FriendshipAction.Update);
        }
    }
}
