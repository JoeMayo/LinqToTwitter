using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
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
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status)
        {
            return await UpdateStatusAsync(status, -1, -1, null, false, null, false, null);
        }

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, Action<TwitterAsyncResponse<Status>> callback)
        {
            return await UpdateStatusAsync(status, -1, -1, null, false, null, false, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, string inReplyToStatusID)
        {
            return await UpdateStatusAsync(status, -1, -1, null, false, inReplyToStatusID, false, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return await UpdateStatusAsync(status, -1, -1, null, false, inReplyToStatusID, false, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude)
        {
            return await UpdateStatusAsync(status, latitude, longitude, null, false, null, false, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, Action<TwitterAsyncResponse<Status>> callback)
        {
            return await UpdateStatusAsync(status, latitude, longitude, null, false, null, false, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, bool displayCoordinates)
        {
            return await UpdateStatusAsync(status, latitude, longitude, null, displayCoordinates, null, false, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return await UpdateStatusAsync(status, latitude, longitude, null, displayCoordinates, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, bool displayCoordinates, string inReplyToStatusID)
        {
            return await UpdateStatusAsync(status, latitude, longitude, null, displayCoordinates, inReplyToStatusID, false, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return await UpdateStatusAsync(status, latitude, longitude, null, displayCoordinates, inReplyToStatusID, false, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, string placeID, bool trimUser)
        {
            return await UpdateStatusAsync(status, latitude, longitude, placeID, false, null, trimUser, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, string placeID, bool trimUser, Action<TwitterAsyncResponse<Status>> callback)
        {
            return await UpdateStatusAsync(status, latitude, longitude, placeID, false, null, trimUser, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, string placeID, string inReplyToStatusID, bool trimUser)
        {
            return await UpdateStatusAsync(status, latitude, longitude, placeID, false, inReplyToStatusID, trimUser, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, string placeID, string inReplyToStatusID, bool trimUser, Action<TwitterAsyncResponse<Status>> callback)
        {
            return await UpdateStatusAsync(status, latitude, longitude, placeID, false, inReplyToStatusID, trimUser, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, bool trimUser)
        {
            return await UpdateStatusAsync(status, latitude, longitude, placeID, displayCoordinates, null, trimUser, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, bool trimUser, Action<TwitterAsyncResponse<Status>> callback)
        {
            return await UpdateStatusAsync(status, latitude, longitude, placeID, displayCoordinates, null, trimUser, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, string placeID, bool displayCoordinates, bool trimUser)
        {
            return await UpdateStatusAsync(status, -1, -1, placeID, displayCoordinates, null, trimUser, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, string placeID, bool displayCoordinates, bool trimUser, Action<TwitterAsyncResponse<Status>> callback)
        {
            return await UpdateStatusAsync(status, -1, -1, placeID, displayCoordinates, null, trimUser, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, string placeID, bool displayCoordinates, string inReplyToStatusID, bool trimUser)
        {
            return await UpdateStatusAsync(status, -1, -1, placeID, displayCoordinates, inReplyToStatusID, trimUser, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, string placeID, bool displayCoordinates, string inReplyToStatusID, bool trimUser, Action<TwitterAsyncResponse<Status>> callback)
        {
            return await UpdateStatusAsync(status, -1, -1, placeID, displayCoordinates, inReplyToStatusID, trimUser, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, string inReplyToStatusID, bool trimUser)
        {
            return await UpdateStatusAsync(status, latitude, longitude, placeID, displayCoordinates, inReplyToStatusID, trimUser, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public async Task<Status> UpdateStatusAsync(string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, string inReplyToStatusID, bool trimUser, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("status is a required parameter.", "status");
            }

            var updateUrl = BaseUrl + "statuses/update.json";

            var reqProc = new StatusRequestProcessor<Status>();

            ITwitterExecute exec = TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                await exec.PostToTwitterAsync(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"in_reply_to_status_id", inReplyToStatusID},
                        {"lat", latitude == -1 ? null : latitude.ToString(Culture.US)},
                        {"long", longitude == -1 ? null : longitude.ToString(Culture.US)},
                        {"place_id", placeID},
                        {"display_coordinates", displayCoordinates ? displayCoordinates.ToString().ToLower() : null},
                        {"trim_user", trimUser ? trimUser.ToString().ToLower() : null }
                    },
                    response => reqProc.ProcessActionResult(response, StatusAction.SingleStatus));

            Status result = reqProc.ProcessActionResult(resultsJson, StatusAction.SingleStatus);
            return result;
        }

        /// <summary>
        /// deletes a status tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <returns>deleted status tweet</returns>
        public async Task<Status> DestroyStatusAsync(string id)
        {
            return await DestroyStatusAsync(id, null);
        }

        /// <summary>
        /// deletes a status tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>deleted status tweet</returns>
        public async Task<Status> DestroyStatusAsync(string id, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "statuses/destroy/" + id + ".json";

            var reqProc = new StatusRequestProcessor<Status>();

            ITwitterExecute exec = TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                await exec.PostToTwitterAsync(
                    destroyUrl,
                    new Dictionary<string, string>(),
                    response => reqProc.ProcessActionResult(response, StatusAction.SingleStatus));

            Status result = reqProc.ProcessActionResult(resultsJson, StatusAction.SingleStatus);
            return result;
        }

        /// <summary>
        /// retweets a tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <returns>deleted status tweet</returns>
        public async Task<Status> RetweetAsync(string id)
        {
            return await RetweetAsync(id, null);
        }

        /// <summary>
        /// retweets a tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>deleted status tweet</returns>
        public async Task<Status> RetweetAsync(string id, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var retweetUrl = BaseUrl + "statuses/retweet/" + id + ".json";

            var reqProc = new StatusRequestProcessor<Status>();

            ITwitterExecute exec = TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                await exec.PostToTwitterAsync(
                    retweetUrl,
                    new Dictionary<string, string>(),
                    response => reqProc.ProcessActionResult(response, StatusAction.SingleStatus));

            Status result = reqProc.ProcessActionResult(resultsJson, StatusAction.SingleStatus);
            return result;
        }
    }
}
