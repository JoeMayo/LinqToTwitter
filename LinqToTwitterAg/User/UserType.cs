namespace LinqToTwitter
{
    /// <summary>
    /// type of user request
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// extended information on a user
        /// </summary>
        Show,

        /// <summary>
        /// Available Twitter suggestion categories
        /// </summary>
        Categories,

        /// <summary>
        /// Users under a specified category
        /// </summary>
        Category,

        /// <summary>
        /// Get most recent tweet of each user in category
        /// </summary>
        CategoryStatus,

        /// <summary>
        /// Get user details for a set of users
        /// </summary>
        Lookup,

        /// <summary>
        /// Perform a user search
        /// </summary>
        Search
    }

    enum UserAction
    {
        SingleUser
    }
}
