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
        /// Bookmarks a tweet
        /// </summary>
        /// <param name="userID">ID of user who is bookmarking tweet</param>
        /// <param name="tweetID">ID of the bookmarked tweet</param>
        /// <param name="cancelToken">Optional cancellation token</param>
        /// <returns>Bookmark status of reply - true if reply is hidden</returns>
        public async Task<BookmarkResponse?> BookmarkAsync(string userID, string tweetID, CancellationToken cancelToken = default)
        {
            _ = userID ?? throw new ArgumentNullException(nameof(userID), $"{nameof(userID)} is required.");
            _ = tweetID ?? throw new ArgumentNullException(nameof(tweetID), $"{nameof(tweetID)} is required.");

            string url = $"{BaseUrl2}users/{userID}/bookmarks";

            var postData = new Dictionary<string, string>();
            var postObj = new BookmarkedTweetID { TweetID = tweetID };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            BookmarkResponse? result = JsonSerializer.Deserialize<BookmarkResponse>(RawResult);

            return result;
        }

        /// <summary>
        /// Removes a bookmark on a tweet
        /// </summary>
        /// <param name="userID">ID of user who is bookmarking tweet</param>
        /// <param name="tweetID">ID of the bookmarked tweet</param>
        /// <param name="cancelToken">Optional cancellation token</param>
        /// <returns>Hidden status of reply - false if reply is no longer hidden</returns>
        public async Task<BookmarkResponse?> RemoveBookmarkAsync(string userID, string tweetID, CancellationToken cancelToken = default)
        {
            _ = userID ?? throw new ArgumentNullException(nameof(userID), $"{nameof(userID)} is required.");
            _ = tweetID ?? throw new ArgumentNullException(nameof(tweetID), $"{nameof(tweetID)} is required.");

            string url = $"{BaseUrl2}users/{userID}/bookmarks/{tweetID}";

            var postData = new Dictionary<string, string>();
            var postObj = new BookmarkedTweetID { TweetID = tweetID };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            BookmarkResponse? result = JsonSerializer.Deserialize<BookmarkResponse>(RawResult);

            return result;
        }
    }
}
