using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    public static class BlocksExtensions
    {
        /// <summary>
        /// Blocks a user
        /// </summary>
        /// <param name="twitterCtx">Twitter Context</param>
        /// <param name="userID">ID of user to block</param>
        /// <param name="screenName">Screen name of user to block</param>
        /// <param name="skipStatus">Don't include status</param>
        /// <returns>User that was unblocked</returns>
        public static User CreateBlock(this TwitterContext twitterCtx, ulong userID, string screenName, bool skipStatus)
        {
            return CreateBlock(twitterCtx, userID, screenName, skipStatus, null);
        }

        /// <summary>
        /// Blocks a user
        /// </summary>
        /// <param name="twitterCtx">Twitter Context</param>
        /// <param name="userID">ID of user to block</param>
        /// <param name="screenName">Screen name of user to block</param>
        /// <param name="skipStatus">Don't include status</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User that was unblocked</returns>
        public static User CreateBlock(this TwitterContext twitterCtx, ulong userID, string screenName, bool skipStatus, Action<TwitterAsyncResponse<User>> callback)
        {
            if (userID <= 0 && string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either userID or screenName are required parameters.", "UserIDOrScreenName");
            }

            var blocksUrl = twitterCtx.BaseUrl + "blocks/create.json";

            var reqProc = new BlocksRequestProcessor<User>();

            ITwitterExecute twitExe = twitterCtx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.ExecuteTwitter(
                    blocksUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID <= 0 ? (string)null : userID.ToString() },
                        { "screen_name", screenName },
                        { "skip_status", skipStatus.ToString().ToLower() }
                    },
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User results = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return results;
        }

        /// <summary>
        /// Unblocks a user
        /// </summary>
        /// <param name="twitterCtx">Twitter Context</param>
        /// <param name="userID">ID of user to block</param>
        /// <param name="screenName">Screen name of user to block</param>
        /// <param name="skipStatus">Don't include status</param>
        /// <returns>User that was unblocked</returns>
        public static User DestroyBlock(this TwitterContext twitterCtx, ulong userID, string screenName, bool skipStatus)
        {
            return DestroyBlock(twitterCtx, userID, screenName, skipStatus, null);
        }

        /// <summary>
        /// Unblocks a user
        /// </summary>
        /// <param name="twitterCtx">Twitter Context</param>
        /// <param name="userID">ID of user to block</param>
        /// <param name="screenName">Screen name of user to block</param>
        /// <param name="skipStatus">Don't include status</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User that was unblocked</returns>
        public static User DestroyBlock(this TwitterContext twitterCtx, ulong userID, string screenName, bool skipStatus, Action<TwitterAsyncResponse<User>> callback)
        {
            if (userID <= 0 && string.IsNullOrEmpty(screenName))
            {
                throw new ArgumentException("Either userID or screenName are required parameters.", "UserIDOrScreenName");
            }

            var blocksUrl = twitterCtx.BaseUrl + "blocks/destroy.json";

            var reqProc = new BlocksRequestProcessor<User>();

            ITwitterExecute twitExe = twitterCtx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.ExecuteTwitter(
                    blocksUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID <= 0 ? (string)null : userID.ToString() },
                        { "screen_name", screenName },
                        { "skip_status", skipStatus.ToString().ToLower() }
                    },
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User results = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return results;
        }
    }
}
