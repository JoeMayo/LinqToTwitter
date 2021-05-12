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
        /// Block a user.
        /// </summary>
        /// <param name="sourceUserID">Following user ID</param>
        /// <param name="targetUserID">Followed user ID</param>
        /// <param name="cancelToken">Allows request cancellation</param>
        /// <returns>User that was unblocked</returns>
        public async Task<BlockingResponse?> BlockUserAsync(string sourceUserID, string targetUserID, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = sourceUserID ?? throw new ArgumentException($"{nameof(sourceUserID)} is a required parameter.", nameof(sourceUserID));
            _ = targetUserID ?? throw new ArgumentException($"{nameof(targetUserID)} is a required parameter.", nameof(targetUserID));

            var url = $"{BaseUrl2}users/{sourceUserID}/blocking";

            var reqProc = new BlocksRequestProcessor<User>();

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

            BlockingResponse? result = JsonSerializer.Deserialize<BlockingResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Unblock a user.
        /// </summary>
        /// <param name="sourceUserID">Following user ID</param>
        /// <param name="targetUserID">Followed user ID</param>
        /// <param name="cancelToken">Allows request cancellation</param>
        /// <returns>User that was unblocked</returns>
        public async Task<BlockingResponse?> UnblockUserAsync(string sourceUserID, string targetUserID, CancellationToken cancelToken = default(CancellationToken))
        {
            _ = sourceUserID ?? throw new ArgumentException($"{nameof(sourceUserID)} is a required parameter.", nameof(sourceUserID));
            _ = targetUserID ?? throw new ArgumentException($"{nameof(targetUserID)} is a required parameter.", nameof(targetUserID));

            var url = $"{BaseUrl2}users/{sourceUserID}/blocking/{targetUserID}";

            var reqProc = new BlocksRequestProcessor<User>();

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

            BlockingResponse? result = JsonSerializer.Deserialize<BlockingResponse>(RawResult);

            return result;
        }
    }
}
