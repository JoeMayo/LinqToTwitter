using System;

namespace LinqToTwitter
{
    /// <summary>
    /// DIrect message elements contain a set of values that describe a message, as well as nested <sender> and <recipient> nodes.
    /// </summary>
    public class DirectMessage
    {
        public DirectMessageType Type { get; set; }

        /// <summary>
        /// Direct Message ID
        /// </summary>
        public ulong ID { get; set; }

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
