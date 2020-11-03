namespace LinqToTwitter
{
    public enum StreamingType
    {
        /// <summary>
        /// Tweets matching a predicate (count, delimited, follow, locations, or track)
        /// </summary>
        Filter,

        /// <summary>
        /// Random (as defined by Twitter) tweets
        /// </summary>
        Sample,
    }
}
