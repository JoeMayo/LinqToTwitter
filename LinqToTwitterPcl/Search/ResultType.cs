namespace LinqToTwitter
{
    /// <summary>
    /// Type of results to return in a search
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// Combination of popular and recent
        /// </summary>
        Mixed,

        /// <summary>
        /// Real-time results
        /// </summary>
        Recent,

        /// <summary>
        /// Most popular tweets
        /// </summary>
        Popular
    }
}
