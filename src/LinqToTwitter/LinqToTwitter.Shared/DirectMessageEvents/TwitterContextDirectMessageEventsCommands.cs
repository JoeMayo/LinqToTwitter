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
        /// <param name="cancelToken">Async cancellation token.</param>
        /// <returns>Direct message events data.</returns>
        public async Task<DirectMessageEvents> NewDirectMessageEventAsync(ulong recipientID, string text, CancellationToken cancelToken = default(CancellationToken))
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
                            Text = text
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

            dmEvents.RecipientID = recipientID;
            dmEvents.Text = text;

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
