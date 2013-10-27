namespace LinqToTwitter
{
    public enum StreamingType
    {
        /// <summary>
        /// Tweets matching a predicate (count, delimited, follow, locations, or track)
        /// </summary>
        Filter,

        /// <summary>
        /// All tweets
        /// </summary>
        Firehose,

        /// <summary>
        /// Random (as defined by Twitter) tweets
        /// </summary>
        Sample,

        /// <summary>
        /// Activity for multiple users
        /// </summary>
        Site,

        /// <summary>
        /// A single user's activity
        /// </summary>
        User
    }
}
