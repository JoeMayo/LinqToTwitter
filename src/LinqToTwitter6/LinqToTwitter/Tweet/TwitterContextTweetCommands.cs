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
        /// Hides a reply to a tweet
        /// </summary>
        /// <param name="tweetID">ID of the replying tweet</param>
        /// <param name="cancelToken">Optional cancellation token</param>
        /// <exception cref="TwitterQueryException">Will receive 403 Forbidden if <see cref="tweetID"/> is for a tweet that is not a reply</exception>
        /// <returns>Hidden status of reply - true if reply is hidden</returns>
        public async Task<bool> HideReplyAsync(string tweetID, CancellationToken cancelToken = default)
        {
            return await HideReplyAsync(tweetID, true, cancelToken);
        }

        /// <summary>
        /// Hides a reply to a tweet
        /// </summary>
        /// <param name="tweetID">ID of the replying tweet</param>
        /// <param name="cancelToken">Optional cancellation token</param>
        /// <exception cref="TwitterQueryException">Will receive 403 Forbidden if <see cref="tweetID"/> is for a tweet that is not a reply</exception>
        /// <returns>Hidden status of reply - false if reply is no longer hidden</returns>
        public async Task<bool> UnHideReplyAsync(string tweetID, CancellationToken cancelToken = default)
        {
            return await HideReplyAsync(tweetID, false, cancelToken);
        }

        /// <summary>
        /// Hides/unhides a reply to a tweet
        /// </summary>
        /// <param name="tweetID">ID of the replying tweet</param>
        /// <param name="shouldHide">true to hide/false to unhide</param>
        /// <param name="cancelToken">Optional cancellation token</param>
        /// <exception cref="TwitterQueryException">Will receive 403 Forbidden if ID is for a tweet that is not a reply</exception>
        /// <returns>Hidden status of reply - false if reply is no longer hidden</returns>
        async Task<bool> HideReplyAsync(string tweetID, bool shouldHide, CancellationToken cancelToken)
        {
            _ = tweetID ?? throw new ArgumentNullException(nameof(tweetID), $"{nameof(tweetID)} is required.");

            string url = $"{BaseUrl2}tweets/{tweetID}/hidden";

            var postData = new Dictionary<string, string>();
            var postObj = new TweetHidden() { Hidden = shouldHide };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Put.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            TweetHideResponse? result = JsonSerializer.Deserialize<TweetHideResponse>(RawResult);

            return result?.Data?.Hidden ?? false;
        }
    }
}
