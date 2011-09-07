namespace LinqToTwitter
{
    /// <summary>
    /// Type of trend to query
    /// </summary>
    public enum TrendType
    {
        /// <summary>
        /// top ten topics that are currently trending world-wide (same as Location with a WoeId of 1)
        /// </summary>
        Trend,

        /// <summary>
        /// top 20 trending topics for every hour of specified day
        /// </summary>
        Daily,

        /// <summary>
        /// top 30 trending topics for every day of specified week
        /// </summary>
        Weekly,

        /// <summary>
        /// Locations of where trends are occurring
        /// </summary>
        Available,

        /// <summary>
        /// Top 10 topics for a location
        /// </summary>
        Location
    }
}
