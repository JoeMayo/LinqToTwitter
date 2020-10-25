namespace LinqToTwitter
{
    /// <summary>
    /// type of search
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// Classic Search on <see cref="Search"/>
        /// </summary>
        Search,

        /// <summary>
        /// Search for Recent Tweets on <see cref="TwitterSearch"/>
        /// </summary>
        RecentSearch
    }
}
