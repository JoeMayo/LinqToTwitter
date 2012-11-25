using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LinqToTwitter
{
    public static class StatusExtensions
    {
        public const ulong NoReply = 0ul;
        public const decimal NoCoordinate = -1m;

        /// <summary>
        /// sends a status update with attached media
        /// </summary>
        /// <param name="twitterCtx">Your instance of TwitterContext</param>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="possiblySensitive">Set to true if media does not contain age appropriate content</param>
        /// <param name="mediaItems">List of Media to send</param>
        /// <returns>Status containing new tweet</returns>
        public static Status TweetWithMedia(this TwitterContext twitterCtx, string status, bool possiblySensitive, List<Media> mediaItems)
        {
            Status results = ReplyWithMedia(twitterCtx, NoReply, status, possiblySensitive, NoCoordinate, NoCoordinate, null, false, mediaItems, null);
            return results;
        }

        /// <summary>
        /// sends a status update with attached media
        /// </summary>
        /// <param name="twitterCtx">Your instance of TwitterContext</param>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="possiblySensitive">Set to true if media does not contain age appropriate content</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="mediaItems">List of Media to send</param>
        /// <param name="callback">Async callback handler</param>
        /// <returns>Status containing new tweet</returns>
        public static Status TweetWithMedia(this TwitterContext twitterCtx, string status, bool possiblySensitive, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, List<Media> mediaItems, Action<TwitterAsyncResponse<Status>> callback)
        {
            Status results = ReplyWithMedia(twitterCtx, NoReply, status, possiblySensitive, latitude, longitude, placeID, displayCoordinates, mediaItems, callback);
            return results;
        }

        /// <summary>
        /// sends a status update with attached media
        /// </summary>
        /// <param name="twitterCtx">Your instance of TwitterContext</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="possiblySensitive">Set to true if media does not contain age appropriate content</param>
        /// <param name="mediaItems">List of Media to send</param>
        /// <returns>Status containing new reply</returns>
        public static Status ReplyWithMedia(this TwitterContext twitterCtx, ulong inReplyToStatusID, string status, bool possiblySensitive, List<Media> mediaItems)
        {
            Status result = ReplyWithMedia(twitterCtx, inReplyToStatusID, status, possiblySensitive, NoCoordinate, NoCoordinate, null, false, mediaItems, null);
            return result;
        }

        /// <summary>
        /// sends a status update with attached media
        /// </summary>
        /// <param name="twitterCtx">Your instance of TwitterContext</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="possiblySensitive">Set to true if media does not contain age appropriate content</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="mediaItems">List of Media to send</param>
        /// <param name="callback">Async callback handler</param>
        /// <returns>Status containing new reply</returns>
        public static Status ReplyWithMedia(this TwitterContext twitterCtx, ulong inReplyToStatusID, string status, bool possiblySensitive, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, List<Media> mediaItems, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentNullException("status", "status is a required parameter.");
            }

            if (mediaItems == null)
            {
                throw new ArgumentNullException("mediaItems", "You must pass at least one Media in mediaItems.");
            }

            if (mediaItems.Count == 0)
            {
                throw new ArgumentException("You must pass at least one Media in mediaItems.", "mediaItems");
            }

            twitterCtx.TwitterExecutor.AsyncCallback = callback;

            var updateUrl = twitterCtx.BaseUrl + "statuses/update_with_media.json";

            var reqProc = new StatusRequestProcessor<Status>();

            string resultString =
                twitterCtx.TwitterExecutor.PostMedia(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"possibly_sensitive", possiblySensitive ? true.ToString() : null },
                        {"lat", latitude == NoCoordinate ? null : latitude.ToString(Culture.US) },
                        {"long", longitude == NoCoordinate ? null : longitude.ToString(Culture.US) },
                        {"place_id", string.IsNullOrEmpty(placeID) ? null : placeID },
                        {"display_coordinates", displayCoordinates ? true.ToString() : null },
                        {"in_reply_to_status_id", inReplyToStatusID == NoReply ? null : inReplyToStatusID.ToString(CultureInfo.InvariantCulture)}
                    },
                    mediaItems,
                    reqProc);

            Status result = reqProc.ProcessActionResult(resultString, StatusAction.SingleStatus);
            return result;
        }

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status)
        {
            return UpdateStatus(ctx, status, false, -1, -1, null, false, null, null);
        }

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks)
        {
            return UpdateStatus(ctx, status, wrapLinks, -1, -1, null, false, null, null);
        }

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, false, -1, -1, null, false, null, callback);
        }

        /// <summary>
        /// sends a status update - overload to make inReplyToStatusID optional
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, wrapLinks, -1, -1, null, false, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, string inReplyToStatusID)
        {
            return UpdateStatus(ctx, status, false, -1, -1, null, false, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, string inReplyToStatusID)
        {
            return UpdateStatus(ctx, status, wrapLinks, -1, -1, null, false, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, false, -1, -1, null, false, inReplyToStatusID, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, wrapLinks, -1, -1, null, false, inReplyToStatusID, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, null, false, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, null, false, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, null, false, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, null, false, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, bool displayCoordinates)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, null, displayCoordinates, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, bool displayCoordinates)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, null, displayCoordinates, null, null);
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
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, null, displayCoordinates, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, null, displayCoordinates, callback);
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
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, null, displayCoordinates, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, null, displayCoordinates, inReplyToStatusID, null);
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
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, null, displayCoordinates, inReplyToStatusID, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, null, displayCoordinates, inReplyToStatusID, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, string placeID)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, placeID, false, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, placeID, false, null, null);
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
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, string placeID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, placeID, false, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, placeID, false, null, callback);
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
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, string placeID, string inReplyToStatusID)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, placeID, false, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, string inReplyToStatusID)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, placeID, false, inReplyToStatusID, null);
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
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, string placeID, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, placeID, false, inReplyToStatusID, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, placeID, false, inReplyToStatusID, callback);
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
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, placeID, displayCoordinates, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, bool displayCoordinates)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, placeID, displayCoordinates, null, null);
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
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, placeID, displayCoordinates, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, placeID, displayCoordinates, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, string placeID, bool displayCoordinates)
        {
            return UpdateStatus(ctx, status, false, -1, -1, placeID, displayCoordinates, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, string placeID, bool displayCoordinates)
        {
            return UpdateStatus(ctx, status, wrapLinks, -1, -1, placeID, displayCoordinates, null, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, string placeID, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, false, -1, -1, placeID, displayCoordinates, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, string placeID, bool displayCoordinates, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, wrapLinks, -1, -1, placeID, displayCoordinates, null, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, string placeID, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(ctx, status, false, -1, -1, placeID, displayCoordinates, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, string placeID, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(ctx, status, wrapLinks, -1, -1, placeID, displayCoordinates, inReplyToStatusID, null);
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
        public static Status UpdateStatus(this TwitterContext ctx, string status, string placeID, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, false, -1, -1, placeID, displayCoordinates, inReplyToStatusID, callback);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, string placeID, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            return UpdateStatus(ctx, status, wrapLinks, -1, -1, placeID, displayCoordinates, inReplyToStatusID, callback);
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
        public static Status UpdateStatus(this TwitterContext ctx, string status, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(ctx, status, false, latitude, longitude, placeID, displayCoordinates, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, string inReplyToStatusID)
        {
            return UpdateStatus(ctx, status, wrapLinks, latitude, longitude, placeID, displayCoordinates, inReplyToStatusID, null);
        }

        /// <summary>
        /// sends a status update
        /// </summary>
        /// <param name="status">(optional @UserName) and (required) status text</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="latitude">Latitude coordinate of where tweet occurred</param>
        /// <param name="longitude">Longitude coordinate of where tweet occurred</param>
        /// <param name="placeID">ID of place (found via Geo Reverse lookup query)</param>
        /// <param name="displayCoordinates">Allow or prevent display of coordinates for this tweet</param>
        /// <param name="inReplyToStatusID">id of status replying to - optional - pass null if not used</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>IQueryable of sent status</returns>
        public static Status UpdateStatus(this TwitterContext ctx, string status, bool wrapLinks, decimal latitude, decimal longitude, string placeID, bool displayCoordinates, string inReplyToStatusID, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("status is a required parameter.", "status");
            }

            var updateUrl = ctx.BaseUrl + "statuses/update.json";

            var reqProc = new StatusRequestProcessor<Status>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"in_reply_to_status_id", inReplyToStatusID},
                        {"lat", latitude == -1 ? null : latitude.ToString(Culture.US)},
                        {"long", longitude == -1 ? null : longitude.ToString(Culture.US)},
                        {"place_id", placeID},
                        {"display_coordinates", displayCoordinates.ToString()},
                        {"wrap_links", wrapLinks ? true.ToString() : null }
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
        public static Status DestroyStatus(this TwitterContext ctx, string id)
        {
            return DestroyStatus(ctx, id, null);
        }

        /// <summary>
        /// deletes a status tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>deleted status tweet</returns>
        public static Status DestroyStatus(this TwitterContext ctx, string id, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = ctx.BaseUrl + "statuses/destroy/" + id + ".json";

            var reqProc = new StatusRequestProcessor<Status>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
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
        public static Status Retweet(this TwitterContext ctx, string id)
        {
            return Retweet(ctx, id, null);
        }

        /// <summary>
        /// retweets a tweet
        /// </summary>
        /// <param name="id">id of status tweet</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>deleted status tweet</returns>
        public static Status Retweet(this TwitterContext ctx, string id, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var retweetUrl = ctx.BaseUrl + "statuses/retweet/" + id + ".json";

            var reqProc = new StatusRequestProcessor<Status>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.PostToTwitter(
                    retweetUrl,
                    new Dictionary<string, string>(),
                    response => reqProc.ProcessActionResult(response, StatusAction.SingleStatus));

            Status result = reqProc.ProcessActionResult(resultsJson, StatusAction.SingleStatus);
            return result;
        }

    }
}
