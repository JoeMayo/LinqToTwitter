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
            else if (mediaItems.Count == 0)
            {
                throw new ArgumentException("You must pass at least one Media in mediaItems.", "mediaItems");
            }

            twitterCtx.TwitterExecutor.AsyncCallback = callback;

            var updateUrl = twitterCtx.UploadUrl + "statuses/update_with_media.xml";

            IRequestProcessor<Status> reqProc = twitterCtx.CreateRequestProcessor<Status>();

            string resultString =
                twitterCtx.TwitterExecutor.PostMedia(
                    updateUrl,
                    new Dictionary<string, string>
                    {
                        {"status", status},
                        {"possibly_sensitive", possiblySensitive ? possiblySensitive.ToString() : null },
                        {"lat", latitude == NoCoordinate ? null : latitude.ToString(CultureInfo.InvariantCulture) },
                        {"long", longitude == NoCoordinate ? null : longitude.ToString(CultureInfo.InvariantCulture) },
                        {"place_id", string.IsNullOrEmpty(placeID) ? null : placeID },
                        {"display_coordinates", displayCoordinates ? displayCoordinates.ToString() : null },
                        {"in_reply_to_status_id", inReplyToStatusID == NoReply ? null : inReplyToStatusID.ToString()}
                    },
                    mediaItems,
                    reqProc);

            List<Status> results = reqProc.ProcessResults(resultString);
            return results.FirstOrDefault();
        }
    }
}
