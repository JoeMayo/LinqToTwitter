using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="userID">ID of user to follow</param>
        /// <param name="screenName">Screen name of user to follow</param>
        /// <param name="follow">Receive notifications for the followed friend</param>
        /// <returns>followed friend user info</returns>
        public async Task<User> CreateFriendshipAsync(ulong userID, string screenName, bool follow)
        {
            if (userID == 0 && string.IsNullOrEmpty(screenName))
                throw new ArgumentException("Either userID or screenName is a required parameter.", "UserIDOrScreenName");

            string destroyUrl = BaseUrl + "friendships/create.json";

            var createParams = new Dictionary<string, string>
                {
                    { "user_id", userID.ToString() },
                    { "screen_name", screenName }
                };

            // If follow exists in the parameter list, Twitter will
            // always treat it as true, even if the value is false;
            // Therefore, only add follow if it is true.
            if (follow)
            {
                createParams.Add("follow", "true");
            }

            var reqProc = new FriendshipRequestProcessor<User>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<User>(
                    destroyUrl,
                    createParams);

            return reqProc.ProcessActionResult(resultsJson, FriendshipAction.Create);
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="userID">ID of user to unfollow</param>
        /// <param name="screenName">Screen name of user to unfollow</param>
        /// <returns>followed friend user info</returns>
        public async Task<User> DestroyFriendshipAsync(string userID, string screenName)
        {
            if (string.IsNullOrEmpty(userID) && string.IsNullOrEmpty(screenName))
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.", "UserIDOrScreenName");

            string destroyUrl = BaseUrl + "friendships/destroy.json";

            var reqProc = new FriendshipRequestProcessor<User>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<User>(
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName }
                    });

            return reqProc.ProcessActionResult(resultsJson, FriendshipAction.Destroy);
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="userID">Twitter's ID for user</param>
        /// <param name="screenName">screen name of user to update</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <returns>updated friend user info</returns>
        public async Task<Friendship> UpdateFriendshipSettingsAsync(string screenName, bool retweets, bool device)
        {
            return await UpdateFriendshipSettingsAsync(0, screenName, retweets, device);
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="userID">Twitter's ID for user</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <returns>updated friend user info</returns>
        public async Task<Friendship> UpdateFriendshipSettingsAsync(ulong userID, bool retweets, bool device)
        {
            return await UpdateFriendshipSettingsAsync(0, null, retweets, device);
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="userID">Twitter's ID for user</param>
        /// <param name="screenName">screen name of user to update</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <returns>updated friend user info</returns>
        public async Task<Friendship> UpdateFriendshipSettingsAsync(ulong userID, string screenName, bool retweets, bool device)
        {
            if (string.IsNullOrEmpty(screenName) && userID == 0)
                throw new ArgumentNullException("screenNameOrUserID", "Either screenName or UserID is a required parameter.");

            var parms = new Dictionary<string, string>
            {
                { "retweets", retweets.ToString().ToLower() },
                { "device", device.ToString().ToLower() }
            };

            if (screenName != null) parms.Add("screen_name", screenName);
            if (userID > 0) parms.Add("user_id", userID.ToString());

            string updateUrl = BaseUrl + "friendships/update.json";

            var reqProc = new FriendshipRequestProcessor<Friendship>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<Friendship>(
                    updateUrl,
                    parms);

            return reqProc.ProcessActionResult(resultsJson, FriendshipAction.Update);
        }
    }
}
