using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Blocks a user.
        /// </summary>
        /// <param name="userID">ID of user to block</param>
        /// <param name="screenName">Screen name of user to block</param>
        /// <param name="skipStatus">Don't include status</param>
        /// <returns>User that was unblocked</returns>
        public async Task<User> CreateBlockAsync(ulong userID, string screenName, bool skipStatus)
        {
            return await CreateBlockAsync(userID, screenName, true, skipStatus);
        }

        /// <summary>
        /// Blocks a user.
        /// </summary>
        /// <param name="userID">ID of user to block</param>
        /// <param name="screenName">Screen name of user to block</param>
        /// <param name="includeEntities">Set to false to not include entities (default: true)</param>
        /// <param name="skipStatus">Don't include status</param>
        /// <returns>User that was unblocked</returns>
        public async Task<User> CreateBlockAsync(ulong userID, string screenName, bool includeEntities, bool skipStatus)
        {
            if (userID <= 0 && string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("Either userID or screenName are required parameters.", "UserIDOrScreenName");

            var blocksUrl = BaseUrl + "blocks/create.json";

            var reqProc = new BlocksRequestProcessor<User>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<User>(
                    blocksUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID <= 0 ? (string)null : userID.ToString() },
                        { "screen_name", screenName },
                        { "include_entities", includeEntities.ToString().ToLower() },
                        { "skip_status", skipStatus.ToString().ToLower() }
                    });

            return reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
        }

        /// <summary>
        /// Unblocks a user.
        /// </summary>
        /// <param name="userID">ID of user to block</param>
        /// <param name="screenName">Screen name of user to block</param>
        /// <param name="skipStatus">Don't include status</param>
        /// <returns>User that was unblocked</returns>
        public async Task<User> DestroyBlockAsync(ulong userID, string screenName, bool skipStatus)
        {
            return await DestroyBlockAsync(userID, screenName, true, skipStatus);
        }

        /// <summary>
        /// Unblocks a user.
        /// </summary>
        /// <param name="userID">ID of user to block</param>
        /// <param name="screenName">Screen name of user to block</param>
        /// <param name="includeEntities">Set to false to not include entities (default: true)</param>
        /// <param name="skipStatus">Don't include status</param>
        /// <returns>User that was unblocked</returns>
        public async Task<User> DestroyBlockAsync(ulong userID, string screenName, bool includeEntities, bool skipStatus)
        {
            if (userID <= 0 && string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("Either userID or screenName are required parameters.", "UserIDOrScreenName");

            var blocksUrl = BaseUrl + "blocks/destroy.json";

            var reqProc = new BlocksRequestProcessor<User>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<User>(
                    blocksUrl,
                    new Dictionary<string, string>
                    {
                        { "user_id", userID <= 0 ? (string)null : userID.ToString() },
                        { "screen_name", screenName },
                        { "include_entities", includeEntities.ToString().ToLower() },
                        { "skip_status", skipStatus.ToString().ToLower() }
                    });

            return reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
        }
    }
}
