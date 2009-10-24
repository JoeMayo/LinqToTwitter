using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Retweet Information
    /// </summary>
    public class Retweet
    {
        /// <summary>
        /// Retweet ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Date/Time Retweeted
        /// </summary>
        public DateTime RetweetedAt { get; set; }

        /// <summary>
        /// User who retweeted
        /// </summary>
        public User RetweetingUser { get; set; }
    }
}
