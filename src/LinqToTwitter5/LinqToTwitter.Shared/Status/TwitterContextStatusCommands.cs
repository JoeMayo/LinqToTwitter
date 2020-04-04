using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        public const ulong MissingID = 0ul;
        public const ulong NoReply = 0ul;
        public const decimal NoCoordinate = Decimal.MaxValue;

        /// <summary>
        /// Replies to a tweet.
        /// </summary>
        /// <remarks>
        /// You must include the recipient's screen name (as @ScreenName) for the reply to work.
        /// </remarks>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <returns>Reply status.</returns>
        public virtual async Task<Status> ReplyAsync(ulong tweetID, string status)
        {
            return await TweetOrReplyAsync(tweetID, status, NoCoordinate, NoCoordinate, null, false, false, null, false, null, null).ConfigureAwait(false);
        }

        /// <summary>
        /// Replies to a tweet.
        /// </summary>
        /// <remarks>
        /// You must include the recipient's screen name (as @ScreenName) for the reply to work.
        /// </remarks>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Reply status.</returns>
        public virtual async Task<Status> ReplyAsync(ulong tweetID, string status, TweetMode tweetMode)
        {
            return await TweetOrReplyAsync(tweetID, status, NoCoordinate, NoCoordinate, null, false, false, null, false, null, null, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Replies to a tweet.
        /// </summary>
        /// <remarks>
        /// You must include the recipient's screen name (as @ScreenName) for the reply to work.
        /// </remarks>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <param name="autoPopulateReplyMetadata">Enables extended tweet mode where mentions don't count towards tweet length.</param>
        /// <param name="excludeReplyUserIds">Comma-separated list of @mentions to exclude from extended tweet prefix list.</param>
        /// <param name="attachmentUrl">Tweet link or DM deep link for extended tweet suffix that doesn't count towards tweet length.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Reply status.</returns>
        public virtual async Task<Status> ReplyAsync(ulong tweetID, string status, bool autoPopulateReplyMetadata, IEnumerable<ulong> excludeReplyUserIds, string attachmentUrl, TweetMode tweetMode = TweetMode.Compat)
        {
            return await TweetOrReplyAsync(tweetID, status, NoCoordinate, NoCoordinate, null, false, false, null, autoPopulateReplyMetadata, excludeReplyUserIds, attachmentUrl, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Replies to a tweet with coordinates.
        /// </summary>
        /// <remarks>
        /// You must include the recipient's screen name (as @ScreenName) for the reply to work.
        /// </remarks>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Reply status.</returns>
        public virtual async Task<Status> ReplyAsync(ulong tweetID, string status, decimal latitude, decimal longitude, bool displayCoordinates, TweetMode tweetMode = TweetMode.Compat)
        {
            return await ReplyAsync(tweetID, status, latitude, longitude, null, displayCoordinates, false, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Replies to a tweet with coordinates, place, and option to not include the user entity.
        /// </summary>
        /// <remarks>
        /// You must include the recipient's screen name (as @ScreenName) for the reply to work.
        /// </remarks>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="trimUser">Don't include user in returned Status.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Reply status.</returns>
        public virtual async Task<Status> ReplyAsync(ulong tweetID, string status, decimal latitude, decimal longitude, string placeID, bool trimUser, TweetMode tweetMode = TweetMode.Compat)
        {
            return await ReplyAsync(tweetID, status, latitude, longitude, placeID, false, trimUser, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Replies to a tweet with place, option to display coordinates, and option to not include user entity.
        /// </summary>
        /// <remarks>
        /// You must include the recipient's screen name (as @ScreenName) for the reply to work.
        /// </remarks>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Reply status.</returns>
        public virtual async Task<Status> ReplyAsync(ulong tweetID, string status, string placeID, bool displayCoordinates, bool trimUser, TweetMode tweetMode = TweetMode.Compat)
        {
            return await ReplyAsync(tweetID, status, NoCoordinate, NoCoordinate, placeID, displayCoordinates, trimUser, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Replies to a tweet with all non-media options.
        /// </summary>
        /// <remarks>
        /// You must include the recipient's screen name (as @ScreenName) for the reply to work.
        /// </remarks>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Reply status.</returns>
        public virtual async Task<Status> ReplyAsync(ulong tweetID, string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, bool trimUser, TweetMode tweetMode = TweetMode.Compat)
        {
            if (tweetID == MissingID)
                throw new ArgumentException("0 is *not* a valid tweetID. You must provide the ID of the tweet you're replying to.", "tweetID");

            return await TweetOrReplyAsync(tweetID, status, latitude, longitude, placeID, displayCoordinates, trimUser, null, false, null, null, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Replies to a tweet with attached media.
        /// </summary>
        /// <remarks>
        /// You must include the recipient's screen name (as @ScreenName) for the reply to work.
        /// </remarks>
        /// <param name="status">Status text.</param>
        /// <param name="mediaIds">Collection of ids of media to include in tweet.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public async Task<Status> ReplyAsync(ulong tweetID, string status, IEnumerable<ulong> mediaIds, TweetMode tweetMode = TweetMode.Compat)
        {
            if (tweetID == MissingID)
                throw new ArgumentException("0 is *not* a valid tweetID. You must provide the ID of the tweet you're replying to.", "tweetID");

            return await TweetOrReplyAsync(tweetID, status, NoCoordinate, NoCoordinate, NoInputParam, false, false, mediaIds, false, null, null, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Replies to a tweet with all options.
        /// </summary>
        /// <remarks>
        /// You must include the recipient's screen name (as @ScreenName) for the reply to work.
        /// </remarks>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <param name="trimUser">Remove user entity from response</param>
        /// <param name="mediaIds">Collection of ids of media to include in tweet.</param>
        /// <param name="autoPopulateReplyMetadata">Enables extended tweet mode where mentions don't count towards tweet length.</param>
        /// <param name="excludeReplyUserIds">Comma-separated list of @mentions to exclude from extended tweet prefix list.</param>
        /// <param name="attachmentUrl">Tweet link or DM deep link for extended tweet suffix that doesn't count towards tweet length.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public virtual async Task<Status> ReplyAsync(ulong tweetID, string status, decimal latitude = NoCoordinate, decimal longitude = NoCoordinate, string placeID = null, bool displayCoordinates = false, bool trimUser = false, IEnumerable<ulong> mediaIds = null, bool autoPopulateReplyMetadata = false, IEnumerable<ulong> excludeReplyUserIds = null, string attachmentUrl = null, TweetMode tweetMode = TweetMode.Compat)
        {
            if (tweetID == MissingID)
                throw new ArgumentException("0 is *not* a valid tweetID. You must provide the ID of the tweet you're replying to.", "tweetID");

            return await TweetOrReplyAsync(tweetID, status, latitude, longitude, placeID, displayCoordinates, trimUser, mediaIds, autoPopulateReplyMetadata, excludeReplyUserIds, attachmentUrl, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a status update.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public virtual async Task<Status> TweetAsync(string status, TweetMode tweetMode = TweetMode.Compat)
        {
            return await TweetOrReplyAsync(NoReply, status, NoCoordinate, NoCoordinate, null, false, false, null, false, null, null, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a status update.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="attachmentUrl">Tweet link or DM deep link for extended tweet suffix that doesn't count towards tweet length.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public virtual async Task<Status> TweetAsync(string status, string attachmentUrl, TweetMode tweetMode = TweetMode.Compat)
        {
            return await TweetOrReplyAsync(NoReply, status, NoCoordinate, NoCoordinate, null, false, false, null, false, null, attachmentUrl, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a status update with coordinates.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public async Task<Status> TweetAsync(string status, decimal latitude, decimal longitude, TweetMode tweetMode = TweetMode.Compat)
        {
            return await TweetOrReplyAsync(NoReply, status, latitude, longitude, null, false, false, null, false, null, null, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a status update with coordinates and option to display.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public virtual async Task<Status> TweetAsync(string status, decimal latitude, decimal longitude, bool displayCoordinates, TweetMode tweetMode = TweetMode.Compat)
        {
            return await TweetOrReplyAsync(NoReply, status, latitude, longitude, null, displayCoordinates, false, null, false, null, null, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a status update with coordinates, placeID, and option to not include user entity.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public async Task<Status> TweetAsync(string status, decimal latitude, decimal longitude, string placeID, bool trimUser, TweetMode tweetMode = TweetMode.Compat)
        {
            return await TweetOrReplyAsync(NoReply, status, latitude, longitude, placeID, false, trimUser, null, false, null, null, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a status update with all location options and option to not include user entity.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public virtual async Task<Status> TweetAsync(string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, bool trimUser, TweetMode tweetMode = TweetMode.Compat)
        {
            return await TweetOrReplyAsync(NoReply, status, latitude, longitude, placeID, displayCoordinates, trimUser, null, false, null, null, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a status update with place, option to display coordinates, and option to not include user entity.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public virtual async Task<Status> TweetAsync(string status, string placeID, bool displayCoordinates, bool trimUser, TweetMode tweetMode = TweetMode.Compat)
        {
            return await TweetOrReplyAsync(NoReply, status, NoCoordinate, NoCoordinate, placeID, displayCoordinates, trimUser, null, false, null, null, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a status update with attached media.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="mediaIds">Collection of ids of media to include in tweet.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public virtual async Task<Status> TweetAsync(string status, IEnumerable<ulong> mediaIds, TweetMode tweetMode = TweetMode.Compat)
        {
            return await TweetOrReplyAsync(NoReply, status, NoCoordinate, NoCoordinate, NoInputParam, false, false, mediaIds, false, null, null, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a status update with all options.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <param name="trimUser">Remove user entity from response</param>
        /// <param name="mediaIds">Collection of ids of media to include in tweet.</param>
        /// <param name="attachmentUrl">Tweet link or DM deep link for extended tweet suffix that doesn't count towards tweet length.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <returns>Tweeted status.</returns>
        public virtual async Task<Status> TweetAsync(string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, bool trimUser, IEnumerable<ulong> mediaIds, string attachmentUrl = null, TweetMode tweetMode = TweetMode.Compat)
        {
            return await TweetOrReplyAsync(NoReply, status, latitude, longitude, placeID, displayCoordinates, trimUser, mediaIds, false, null, attachmentUrl, tweetMode).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a status update.
        /// </summary>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to. Sent via ReplyAsync overloads.</param>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <param name="trimUser">Remove user entity from response</param>
        /// <param name="mediaIds">Collection of ids of media to include in tweet.</param>
        /// <param name="autoPopulateReplyMetadata">Enables extended tweet mode where mentions don't count towards tweet length.</param>
        /// <param name="excludeReplyUserIds">Comma-separated list of @mentions to exclude from extended tweet prefix list.</param>
        /// <param name="attachmentUrl">Tweet link or DM deep link for extended tweet suffix that doesn't count towards tweet length.</param>
        /// <param name="tweetMode">Set to Extended for 280 characters (Text is blank and FullText contains tweet.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Tweeted status.</returns>
        internal virtual async Task<Status> TweetOrReplyAsync(ulong tweetID, string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, bool trimUser, IEnumerable<ulong> mediaIds, bool autoPopulateReplyMetadata, IEnumerable<ulong> excludeReplyUserIds, string attachmentUrl, TweetMode tweetMode = TweetMode.Compat, CancellationToken cancelToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(status) && (mediaIds == null || !mediaIds.Any()))
                throw new ArgumentException("status is a required parameter.", "status");

            var updateUrl = BaseUrl + "statuses/update.json";

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<Status>(
                    HttpMethod.Post.ToString(),
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"in_reply_to_status_id", tweetID == NoReply ? null : tweetID.ToString()},
                        {"lat", latitude == NoCoordinate ? null : latitude.ToString(Culture.US)},
                        {"long", longitude == NoCoordinate ? null : longitude.ToString(Culture.US)},
                        {"place_id", placeID == NoInputParam ? null : placeID },
                        {"display_coordinates", displayCoordinates ? displayCoordinates.ToString().ToLower() : null},
                        {"trim_user", trimUser ? trimUser.ToString().ToLower() : null },
                        {"media_ids", mediaIds == null || !mediaIds.Any() ? null : string.Join(",", mediaIds) },
                        {"auto_populate_reply_metadata", autoPopulateReplyMetadata ? autoPopulateReplyMetadata.ToString().ToLower() : null },
                        {"exclude_reply_user_ids", excludeReplyUserIds == null || !excludeReplyUserIds.Any() ? null : string.Join(",", excludeReplyUserIds) },
                        {"attachment_url", attachmentUrl },
                        {"tweet_mode", tweetMode == TweetMode.Compat ? null : tweetMode.ToString().ToLower() }
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return new StatusRequestProcessor<Status>()
                .ProcessActionResult(RawResult, StatusAction.SingleStatus);
        }

        /// <summary>
        /// Deletes a tweet.
        /// </summary>
        /// <param name="tweetID">ID of tweet to delete.</param>
        /// <returns>Deleted tweet.</returns>
        public virtual async Task<Status> DeleteTweetAsync(ulong tweetID, CancellationToken cancelToken = default(CancellationToken))
        {
            if (tweetID == MissingID)
                throw new ArgumentException("0 is *not* a valid tweetID. You must provide the ID of the tweet you're deleting.", "tweetID");

            var destroyUrl = BaseUrl + "statuses/destroy/" + tweetID + ".json";

            RawResult = await TwitterExecutor
                .PostFormUrlEncodedToTwitterAsync<Status>(HttpMethod.Post.ToString(), destroyUrl, new Dictionary<string, string>(), cancelToken)
                .ConfigureAwait(false);

            return new StatusRequestProcessor<Status>()
                .ProcessActionResult(RawResult, StatusAction.SingleStatus);
        }

        /// <summary>
        /// Retweets a tweet.
        /// </summary>
        /// <param name="tweetID">ID of tweet being retweeted.</param>
        /// <returns>Retweeted tweet.</returns>
        public virtual async Task<Status> RetweetAsync(ulong tweetID, TweetMode tweetMode = TweetMode.Compat, CancellationToken cancelToken = default(CancellationToken))
        {
            if (tweetID == MissingID)
                throw new ArgumentException("0 is *not* a valid tweetID. You must provide the ID of the tweet you're retweeting.", "tweetID");

            var retweetUrl = BaseUrl + "statuses/retweet/" + tweetID + ".json?tweet_mode=" + tweetMode.ToString().ToLower();

            RawResult = await TwitterExecutor
                .PostFormUrlEncodedToTwitterAsync<Status>(HttpMethod.Post.ToString(), retweetUrl, new Dictionary<string, string>(), cancelToken)
                .ConfigureAwait(false);

            return new StatusRequestProcessor<Status>()
                .ProcessActionResult(RawResult, StatusAction.SingleStatus);
        }
    }
}
