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
        /// Posts a new tweet
        /// </summary>
        /// <param name="text">Tweet text</param>
        /// <param name="directMessageLink">Link to a DM</param>
        /// <param name="forSuperFollowersOnly">Only show to super followers</param>
        /// <param name="geo">Tweet location - <see cref="TweetGeo"/></param>
        /// <param name="quoteTweetID">ID of tweet to quote (quoted retweet)</param>
        /// <param name="replySettings">Who can reply - <see cref="TweetReplySettings"/></param>
        /// <param name="cancelToken"></param>
        /// <returns>Tweet with new ID and Text - <see cref="Tweet"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Tweet?> TweetAsync(
            string text, 
            Uri? directMessageLink = null, 
            bool forSuperFollowersOnly = false, 
            TweetGeo? geo = null,
            string? quoteTweetID = null,
            TweetReplySettings? replySettings = null,
            CancellationToken cancelToken = default)
        {
            _ = text ?? throw new ArgumentNullException(nameof(text));

            string url = $"{BaseUrl2}tweets";

            var postData = new Dictionary<string, string>();
            var postObj = new TweetRequest
            {
                Text = text,
                DirectMessageDeepLink = directMessageLink?.AbsoluteUri,
                ForSuperFollowersOnly = forSuperFollowersOnly,
                Geo = geo,
                QuoteTweetID = quoteTweetID,
                ReplySettings = replySettings,
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            TweetResponse? response = JsonSerializer.Deserialize<TweetResponse>(RawResult);
            return response?.Tweet;
        }

        /// <summary>
        /// Posts a new tweet
        /// </summary>
        /// <param name="text">Tweet text</param>
        /// <param name="directMessageLink">Link to a DM</param>
        /// <param name="forSuperFollowersOnly">Only show to super followers</param>
        /// <param name="geo">Tweet location - <see cref="TweetGeo"/></param>
        /// <param name="quoteTweetID">ID of tweet to quote (quoted retweet)</param>
        /// <param name="replySettings">Who can reply - <see cref="TweetReplySettings"/></param>
        /// <param name="cancelToken"></param>
        /// <returns>Tweet with new ID and Text - <see cref="Tweet"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Tweet?> TweetMediaAsync(
            string text,
            IEnumerable<string> mediaIds,
            IEnumerable<string>? taggedUserIds = null,
            Uri? directMessageLink = null,
            bool forSuperFollowersOnly = false,
            TweetGeo? geo = null,
            string? quoteTweetID = null,
            TweetReplySettings? replySettings = null,
            CancellationToken cancelToken = default)
        {
            _ = text ?? throw new ArgumentNullException(nameof(text));

            string url = $"{BaseUrl2}tweets";

            var postData = new Dictionary<string, string>();
            var postObj = new TweetRequest
            {
                Text = text,
                DirectMessageDeepLink = directMessageLink?.AbsoluteUri,
                ForSuperFollowersOnly = forSuperFollowersOnly,
                Geo = geo,
                Media = new TweetMedia
                {
                    MediaIds = mediaIds,
                    TaggedUserIds = taggedUserIds,
                },
                QuoteTweetID = quoteTweetID,
                ReplySettings = replySettings,
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            TweetResponse? response = JsonSerializer.Deserialize<TweetResponse>(RawResult);
            return response?.Tweet;
        }

        /// <summary>
        /// Hides a reply to a tweet
        /// </summary>
        /// <param name="tweetID">ID of the replying tweet</param>
        /// <param name="cancelToken">Optional cancellation token</param>
        /// <exception cref="TwitterQueryException">Will receive 403 Forbidden if <see cref="tweetID"/> is for a tweet that is not a reply</exception>
        /// <returns>Hidden status of reply - true if reply is hidden</returns>
        public async Task<TweetHideResponse?> HideReplyAsync(string tweetID, CancellationToken cancelToken = default)
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
        public async Task<TweetHideResponse?> UnhideReplyAsync(string tweetID, CancellationToken cancelToken = default)
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
        async Task<TweetHideResponse?> HideReplyAsync(string tweetID, bool shouldHide, CancellationToken cancelToken)
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

            return result;
        }
    }
}
