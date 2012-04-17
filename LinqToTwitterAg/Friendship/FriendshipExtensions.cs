using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Globalization;

namespace LinqToTwitter
{
    public static class FriendshipExtensions
    {
        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">ID or screen name of user to follow (use userID or ScreenName to avoid ambiguity)</param>
        /// <param name="userID">Numeric ID of user to follow</param>
        /// <param name="screenName">Screen name of user to follow</param>
        /// <param name="follow">Receive notifications for the followed friend</param>
        /// <returns>followed friend user info</returns>
        public static User CreateFriendship(this TwitterContext ctx, string id, string userID, string screenName, bool follow)
        {
            return CreateFriendship(ctx, id, userID, screenName, follow, null);
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">ID or screen name of user to follow (use userID or ScreenName to avoid ambiguity)</param>
        /// <param name="userID">Numeric ID of user to follow</param>
        /// <param name="screenName">Screen name of user to follow</param>
        /// <param name="follow">Receive notifications for the followed friend</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>followed friend user info</returns>
        public static User CreateFriendship(this TwitterContext ctx, string id, string userID, string screenName, bool follow, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string destroyUrl;

            if (!string.IsNullOrEmpty(id))
            {
                destroyUrl = ctx.BaseUrl + "friendships/create/" + id + ".xml";
            }
            else if (!string.IsNullOrEmpty(userID))
            {
                destroyUrl = ctx.BaseUrl + "friendships/create/" + userID + ".xml";
            }
            else
            {
                destroyUrl = ctx.BaseUrl + "friendships/create/" + screenName + ".xml";
            }

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

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsXml =
                twitExe.ExecuteTwitter(
                    destroyUrl,
                    createParams,
                    reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">ID or screen name of user to unfollow (use userID or ScreenName to avoid ambiguity)</param>
        /// <param name="userID">Numeric ID of user to unfollow</param>
        /// <param name="screenName">Screen name of user to unfollow</param>
        /// <returns>followed friend user info</returns>
        public static User DestroyFriendship(this TwitterContext ctx, string id, string userID, string screenName)
        {
            return DestroyFriendship(ctx, id, userID, screenName, null);
        }

        /// <summary>
        /// lets logged-in user follow another user
        /// </summary>
        /// <param name="id">ID or screen name of user to unfollow (use userID or ScreenName to avoid ambiguity)</param>
        /// <param name="userID">Numeric ID of user to unfollow</param>
        /// <param name="screenName">Screen name of user to unfollow</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>followed friend user info</returns>
        public static User DestroyFriendship(this TwitterContext ctx, string id, string userID, string screenName, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.");
            }

            string destroyUrl;

            if (!string.IsNullOrEmpty(id))
            {
                destroyUrl = ctx.BaseUrl + "friendships/destroy/" + id + ".xml";
            }
            else
            {
                destroyUrl = ctx.BaseUrl + "friendships/destroy.xml";
            }

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsXml =
                twitExe.ExecuteTwitter(
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID },
                        { "screen_name", screenName }
                    },
                    reqProc);

            List<User> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="screenName">screen name of user to update</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <returns>updated friend user info</returns>
        public static Friendship UpdateFriendshipSettings(this TwitterContext ctx, string screenName, bool retweets, bool device)
        {
            return UpdateFriendshipSettings(ctx, screenName, retweets, device, null);
        }

        /// <summary>
        /// lets logged-in user set retweets and/or device notifications for a follower
        /// </summary>
        /// <param name="screenName">screen name of user to update</param>
        /// <param name="retweets">Enable retweets</param>
        /// <param name="device">Receive notifications</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>updated friend user info</returns>
        public static Friendship UpdateFriendshipSettings(this TwitterContext ctx, string screenName, bool retweets, bool device, Action<TwitterAsyncResponse<Friendship>> callback)
        {
            if (string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentNullException("screenName", "screenName is a required parameter.");
            }

            string updateUrl = ctx.BaseUrl + "friendships/update.xml";

            var reqProc = new FriendshipRequestProcessor<Friendship>();

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsXml =
                twitExe.ExecuteTwitter(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        { "screen_name", screenName },
                        { "retweets", retweets.ToString(CultureInfo.InvariantCulture).ToLower() },
                        { "device", device.ToString(CultureInfo.InvariantCulture).ToLower() }
                    },
                    reqProc);

            var results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }
    }
}
