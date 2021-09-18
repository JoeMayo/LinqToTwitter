namespace LinqToTwitter
{
    /// <summary>
    /// Frequency of Count query results
    /// </summary>
    public enum Granularity
    {
        /// <summary>
        /// Unspecified, defaults to Day
        /// </summary>
        None,

        /// <summary>
        /// Return results for every day of the time frame
        /// </summary>
        Day,

        /// <summary>
        /// Return results for every hour of the time frame
        /// </summary>
        Hour,

        /// <summary>
        /// Return results for every minute of the time frame
        /// </summary>
        Minute
    }
}
