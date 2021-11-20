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
        /// Posts a tweet with image or video media
        /// </summary>
        /// <param name="text">Tweet text</param>
        /// <param name="mediaIds">List of IDs for media that were previously uploaded</param>
        /// <param name="taggedUserIds">List of user Ids to tag in tweet</param>
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
            _ = mediaIds ?? throw new ArgumentNullException(nameof(mediaIds));

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
        /// Posts a tweet with a poll
        /// </summary>
        /// <param name="text">Tweet text</param>
        /// <param name="duration">Number of minutes to run poll</param>
        /// <param name="options">List of voting options</param>
        /// <param name="directMessageLink">Link to a DM</param>
        /// <param name="forSuperFollowersOnly">Only show to super followers</param>
        /// <param name="geo">Tweet location - <see cref="TweetGeo"/></param>
        /// <param name="quoteTweetID">ID of tweet to quote (quoted retweet)</param>
        /// <param name="replySettings">Who can reply - <see cref="TweetReplySettings"/></param>
        /// <param name="cancelToken"></param>
        /// <returns>Tweet with new ID and Text - <see cref="Tweet"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Tweet?> TweetPollAsync(
            string text,
            int duration,
            IEnumerable<string>? options = null,
            Uri? directMessageLink = null,
            bool forSuperFollowersOnly = false,
            TweetGeo? geo = null,
            string? quoteTweetID = null,
            TweetReplySettings? replySettings = null,
            CancellationToken cancelToken = default)
        {
            _ = text ?? throw new ArgumentNullException(nameof(text));
            if (duration == 0) throw new ArgumentNullException(nameof(duration));
            _ = options ?? throw new ArgumentNullException(nameof(options));

            string url = $"{BaseUrl2}tweets";

            var postData = new Dictionary<string, string>();
            var postObj = new TweetRequest
            {
                Text = text,
                DirectMessageDeepLink = directMessageLink?.AbsoluteUri,
                ForSuperFollowersOnly = forSuperFollowersOnly,
                Geo = geo,
                Poll = new TweetPoll
                {
                    DurationMinutes = duration,
                    Options = options
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
        /// Posts a new tweet
        /// </summary>
        /// <param name="text">Tweet text</param>
        /// <param name="replyTweetID">ID of tweet to reply to</param>
        /// <param name="excludeUserIds">List of Ids for users you don't want to reply to</param>
        /// <param name="directMessageLink">Link to a DM</param>
        /// <param name="forSuperFollowersOnly">Only show to super followers</param>
        /// <param name="geo">Tweet location - <see cref="TweetGeo"/></param>
        /// <param name="quoteTweetID">ID of tweet to quote (quoted retweet)</param>
        /// <param name="replySettings">Who can reply - <see cref="TweetReplySettings"/></param>
        /// <param name="cancelToken"></param>
        /// <returns>Tweet with new ID and Text - <see cref="Tweet"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Tweet?> ReplyAsync(
            string text,
            string replyTweetID,
            IEnumerable<string>? excludeUserIds = null,
            Uri? directMessageLink = null,
            bool forSuperFollowersOnly = false,
            TweetGeo? geo = null,
            string? quoteTweetID = null,
            TweetReplySettings? replySettings = null,
            CancellationToken cancelToken = default)
        {
            _ = text ?? throw new ArgumentNullException(nameof(text));
            _ = replyTweetID ?? throw new ArgumentNullException(nameof(replyTweetID));

            string url = $"{BaseUrl2}tweets";

            var postData = new Dictionary<string, string>();
            var postObj = new TweetRequest
            {
                Text = text,
                DirectMessageDeepLink = directMessageLink?.AbsoluteUri,
                ForSuperFollowersOnly = forSuperFollowersOnly,
                Geo = geo,
                QuoteTweetID = quoteTweetID,
                Reply = new TweetReply
                {
                    ExcludeReplyUserIds = excludeUserIds,
                    InReplyToTweetID = replyTweetID,
                },
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
        /// Deletes a tweet
        /// </summary>
        /// <param name="tweetID">ID of the tweet to delete</param>
        /// <param name="cancelToken">Optional cancellation token</param>
        /// <returns>Deleted status - true if delete succeeds</returns>
        public async Task<TweetDeletedResponse?> DeleteTweetAsync(string tweetID, CancellationToken cancelToken = default)
        {
            _ = tweetID ?? throw new ArgumentNullException(nameof(tweetID), $"{nameof(tweetID)} is required.");

            string url = $"{BaseUrl2}tweets/{tweetID}";

            var postData = new Dictionary<string, string>();
            var postObj = new TweetDeleteRequest() { ID = tweetID };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Delete.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            TweetDeletedResponse? result = JsonSerializer.Deserialize<TweetDeletedResponse>(RawResult);

            return result;
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
