using System;
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
                    MessageCreate = new MessageCreate
                    {
                        MessageData = new MessageData
                        {
                            Text = text
                        },
                        Target = new Target
                        {
                            RecipientID = recipientID.ToString()
                        }
                    },
                    Type = "message_create"
                }
            };

            RawResult =
                await TwitterExecutor.PostJsonToTwitterAsync(
                    newUrl,
                    dmEventParams,
                    cancelToken)
                    .ConfigureAwait(false);

            var reqProc = new DirectMessageEventsRequestProcessor<DirectMessageEvents>();
            DirectMessageEvents dmEvents = reqProc.ProcessActionResult(RawResult, DirectMessageEventsType.Show);

            dmEvents.RecipientID = recipientID;
            dmEvents.Text = text;

            return dmEvents;
        }
    }
}
