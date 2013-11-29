using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    public static class FriendshipExtensions
    {
        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="userID">Numeric ID of user to follow</param>
        /// <param name="screenName">Screen name of user to follow</param>
        /// <param name="follow">Receive notifications for the followed friend</param>
        /// <returns>followed friend user info</returns>
        public static User CreateFriendship(this TwitterContext ctx, string userID, string screenName, bool follow)
        {
            return CreateFriendship(ctx, userID, screenName, follow, null);
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="userID">Numeric ID of user to follow</param>
        /// <param name="screenName">Screen name of user to follow</param>
        /// <param name="follow">Receive notifications for the followed friend</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>followed friend user info</returns>
        public static User CreateFriendship(this TwitterContext ctx, string userID, string screenName, bool follow, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either userID or screenName is a required parameter.", "UserIDOrScreenName");
            }

            string destroyUrl = ctx.BaseUrl + "friendships/create.json";

            var createParams = new Dictionary<string, string>
                {
                    { "user_id", userID },
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

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.PostToTwitter(
                    destroyUrl,
                    createParams,
                    response => reqProc.ProcessActionResult(response, FriendshipAction.Create));

            User results = reqProc.ProcessActionResult(resultsJson, FriendshipAction.Create);
            return results;
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="userID">Numeric ID of user to unfollow</param>
        /// <param name="screenName">Screen name of user to unfollow</param>
        /// <returns>followed friend user info</returns>
        public static User DestroyFriendship(this TwitterContext ctx, string userID, string screenName)
        {
            return DestroyFriendship(ctx, userID, screenName, null);
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="userID">Numeric ID of user to unfollow</param>
        /// <param name="screenName">Screen name of user to unfollow</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>followed friend user info</returns>
        public static User DestroyFriendship(this TwitterContext ctx, string userID, string screenName, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.", "UserIDOrScreenName");
            }

            string destroyUrl = ctx.BaseUrl + "friendships/destroy.json";

            var parms = new Dictionary<string, string>();
            if (screenName != null) parms.Add("screen_name", screenName);
            if (userID != null) parms.Add("user_id", userID);

            var reqProc = new FriendshipRequestProcessor<User>();

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.PostToTwitter(
                    destroyUrl,
                    parms,
                    response => reqProc.ProcessActionResult(response, FriendshipAction.Destroy));

            User results = reqProc.ProcessActionResult(resultsJson, FriendshipAction.Destroy);
            return results;
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="userID">Twitter's ID for user</param>
        /// <param name="screenName">screen name of user to update</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <returns>updated friend user info</returns>
        public static Friendship UpdateFriendshipSettings(this TwitterContext ctx, string screenName, bool retweets, bool device)
        {
            return UpdateFriendshipSettings(ctx, 0, screenName, retweets, device, null);
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="userID">Twitter's ID for user</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <returns>updated friend user info</returns>
        public static Friendship UpdateFriendshipSettings(this TwitterContext ctx, ulong userID, bool retweets, bool device)
        {
            return UpdateFriendshipSettings(ctx, 0, null, retweets, device, null);
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="userID">Twitter's ID for user</param>
        /// <param name="screenName">screen name of user to update</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>updated friend user info</returns>
        public static Friendship UpdateFriendshipSettings(this TwitterContext ctx, ulong userID, string screenName, bool retweets, bool device, Action<TwitterAsyncResponse<Friendship>> callback)
        {
            if (string.IsNullOrEmpty(screenName) && userID <= 0)
            {
                throw new ArgumentNullException("screenNameOrUserID", "Either screenName or UserID is a required parameter.");
            }

            var parms = new Dictionary<string, string>
            {
                { "retweets", retweets.ToString().ToLower() },
                { "device", device.ToString().ToLower() }
            };

            if (screenName != null) parms.Add("screen_name", screenName);
            if (userID > 0) parms.Add("user_id", userID.ToString());

            string updateUrl = ctx.BaseUrl + "friendships/update.json";

            var reqProc = new FriendshipRequestProcessor<Friendship>();

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.PostToTwitter(
                    updateUrl,
                    parms,
                    response => reqProc.ProcessActionResult(response, FriendshipAction.Update));

            Friendship results = reqProc.ProcessActionResult(resultsJson, FriendshipAction.Update);
            return results;
        }
    }
}
