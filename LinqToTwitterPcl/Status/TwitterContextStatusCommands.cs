using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        public const ulong MissingID = 0ul;
        public const ulong NoReply = 0ul;
        public const decimal NoCoordinate = -1m;

        ///// <summary>
        ///// sends a status update with attached media
        ///// </summary>
        ///// <param name="status">(optional @UserName) and (required) status text</param>
        ///// <param name="possiblySensitive">Set to true if media does not contain age appropriate content</param>
        ///// <param name="mediaItems">List of Media to send</param>
        ///// <returns>Status containing new tweet</returns>
        //public async Task<Status> TweetWithMedia(string status, bool possiblySensitive, List<Media> mediaItems)
        //{
        //    Status results = ReplyWithMedia(NoReply, status, possiblySensitive, NoCoordinate, NoCoordinate, null, false, mediaItems, null);
        //    return results;
        //}

        ///// <summary>
        ///// sends a status update with attached media
        ///// </summary>
        ///// <param name="status">(optional @UserName) and (required) status text</param>
        ///// <param name="possiblySensitive">Set to true if media does not contain age appropriate content</param>
        ///// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        ///// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        ///// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        ///// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        ///// <param name="mediaItems">List of Media to send</param>
        ///// <param name="callback">Async callback handler</param>
        ///// <returns>Status containing new tweet</returns>
        //public async Task<Status> TweetWithMedia(string status, bool possiblySensitive, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, List<Media> mediaItems, Action<TwitterAsyncResponse<Status>> callback)
        //{
        //    Status results = ReplyWithMedia(NoReply, status, possiblySensitive, latitude, longitude, placeID, displayCoordinates, mediaItems, callback);
        //    return results;
        //}

        ///// <summary>
        ///// sends a status update with attached media
        ///// </summary>
        ///// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        ///// <param name="status">(optional @UserName) and (required) status text</param>
        ///// <param name="possiblySensitive">Set to true if media does not contain age appropriate content</param>
        ///// <param name="mediaItems">List of Media to send</param>
        ///// <returns>Status containing new reply</returns>
        //public async Task<Status> ReplyWithMedia(ulong inReplyToStatusID, string status, bool possiblySensitive, List<Media> mediaItems)
        //{
        //    Status result = ReplyWithMedia(inReplyToStatusID, status, possiblySensitive, NoCoordinate, NoCoordinate, null, false, mediaItems, null);
        //    return result;
        //}

        ///// <summary>
        ///// sends a status update with attached media
        ///// </summary>
        ///// <param name="twitterCtx">Your instance of TwitterContext</param>
        ///// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        ///// <param name="status">(optional @UserName) and (required) status text</param>
        ///// <param name="possiblySensitive">Set to true if media does not contain age appropriate content</param>
        ///// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        ///// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        ///// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        ///// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        ///// <param name="mediaItems">List of Media to send</param>
        ///// <param name="callback">Async callback handler</param>
        ///// <returns>Status containing new reply</returns>
        //public async Task<Status> ReplyWithMedia(ulong inReplyToStatusID, string status, bool possiblySensitive, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, List<Media> mediaItems, Action<TwitterAsyncResponse<Status>> callback)
        //{
        //    if (string.IsNullOrEmpty(status))
        //    {
        //        throw new ArgumentNullException("status", "status is a required parameter.");
        //    }

        //    if (mediaItems == null)
        //    {
        //        throw new ArgumentNullException("mediaItems", "You must pass at least one Media in mediaItems.");
        //    }

        //    if (mediaItems.Count == 0)
        //    {
        //        throw new ArgumentException("You must pass at least one Media in mediaItems.", "mediaItems");
        //    }

        //    TwitterExecutor.AsyncCallback = callback;

        //    var updateUrl = BaseUrl + "statuses/update_with_media.json";

        //    var reqProc = new StatusRequestProcessor<Status>();

        //    string resultString =
        //        TwitterExecutor.PostMedia(
        //            updateUrl,
        //            new Dictionary<string, string>
        //            {
        //                {"status", status},
        //                {"possibly_sensitive", possiblySensitive ? true.ToString() : null },
        //                {"lat", latitude == NoCoordinate ? null : latitude.ToString(Culture.US) },
        //                {"long", longitude == NoCoordinate ? null : longitude.ToString(Culture.US) },
        //                {"place_id", string.IsNullOrEmpty(placeID) ? null : placeID },
        //                {"display_coordinates", displayCoordinates ? true.ToString() : null },
        //                {"in_reply_to_status_id", inReplyToStatusID == NoReply ? null : inReplyToStatusID.ToString(CultureInfo.InvariantCulture)}
        //            },
        //            mediaItems,
        //            reqProc);

        //    Status result = reqProc.ProcessActionResult(resultString, StatusAction.SingleStatus);
        //    return result;
        //}

        /// <summary>
        /// Replies to a tweet.
        /// </summary>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <returns>Reply status.</returns>
        public async Task<Status> ReplyAsync(ulong tweetID, string status)
        {
            return await ReplyAsync(tweetID, status, NoCoordinate, NoCoordinate, null, false, false);
        }

        /// <summary>
        /// Replies to a tweet with coordinates.
        /// </summary>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <returns>Reply status.</returns>
        public async Task<Status> ReplyAsync(ulong tweetID, string status, decimal latitude, decimal longitude, bool displayCoordinates)
        {
            return await ReplyAsync(tweetID, status, latitude, longitude, null, displayCoordinates, false);
        }

        /// <summary>
        /// Replies to a tweet with.
        /// </summary>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <returns>Reply status.</returns>
        public async Task<Status> ReplyAsync(ulong tweetID, string status, decimal latitude, decimal longitude, string placeID, bool trimUser)
        {
            return await ReplyAsync(tweetID, status, latitude, longitude, placeID, false, trimUser);
        }

        /// <summary>
        /// Replies to a tweet.
        /// </summary>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <returns>Reply status.</returns>
        public async Task<Status> ReplyAsync(ulong tweetID, string status, string placeID, bool displayCoordinates, bool trimUser)
        {
            return await ReplyAsync(tweetID, status, NoCoordinate, NoCoordinate, placeID, displayCoordinates, trimUser);
        }

        /// <summary>
        /// Replies to a tweet.
        /// </summary>
        /// <param name="tweetID">ID (aka StatusID) of tweet to reply to.</param>
        /// <param name="status">Reply status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <returns>Reply status.</returns>
        public async Task<Status> ReplyAsync(ulong tweetID, string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, bool trimUser)
        {
            if (tweetID == MissingID)
                throw new ArgumentException("0 is *not* a valid tweetID. You must provide the ID of the tweet you're replying to.", "tweetID");

            return await TweetOrReplyAsync(tweetID, status, latitude, longitude, placeID, displayCoordinates, trimUser);
        }

        /// <summary>
        /// Sends a status update.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <returns>Tweeted status.</returns>
        public async Task<Status> TweetAsync(string status)
        {
            return await TweetOrReplyAsync(NoReply, status, NoCoordinate, NoCoordinate, null, false, false);
        }

        /// <summary>
        /// Sends a status update.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <returns>Tweeted status.</returns>
        public async Task<Status> TweetAsync(string status, decimal latitude, decimal longitude)
        {
            return await TweetOrReplyAsync(NoReply, status, latitude, longitude, null, false, false);
        }

        /// <summary>
        /// Sends a status update.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <returns>Tweeted status.</returns>
        public async Task<Status> TweetAsync(string status, decimal latitude, decimal longitude, bool displayCoordinates)
        {
            return await TweetOrReplyAsync(NoReply, status, latitude, longitude, null, displayCoordinates, false);
        }

        /// <summary>
        /// Sends a status update.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <returns>Tweeted status.</returns>
        public async Task<Status> TweetAsync(string status, decimal latitude, decimal longitude, string placeID, bool trimUser)
        {
            return await TweetOrReplyAsync(NoReply, status, latitude, longitude, placeID, false, trimUser);
        }

        /// <summary>
        /// Sends a status update.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred.</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <returns>Tweeted status.</returns>
        public async Task<Status> TweetAsync(string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, bool trimUser)
        {
            return await TweetOrReplyAsync(NoReply, status, latitude, longitude, placeID, displayCoordinates, trimUser);
        }

        /// <summary>
        /// Sends a status update.
        /// </summary>
        /// <param name="status">Status text.</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query).</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet.</param>
        /// <returns>Tweeted status.</returns>
        public async Task<Status> TweetAsync(string status, string placeID, bool displayCoordinates, bool trimUser)
        {
            return await TweetOrReplyAsync(NoReply, status, NoCoordinate, NoCoordinate, placeID, displayCoordinates, trimUser);
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
        /// <returns>Tweeted status.</returns>
        internal async Task<Status> TweetOrReplyAsync(ulong tweetID, string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, bool trimUser)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("status is a required parameter.", "status");

            var updateUrl = BaseUrl + "statuses/update.json";

            string resultsJson =
                await TwitterExecutor.PostToTwitterAsync<Status>(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"in_reply_to_status_id", tweetID == NoReply ? null : tweetID.ToString()},
                        {"lat", latitude == NoCoordinate ? null : latitude.ToString(Culture.US)},
                        {"long", longitude == NoCoordinate ? null : longitude.ToString(Culture.US)},
                        {"place_id", placeID},
                        {"display_coordinates", displayCoordinates ? displayCoordinates.ToString().ToLower() : null},
                        {"trim_user", trimUser ? trimUser.ToString().ToLower() : null }
                    });

            return new StatusRequestProcessor<Status>()
                .ProcessActionResult(resultsJson, StatusAction.SingleStatus);
        }

        /// <summary>
        /// Deletes a tweet.
        /// </summary>
        /// <param name="tweetID">ID of tweet to delete.</param>
        /// <returns>Deleted tweet.</returns>
        public async Task<Status> DeleteTweetAsync(ulong tweetID)
        {
            if (tweetID == MissingID)
                throw new ArgumentException("0 is *not* a valid tweetID. You must provide the ID of the tweet you're deleting.", "tweetID");

            var destroyUrl = BaseUrl + "statuses/destroy/" + tweetID + ".json";

            string resultsJson = await TwitterExecutor
                .PostToTwitterAsync<Status>(destroyUrl, new Dictionary<string, string>());

            return new StatusRequestProcessor<Status>()
                .ProcessActionResult(resultsJson, StatusAction.SingleStatus);
        }

        /// <summary>
        /// Retweets a tweet.
        /// </summary>
        /// <param name="tweetID">ID of tweet being retweeted.</param>
        /// <returns>Retweeted tweet.</returns>
        public async Task<Status> RetweetAsync(ulong tweetID)
        {
            if (tweetID == MissingID)
                throw new ArgumentException("0 is *not* a valid tweetID. You must provide the ID of the tweet you're retweeting.", "tweetID");

            var retweetUrl = BaseUrl + "statuses/retweet/" + tweetID + ".json";

            string resultsJson = await TwitterExecutor
                .PostToTwitterAsync<Status>(retweetUrl, new Dictionary<string, string>());

            return new StatusRequestProcessor<Status>()
                .ProcessActionResult(resultsJson, StatusAction.SingleStatus);
        }
    }
}
