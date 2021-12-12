namespace LinqToTwitter
{
    /// <summary>
    /// Available types of queries for the Twitter Lists API
    /// </summary>
    public enum ListType
    {
        /// <summary>
        /// Get list by ID
        /// </summary>
        Lookup,

        /// <summary>
        /// Get lists that a user owns
        /// </summary>
        Owned,

        /// <summary>
        /// Gets lists that a user is a member of
        /// </summary>
        Member,

        /// <summary>
        /// Get lists that user is following
        /// </summary>
        Following,

        /// <summary>
        /// Gets lists that user has pinned
        /// </summary>
        Pinned,
    }
}
