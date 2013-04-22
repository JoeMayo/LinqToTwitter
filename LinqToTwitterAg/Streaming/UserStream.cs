namespace LinqToTwitter
{
    /// <summary>
    /// Information for creating user streams
    /// </summary>
    public class UserStream
    {
        /// <summary>
        /// Type of user stream
        /// </summary>
        public UserStreamType Type { get; set; }

        /// <summary>
        /// Stream delimiter
        /// </summary>
        public string Delimited { get; set; }

        /// <summary>
        /// Comma-separated list (no spaces) of users to add to Site Stream
        /// </summary>
        public string Follow { get; set; }

        /// <summary>
        /// Search terms
        /// </summary>
        public string Track { get; set; }

        /// <summary>
        /// Type of entities to return, i.e. Follow, User, etc.
        /// </summary>
        public string With { get; set; }

        /// <summary>
        /// Normally, only replies between two users that follow each other show.
        /// Setting this to true will show replies, regardless of follow status.
        /// </summary>
        public bool AllReplies { get; set; }

        /// <summary>
        /// Tell Twitter to send stall warnings
        /// </summary>
        public bool StallWarnings { get; set; }

        /// <summary>
        /// Bounding box of locations to include tweets
        /// </summary>
        public string Locations { get; set; }

        /// <summary>
        /// Executor managing stream
        /// </summary>
        internal ITwitterExecute TwitterExecutor { get; set; }

        /// <summary>
        /// Closes stream
        /// </summary>
        public void CloseStream()
        {
            TwitterExecutor.CloseStream = true;
        }
    }
}
