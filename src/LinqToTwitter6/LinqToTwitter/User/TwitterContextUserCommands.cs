using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Lets logged-in user report spam.
        /// </summary>
        /// <param name="userID">User id of alleged spammer.</param>
        /// <returns>Alleged spammer user info.</returns>
        public async Task<User?> ReportSpamAsync(ulong userID, bool performBlock = false, CancellationToken cancelToken = default)
        {
            if (userID == 0)
                throw new ArgumentException("Twitter doesn't have a user with ID == 0", nameof(userID));

            var reportParams = new Dictionary<string, string?>
            {
                { "user_id", userID.ToString() },
                { "perform_block", performBlock ? bool.TrueString.ToLower() : null }
            };

            return await ReportSpamAsync(reportParams, cancelToken).ConfigureAwait(false);
        }


        /// <summary>
        /// Lets logged-in user report spam.
        /// </summary>
        /// <param name="screenName">Screen name of alleged spammer.</param>
        /// <returns>Alleged spammer user info.</returns>
        public async Task<User?> ReportSpamAsync(string screenName, bool performBlock = false, CancellationToken cancelToken = default)
        {
            if (string.IsNullOrWhiteSpace(screenName))
                throw new ArgumentException("Please supply a valid screen name", nameof(screenName));

            var reportParams = new Dictionary<string, string?>
            {
                { "screen_name", screenName },
                { "perform_block", performBlock ? bool.TrueString.ToLower() : null }
            };

            return await ReportSpamAsync(reportParams, cancelToken).ConfigureAwait(false);
        }

        internal async Task<User?> ReportSpamAsync(IDictionary<string, string?> reportParams, CancellationToken cancelToken = default)
        {
            string reportSpamUrl = BaseUrl + "users/report_spam.json";

            RawResult =
                await TwitterExecutor
                    .PostFormUrlEncodedToTwitterAsync<User>(HttpMethod.Post.ToString(), reportSpamUrl, reportParams, cancelToken)
                    .ConfigureAwait(false);

            return new UserRequestProcessor<User>()
                .ProcessActionResult(RawResult, StatusAction.SingleStatus);
        }

        /// <summary>
        /// Make a source user follow a target user
        /// </summary>
        /// <param name="sourceUserID">Following user ID</param>
        /// <param name="targetUserID">Followed user ID</param>
        /// <param name="cancelToken">Allows request cancellation</param>
        /// <returns>Follow Status</returns>
        public async Task<TwitterUserFollowResponse?> FollowAsync(string sourceUserID, string targetUserID, CancellationToken cancelToken = default)
        {
            _ = sourceUserID ?? throw new ArgumentException($"{nameof(sourceUserID)} is a required parameter.", nameof(sourceUserID));
            _ = targetUserID ?? throw new ArgumentException($"{nameof(targetUserID)} is a required parameter.", nameof(targetUserID));

            string url = $"{BaseUrl2}users/{sourceUserID}/following";

            var postData = new Dictionary<string, string>();
            var postObj = new TwitterUserTargetID() { TargetUserID = targetUserID };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            TwitterUserFollowResponse? result = JsonSerializer.Deserialize<TwitterUserFollowResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Make a source user un-follow a target user
        /// </summary>
        /// <param name="sourceUserID">Following user ID</param>
        /// <param name="targetUserID">Followed user ID</param>
        /// <param name="cancelToken">Allows request cancellation</param>
        /// <returns>Follow Status</returns>
        public async Task<TwitterUserFollowResponse?> UnFollowAsync(string sourceUserID, string targetUserID, CancellationToken cancelToken = default)
        {
            _ = sourceUserID ?? throw new ArgumentException($"{nameof(sourceUserID)} is a required parameter.", nameof(sourceUserID));
            _ = targetUserID ?? throw new ArgumentException($"{nameof(targetUserID)} is a required parameter.", nameof(targetUserID));

            string url = $"{BaseUrl2}users/{sourceUserID}/following/{targetUserID}";

            var postData = new Dictionary<string, string>();
            var postObj = new TwitterUserTargetID() { TargetUserID = targetUserID };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            TwitterUserFollowResponse? result = JsonSerializer.Deserialize<TwitterUserFollowResponse>(RawResult);

            return result;
        }
    }
}
