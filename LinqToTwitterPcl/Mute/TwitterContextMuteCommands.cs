using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Mutes a user.
        /// </summary>
        /// <param name="screenName">Screen name of user to mute.</param>
        /// <returns>User entity for muted user.</returns>
        public async Task<User> MuteAsync(string screenName)
        {
            if (string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentNullException("screenName", "screenName is required");

            var muteParams = new Dictionary<string, string> { { "screen_name", screenName } };

            return await MuteAsync(muteParams).ConfigureAwait(false);
        }

        /// <summary>
        /// Mutes a user.
        /// </summary>
        /// <param name="userID">ID of user to mute.</param>
        /// <returns>User entity for muted user.</returns>
        public async Task<User> MuteAsync(ulong userID)
        {
            if (userID == 0)
                throw new ArgumentException("userID can't be 0 - no user has this ID", "userID");

            var muteParams = new Dictionary<string, string> { { "user_id", userID.ToString() } };

            return await MuteAsync(muteParams).ConfigureAwait(false);
        }

        async Task<User> MuteAsync(IDictionary<string, string> muteParams)
        {
            var muteUrl = BaseUrl + "mutes/users/create.json";

            var reqProc = new UserRequestProcessor<User>();

            var resultsJson =
                await TwitterExecutor
                    .PostToTwitterAsync<User>(muteUrl, muteParams)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
        }

        /// <summary>
        /// UnMutes a user.
        /// </summary>
        /// <param name="screenName">Screen name of user to mute.</param>
        /// <returns>User entity for muted user.</returns>
        public async Task<User> UnMuteAsync(string screenName)
        {
            if (string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentNullException("screenName", "screenName is required");

            var muteParams = new Dictionary<string, string> { { "screen_name", screenName } };

            return await UnMuteAsync(muteParams).ConfigureAwait(false);
        }

        /// <summary>
        /// UnMutes a user.
        /// </summary>
        /// <param name="userID">ID of user to mute.</param>
        /// <returns>User entity for muted user.</returns>
        public async Task<User> UnMuteAsync(ulong userID)
        {
            if (userID == 0)
                throw new ArgumentException("userID can't be 0 - no user has this ID", "userID");

            var muteParams = new Dictionary<string, string> { { "user_id", userID.ToString() } };

            return await UnMuteAsync(muteParams).ConfigureAwait(false);
        }

        async Task<User> UnMuteAsync(IDictionary<string, string> muteParams)
        {
            var muteUrl = BaseUrl + "mutes/users/destroy.json";

            var reqProc = new UserRequestProcessor<User>();

            var resultsJson =
                await TwitterExecutor
                    .PostToTwitterAsync<User>(muteUrl, muteParams)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(resultsJson, UserAction.SingleUser);
        }
    }
}
