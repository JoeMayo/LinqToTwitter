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
        /// Sends a new direct message that attaches media.
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
        /// Sends a new direct message with lat/long location.
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
        /// Sends a new direct message with place ID location.
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
        /// Sends a new direct message to specified user with just text.
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
        /// Sends a new direct message quick reply.
        /// </summary>
        /// <param name="recipientID">ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <param name="externalId">Unique ID for you to correlate request with response</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<DirectMessageEvents> RequestQuickReplyLocationAsync(ulong recipientID, string text, string externalId, CancellationToken cancelToken = default(CancellationToken))
        {
            var quickReply = new QuickReply
            {
                Type = "location",
                Location = new QuickReplyLocation
                {
                    Metadata = externalId
                }
            };

            return await NewDirectMessageEventAsync(recipientID, text, attachment: null, quickReply: quickReply);
        }

        /// <summary>
        /// Sends a new direct message with options.
        /// </summary>
        /// <param name="recipientID">ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <param name="options">List of options for the user to choose from.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<DirectMessageEvents> RequestQuickReplyOptionsAsync(ulong recipientID, string text, IEnumerable<QuickReplyOption> options, CancellationToken cancelToken = default(CancellationToken))
        {
            var quickReply = new QuickReply
            {
                Type = "options",
                Options = options
            };

            return await NewDirectMessageEventAsync(recipientID, text, attachment: null, quickReply: quickReply);
        }

        /// <summary>
        /// Sends a new direct message for text input.
        /// </summary>
        /// <param name="recipientID">ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <param name="keyboard">User keyboard - "default" for normal text or "number" for numeric.</param>
        /// <param name="label">Input label.</param>
        /// <param name="metadata">Unique ID for you to correlate request with response</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<DirectMessageEvents> RequestQuickReplyTextInputAsync(ulong recipientID, string text, string keyboard, string label, string metadata, CancellationToken cancelToken = default(CancellationToken))
        {
            var quickReply = new QuickReply
            {
                Type = "text_input",
                TextInput = new TextInput
                {
                    Keyboard = keyboard ?? "default",
                    Label = label,
                    Metadata = metadata
                }
            };

            return await NewDirectMessageEventAsync(recipientID, text, attachment: null, quickReply: quickReply);
        }

        /// <summary>
        /// Sends a new direct message with buttons.
        /// </summary>
        /// <param name="recipientID">ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <param name="callToActions">List of Call to Action, which creates buttons in the message.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<DirectMessageEvents> RequestButtonChoiceAsync(ulong recipientID, string text, IEnumerable<CallToAction> callToActions, CancellationToken cancelToken = default(CancellationToken))
        {
            return await NewDirectMessageEventAsync(recipientID, text, attachment: null, quickReply: null, callToActions: callToActions);
        }

        /// <summary>
        /// Sends a new direct message to specified user.
        /// </summary>
        /// <param name="recipientID">ID of user to send to.</param>
        /// <param name="text">Direct message contents.</param>
        /// <param name="attachment">Optional attachment from overloads that support it.</param>
        /// <param name="quickReply">Quick reply from overloads that support it.</param>
        /// <param name="callToActions">List of Call to Action, which creates buttons in the message.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        async Task<DirectMessageEvents> NewDirectMessageEventAsync(ulong recipientID, string text, Attachment attachment = null, QuickReply quickReply = null, IEnumerable<CallToAction> callToActions = null, CancellationToken cancelToken = default(CancellationToken))
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
                            Attachment = attachment,
                            QuickReply = quickReply,
                            CallToActions = callToActions
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

        /// <summary>
        /// Marks direct messages as having been read.
        /// </summary>
        /// <param name="lastReadEventID">ID of last direct message read.</param>
        /// <param name="recipientID">ID of user to send typing indicator to.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        public async Task MarkReadAsync(ulong lastReadEventID, ulong recipientID, CancellationToken cancelToken = default(CancellationToken))
        {
            if (lastReadEventID == default(ulong))
                throw new ArgumentException($"{nameof(lastReadEventID)} must be set.", nameof(lastReadEventID));
            if (recipientID == default(ulong))
                throw new ArgumentException($"{nameof(recipientID)} must be set.", nameof(recipientID));

            var newUrl = BaseUrl + "direct_messages/mark_read.json";

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<DirectMessageEvents>(
                    HttpMethod.Post.ToString(),
                    newUrl,
                    new Dictionary<string, string>
                    {
                        ["last_read_event_id"] = lastReadEventID.ToString(),
                        ["recipient_id"] = recipientID.ToString()
                    },
                    cancelToken)
                   .ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a typing indicator to a user.
        /// </summary>
        /// <param name="recipientID">ID of user to send typing indicator to.</param>
        /// <param name="cancelToken">Async cancellation token.</param>
        public async Task IndicateTypingAsync(ulong recipientID, CancellationToken cancelToken = default(CancellationToken))
        {
            if (recipientID == default(ulong))
                throw new ArgumentException($"{nameof(recipientID)} must be set.", nameof(recipientID));

            var newUrl = BaseUrl + "direct_messages/indicate_typing.json";

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<DirectMessageEvents>(
                    HttpMethod.Post.ToString(),
                    newUrl,
                    new Dictionary<string, string>
                    {
                        ["recipient_id"] = recipientID.ToString()
                    },
                    cancelToken)
                   .ConfigureAwait(false);
        }
    }
}
