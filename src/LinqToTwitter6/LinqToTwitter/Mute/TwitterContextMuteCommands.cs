using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Mutes a user.
        /// </summary>
        /// <param name="sourceUserID">Following user ID</param>
        /// <param name="targetUserID">Followed user ID</param>
        /// <param name="cancelToken">Allows request cancellation</param>
        /// <returns>Indicates if the user was muted.</returns>
        public async Task<MuteResponse?> MuteAsync(string sourceUserID, string targetUserID, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = sourceUserID ?? throw new ArgumentNullException(nameof(sourceUserID), $"{nameof(sourceUserID)} is a required parameter.");
            _ = targetUserID ?? throw new ArgumentNullException(nameof(targetUserID), $"{nameof(targetUserID)} is a required parameter.");

            var url = $"{BaseUrl2}users/{sourceUserID}/muting";

            var postData = new Dictionary<string, string>();
            var postObj = new TwitterUserTargetID() { TargetUserID = targetUserID.ToString() };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            MuteResponse? result = JsonSerializer.Deserialize<MuteResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Unmutes a user.
        /// </summary>
        /// <param name="sourceUserID">Following user ID</param>
        /// <param name="targetUserID">Followed user ID</param>
        /// <param name="cancelToken">Allows request cancellation</param>
        /// <returns>Indicates if the user is no longer muted.</returns>
        public async Task<MuteResponse?> UnMuteAsync(string sourceUserID, string targetUserID, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = sourceUserID ?? throw new ArgumentNullException(nameof(sourceUserID), $"{nameof(sourceUserID)} is a required parameter.");
            _ = targetUserID ?? throw new ArgumentNullException(nameof(targetUserID), $"{nameof(targetUserID)} is a required parameter.");

            var url = $"{BaseUrl2}users/{sourceUserID}/muting/{targetUserID}";

            var postData = new Dictionary<string, string>();
            var postObj = new TwitterUserTargetID() { TargetUserID = targetUserID.ToString() };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            MuteResponse? result = JsonSerializer.Deserialize<MuteResponse>(RawResult);

            return result;
        }
    }
}
