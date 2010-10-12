using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Retweet Information
    /// </summary>
    [Serializable]
    public class Retweet
    {
        /// <summary>
        /// Retweet ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Date/Time Retweeted
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// User who retweeted
        /// </summary>
        public User RetweetingUser { get; set; }

        /// <summary>
        /// Retweet Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Which application sent retweet
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Is text truncated
        /// </summary>
        public bool Truncated { get; set; }

        /// <summary>
        /// Status ID retweeted
        /// </summary>
        public string InReplyToStatusID { get; set; }

        /// <summary>
        /// ID of User retweeted
        /// </summary>
        public string InReplyToUserID { get; set; }

        /// <summary>
        /// Is Favorited
        /// </summary>
        public bool Favorited { get; set; }

        /// <summary>
        /// Screen name of retweeted user
        /// </summary>
        public string InReplyToScreenName { get; set; }

        /// <summary>
        /// Number of retweets
        /// </summary>
        public int RetweetCount { get; set; }

        /// <summary>
        /// Has retweet been retweeted
        /// </summary>
        public object Retweeted { get; set; }
    }
}
