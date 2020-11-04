namespace LinqToTwitter
{
    public enum StreamingType
    {
        /// <summary>
        /// Tweets matching a predicate (count, delimited, follow, locations, or track)
        /// </summary>
        Filter,

        /// <summary>
        /// Get Filter stream search rules
        /// </summary>
        Rules,

        /// <summary>
        /// Random (as defined by Twitter) tweets
        /// </summary>
        Sample,
    }
}
