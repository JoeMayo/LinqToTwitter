using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Sends a new direct message to specified user.
        /// </summary>
        /// <param name="recipientID">ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <param name="mediaId">ID of an uploaded media.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<DirectMessageEvents> NewDirectMessageEventAsync(ulong recipientID, string text, ulong mediaId, CancellationToken cancelToken = default(CancellationToken))
        {
            var attachment = new Attachment
            {
                Type = "media",
                Media = new DirectMessageMedia
                {
                    Id = mediaId
                }
            };

            return await NewDirectMessageEventAsync(recipientID, text, attachment);
        }

        /// <summary>
        /// Sends a new direct message to specified user.
        /// </summary>
        /// <param name="recipientID">ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <param name="latitude">Latitude coordinate.</param>
        /// <param name="longitude">Longitude coordinate.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<DirectMessageEvents> NewDirectMessageEventAsync(ulong recipientID, string text, double latitude, double longitude, CancellationToken cancelToken = default(CancellationToken))
        {
            var attachment = new Attachment
            {
                Type = "location",
                Location = new DirectMessageEventLocation
                {
                    Type = "shared_coordinate",
                    SharedCoordinate = new SharedCoordinate
                    {
                        Coordinates = new DirectMessageEventCoordinates
                        {
                            Type = "Point",
                            Coordinates = new double[] { latitude, longitude }
                        }
                    }
                }
            };

            return await NewDirectMessageEventAsync(recipientID, text, attachment);
        }

        /// <summary>
        /// Sends a new direct message to specified user.
        /// </summary>
        /// <param name="recipientID">ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <param name="placeID">ID of place (Tip: Use the Geo API if you don't already have a place ID)</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<DirectMessageEvents> NewDirectMessageEventAsync(ulong recipientID, string text, string placeID, CancellationToken cancelToken = default(CancellationToken))
        {
            var attachment = new Attachment
            {
                Type = "location",
                Location = new DirectMessageEventLocation
                {
                    Type = "shared_place",
                    SharedPlace = new SharedPlace
                    {
                        Place = new DirectMessageEventPlace
                        {
                            Id = placeID
                        }
                    }
                }
            };

            return await NewDirectMessageEventAsync(recipientID, text, attachment);
        }

        /// <summary>
        /// Sends a new direct message to specified user.
        /// </summary>
        /// <param name="recipientID">ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<DirectMessageEvents> NewDirectMessageEventAsync(ulong recipientID, string text, CancellationToken cancelToken = default(CancellationToken))
        {
            return await NewDirectMessageEventAsync(recipientID, text, attachment: null);
        }

        /// <summary>
        /// Sends a new direct message to specified user.
        /// </summary>
        /// <param name="recipientID">ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <param name="attachment">Optional attachment from overloads that support it.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        private async Task<DirectMessageEvents> NewDirectMessageEventAsync(ulong recipientID, string text, Attachment attachment = null, CancellationToken cancelToken = default(CancellationToken))
        {
            if (recipientID == default(ulong))
                throw new ArgumentException($"{nameof(recipientID)} must be set.", nameof(recipientID));
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException($"{nameof(text)} is a required parameter.", nameof(text));

            var newUrl = BaseUrl + "direct_messages/events/new.json";

            var dmEventParams = new DirectMessageEventsValue
            {
                DMEvent = new DMEvent
                {
                    MessageCreate = new DirectMessageCreate
                    {
                        MessageData = new DirectMessageData
                        {
                            Text = text,
                            Attachment = attachment
                        },
                        Target = new DirectMessageTarget
                        {
                            RecipientID = recipientID.ToString()
                        }
                    },
                    Type = "message_create"
                }
            };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Post.ToString(),
                    newUrl,
                    new Dictionary<string, string>(),
                    dmEventParams,
                    cancelToken)
                   .ConfigureAwait(false);

            var reqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents>();
            DirectMessageEvents dmEvents = reqProc.ProcessActionResult(RawResult, DirectMessageEventsType.Show);

            return dmEvents;
        }

        /// <summary>
        /// Deletes a direct message.
        /// </summary>
        /// <param name="directMessageID">ID of direct message to delete.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        public async Task DeleteDirectMessageEventAsync(ulong directMessageID, CancellationToken cancelToken = default(CancellationToken))
        {
            if (directMessageID == default(ulong))
                throw new ArgumentException($"{nameof(directMessageID)} must be set.", nameof(directMessageID));

            var newUrl = BaseUrl + "direct_messages/events/destroy.json?id=" + directMessageID;

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<DirectMessageEvents>(
                    HttpMethod.Delete.ToString(),
                    newUrl,
                    new Dictionary<string, string>
                    {
                        ["id"] = directMessageID.ToString()
                    },
                    cancelToken)
                   .ConfigureAwait(false);
        }
    }
}
