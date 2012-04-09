using System;

using LinqToTwitter.Common;

using LitJson;

namespace LinqToTwitter
{
    // TODO: Add support for entities (which are included in the JSON response)
    /// <summary>
    /// Direct message elements contain a set of values that describe a message.
    /// </summary>
    public class DirectMessage
    {
        public DirectMessage() { }
        public DirectMessage(JsonData dmJson)
        {
            CreatedAt = dmJson.GetValue<string>("created_at").GetDate(DateTime.MinValue);
            SenderID = dmJson.GetValue<ulong>("sender_id");
            SenderScreenName = dmJson.GetValue<string>("sender_screen_name");
            Sender = new User(dmJson.GetValue<JsonData>("sender"));
            RecipientID = dmJson.GetValue<ulong>("recipient_id");
            RecipientScreenName = dmJson.GetValue<string>("recipient_screen_name");
            Recipient = new User(dmJson.GetValue<JsonData>("recipient"));
            ID = dmJson.GetValue<ulong>("id");
            IDString = dmJson.GetValue<string>("id_str");
            Text = dmJson.GetValue<string>("text");
        }

        public DirectMessageType Type { get; set; }

        /// <summary>
        /// Direct Message ID
        /// </summary>
        public ulong ID { get; set; }

        /// <summary>
        /// Direct Message ID in string format from JSON
        /// </summary>
        /// <remarks>>
        /// Twitter added this to the API because of the possibility that the real ID in ulong format wasn't accurate
        /// </remarks>
        public string IDString { get; set; }

        /// <summary>
        /// User ID of sender
        /// </summary>
        public ulong SenderID { get; set; }

        /// <summary>
        /// since this message ID
        /// </summary>
        public ulong SinceID { get; set; }

        /// <summary>
        /// max ID to return
        /// </summary>
        public ulong MaxID { get; set; }

        /// <summary>
        /// page number to return
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// number of items to return (works for SentBy and SentTo
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// DM Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// User ID of Recipient
        /// </summary>
        public ulong RecipientID { get; set; }

        /// <summary>
        /// When DM was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ScreenName of Sender
        /// </summary>
        public string SenderScreenName { get; set; }

        /// <summary>
        /// ScreenName of Recipient
        /// </summary>
        public string RecipientScreenName { get; set; }

        /// <summary>
        /// User object for sender
        /// </summary>
        public User Sender { get; set; }

        /// <summary>
        /// User object for recipient
        /// </summary>
        public User Recipient { get; set; }
    }
}
