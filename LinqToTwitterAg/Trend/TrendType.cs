namespace LinqToTwitter
{
    /// <summary>
    /// Type of trend to query
    /// </summary>
    public enum TrendType
    {
        /// <summary>
        /// Locations of where trends are occurring
        /// </summary>
        Available,

        /// <summary>
        /// Trends closest to specified lat/long
        /// </summary>
        Closest,

        /// <summary>
        /// Top 10 topics for a WOEID
        /// </summary>
        Place
    }
}
