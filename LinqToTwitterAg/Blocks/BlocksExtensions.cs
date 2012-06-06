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
        /// <param name="id">id of user to block</param>
        /// <returns>User that was unblocked</returns>
        public static User CreateBlock(this TwitterContext twitterCtx, string id)
        {
            return CreateBlock(twitterCtx, id, null);
        }

        /// <summary>
        /// Blocks a user
        /// </summary>
        /// <param name="twitterCtx">Twitter Context</param>
        /// <param name="id">id of user to block</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User that was unblocked</returns>
        public static User CreateBlock(this TwitterContext twitterCtx, string id, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var blocksUrl = twitterCtx.BaseUrl + "blocks/create/" + id + ".json";

            var reqProc = new BlocksRequestProcessor<User>();

            ITwitterExecute twitExe = twitterCtx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.ExecuteTwitter(
                    blocksUrl,
                    new Dictionary<string, string>(),
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User results = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return results;
        }

        /// <summary>
        /// Unblocks a user
        /// </summary>
        /// <param name="twitterCtx">Twitter Context</param>
        /// <param name="id">id of user to unblock</param>
        /// <returns>User that was unblocked</returns>
        public static User DestroyBlock(this TwitterContext twitterCtx, string id)
        {
            return DestroyBlock(twitterCtx, id, null);
        }

        /// <summary>
        /// Unblocks a user
        /// </summary>
        /// <param name="twitterCtx">Twitter Context</param>
        /// <param name="id">id of user to unblock</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>User that was unblocked</returns>
        public static User DestroyBlock(this TwitterContext twitterCtx, string id, Action<TwitterAsyncResponse<User>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var blocksUrl = twitterCtx.BaseUrl + "blocks/destroy/" + id + ".json";

            var reqProc = new BlocksRequestProcessor<User>();

            ITwitterExecute twitExe = twitterCtx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.ExecuteTwitter(
                    blocksUrl,
                    new Dictionary<string, string>(),
                    response => reqProc.ProcessActionResult(response, UserAction.SingleUser));

            User results = reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
            return results;
        }

    }
}
