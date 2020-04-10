using System;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Reference to stream, details, and controls
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Streaming
    {
        /// <summary>
        /// Stream method
        /// </summary>
        public StreamingType Type { get; set; }

        /// <summary>
        /// Normally, only replies between two users that follow each other show.
        /// Setting this to true will show replies, regardless of follow status.
        /// </summary>
        public bool AllReplies { get; set; }

        /// <summary>
        /// Number of tweets to go back to when reconnecting
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Tweets are delimeted in the stream
        /// </summary>
        public string Delimited { get; set; }

        /// <summary>
        /// Limit results to a comma-separated set of users
        /// </summary>
        public string Follow { get; set; }

        /// <summary>
        /// Comma-separated list of languages to filter results on
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Get tweets in the comma-separated list of lat/lon's
        /// </summary>
        public string Locations { get; set; }

        /// <summary>
        /// Comma-separated list of keywords to get tweets for
        /// </summary>
        public string Track { get; set; }

        /// <summary>
        /// Tell Twitter to send stall warnings
        /// </summary>
        public bool StallWarnings { get; set; }

        /// <summary>
        /// Type of entities to return, i.e. Follow, User, etc.
        /// </summary>
        public string With { get; set; }

        /// <summary>
        /// Supports compatibility or extended mode tweets.
        /// </summary>
        [Obsolete("This isn't required on streams. Instead, check Retweeted.ExtendedTweet.FullText for retweets, ExtendedTweet.FullText for regular tweets, and fallback to FullText if the other checks are null.")]
        public TweetMode TweetMode { get; set; }

        /// <summary>
        /// Executor managing stream
        /// </summary>
        internal ITwitterExecute TwitterExecutor { get; set; }

        /// <summary>
        /// Closes stream
        /// </summary>
        public void CloseStream()
        {
            TwitterExecutor.CloseStream();
        }
    }
}
