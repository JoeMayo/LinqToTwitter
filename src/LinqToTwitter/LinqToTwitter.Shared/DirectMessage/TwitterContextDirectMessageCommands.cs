using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Sends a new direct message to specified user.
        /// </summary>
        /// <param name="screenName">ScreenName of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <returns>Direct message element.</returns>
        public async Task<DirectMessage> NewDirectMessageAsync(string screenName, string text, CancellationToken cancelToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("screenName is a required parameter.", "screenName");

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("text is a required parameter.", "text");

            var newUrl = BaseUrl + "direct_messages/new.json";

            var reqProc = new DirectMessageRequestProcessor<DirectMessage>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<DirectMessage>(
                    HttpMethod.Post,
                    newUrl,
                    new Dictionary<string, string>
                    {
                        {"screen_name", screenName},
                        {"text", text}
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, DirectMessageType.Show);
        }

        /// <summary>
        /// Sends a new direct message to specified user.
        /// </summary>
        /// <param name="userID">User ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <returns>Direct message element.</returns>
        public async Task<DirectMessage> NewDirectMessageAsync(ulong userID, string text, CancellationToken cancelToken = default(CancellationToken))
        {
            if (userID == 0)
                throw new ArgumentException("userID must be set.", "userID");

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("text is a required parameter.", "text");

            var newUrl = BaseUrl + "direct_messages/new.json";

            var reqProc = new DirectMessageRequestProcessor<DirectMessage>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<DirectMessage>(
                    HttpMethod.Post,
                    newUrl,
                    new Dictionary<string, string>
                    {
                        {"user_id", userID.ToString()},
                        {"text", text}
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, DirectMessageType.Show);
        }

        /// <summary>
        /// Deletes a direct message.
        /// </summary>
        /// <param name="id">id of direct message</param>
        /// <param name="includeEntites">Set to false to prevent entities from being included (default: true).</param>
        /// <returns>direct message element</returns>
        public async Task<DirectMessage> DestroyDirectMessageAsync(ulong id, bool includeEntites, CancellationToken cancelToken = default(CancellationToken))
        {
            if (id == 0)
                throw new ArgumentNullException("id", "id is required.");

            var destroyUrl = BaseUrl + "direct_messages/destroy.json";

            var reqProc = new DirectMessageRequestProcessor<DirectMessage>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<DirectMessage>(
                    HttpMethod.Post,
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        {"id", id.ToString()},
                        {"include_entities", includeEntites.ToString().ToLower()}
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, DirectMessageType.Show);
        }
    }
}
