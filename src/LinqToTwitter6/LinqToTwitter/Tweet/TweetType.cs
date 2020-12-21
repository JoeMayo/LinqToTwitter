namespace LinqToTwitter
{
    public enum TweetType
    {
        /// <summary>
        /// Lookup one or more tweets
        /// </summary>
        Lookup,

        /// <summary>
        /// Get the mentions timeline
        /// </summary>
        MentionsTimeline,

        /// <summary>
        /// Get the user timeline
        /// </summary>
        UserTimeline
    }
}
