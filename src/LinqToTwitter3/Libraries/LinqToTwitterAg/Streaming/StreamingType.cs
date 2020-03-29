namespace LinqToTwitter
{
    public enum StreamingType
    {
        /// <summary>
        /// Tweets matching a predicate (count, delimited, follow, locations, or track)
        /// </summary>
        Filter,

        /// <summary>
        /// All public tweets
        /// </summary>
        Firehose,

        // TODO: Not documented on Twitter API 1.1
        /// <summary>
        /// Tweets containing http or https
        /// </summary>
        Links,

        // TODO: Not documented on Twitter API 1.1
        /// <summary>
        /// Retweets...
        /// </summary>
        Retweet,

        /// <summary>
        /// Random (as defined by Twitter) tweets
        /// </summary>
        Sample
    }
}
