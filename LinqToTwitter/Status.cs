/***********************************************************
 * Credits:
 * 
 * Written by: Joe Mayo, 8/26/08
 * *********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// returned information from Twitter Status queries
    /// </summary>
    public class Status
    {
        /// <summary>
        /// type of status request, i.e. Friends or Public
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// when was the tweet created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// TweetID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Tweet Text (140)characters
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// where did the tweet come from
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// has the tween been truncated
        /// </summary>
        public bool Truncated { get; set; }

        /// <summary>
        /// id of tweet being replied to, if it is a reply
        /// </summary>
        public string InReplyToStatusID { get; set; }

        /// <summary>
        /// id of user being replied to, if it is a reply
        /// </summary>
        public string InReplyToUserID { get; set; }

        /// <summary>
        /// is listed as a favorite
        /// </summary>
        public bool Favorited { get; set; }

        /// <summary>
        /// information about user posting tweet
        /// </summary>
        public User User { get; set; }
    }
}
