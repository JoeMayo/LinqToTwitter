using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    public static class NotificationsExtensions
    {
        const string UserIdOrScreenNameParam = "UserIdOrScreenName";

        /// <summary>
        /// Disables notifications from specified user. (Notification Leave)
        /// </summary>
        /// <remarks>
        /// A least one parameter is required.
        /// </remarks>
        /// <param name="id">ID of user to disable notifications on.</param>
        /// <param name="userID">ID of user - disambiguates when ID is screen name.</param>
        /// <param name="screenName">Screen Name of user - disambiguates when ID is screen name.</param>
        /// <returns>Specified user info</returns>
        public static User DisableNotifications(this TwitterContext ctx, string id, string userID, string screenName)
        {
            return DisableNotifications(ctx, id, userID, screenName, null);
        }

        /// <summary>
        /// Disables notifications from specified user. (Notification Leave)
        /// </summary>
        /// <remarks>
        /// A least one parameter is required.
        /// </remarks>
        /// <param name="id">ID of user to disable notifications on.</param>
        /// <param name="userID">ID of user - disambiguates when ID is screen name.</param>
        /// <param name="screenName">Screen Name of user - disambiguates when ID is screen name.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>Specified user info</returns>
        public static User DisableNotifications(this TwitterContext ctx, string id, string userID, string screenName, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.", UserIdOrScreenNameParam);
            }

            string notificationsUrl;

            if (!string.IsNullOrEmpty(id))
            {
                notificationsUrl = ctx.BaseUrl + "notifications/leave/" + id + ".json";
            }
            else if (!string.IsNullOrEmpty(userID))
            {
                notificationsUrl = ctx.BaseUrl + "notifications/leave/" + userID + ".json";
            }
            else
            {
                notificationsUrl = ctx.BaseUrl + "notifications/leave/" + screenName + ".json";
            }

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    },
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        }

        /// <summary>
        /// Enables notifications from specified user (Notification Follow)
        /// </summary>
        /// <remarks>
        /// A least one parameter is required.
        /// </remarks>
        /// <param name="id">ID of user to enable notifications on.</param>
        /// <param name="userID">ID of user - disambiguates when ID is screen name.</param>
        /// <param name="screenName">Screen Name of user - disambiguates when ID is screen name.</param>
        /// <returns>Specified user info</returns>
        public static User EnableNotifications(this TwitterContext ctx, string id, string userID, string screenName)
        {
            return EnableNotifications(ctx, id, userID, screenName, null);
        }

        /// <summary>
        /// Enables notifications from specified user (Notification Follow)
        /// </summary>
        /// <remarks>
        /// A least one parameter is required.
        /// </remarks>
        /// <param name="id">ID of user to enable notifications on.</param>
        /// <param name="userID">ID of user - disambiguates when ID is screen name.</param>
        /// <param name="screenName">Screen Name of user - disambiguates when ID is screen name.</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>Specified user info</returns>
        public static User EnableNotifications(this TwitterContext ctx, string id, string userID, string screenName, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(id) &&
                string.IsNullOrEmpty(userID) &&
                string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either id, userID, or screenName is a required parameter.", UserIdOrScreenNameParam);
            }

            string notificationsUrl;

            if (!string.IsNullOrEmpty(id))
            {
                notificationsUrl = ctx.BaseUrl + "notifications/follow/" + id + ".json";
            }
            else if (!string.IsNullOrEmpty(userID))
            {
                notificationsUrl = ctx.BaseUrl + "notifications/follow/" + userID + ".json";
            }
            else
            {
                notificationsUrl = ctx.BaseUrl + "notifications/follow/" + screenName + ".json";
            }

            var reqProc = new UserRequestProcessor<User>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
                    notificationsUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID},
                        {"screen_name", screenName}
                    },
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User user = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return user;
        }

    }
}
