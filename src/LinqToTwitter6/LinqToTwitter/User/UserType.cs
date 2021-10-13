namespace LinqToTwitter
{
    /// <summary>
    /// type of user request
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// List of sizes for account banners
        /// </summary>
        BannerSizes,

        /// <summary>
        /// Users that specified user can contribute to
        /// </summary>
        Contributees,

        /// <summary>
        /// Users who can contribute to an account
        /// </summary>
        Contributors,

        /// <summary>
        /// Search users by ids
        /// </summary>
        IdLookup,

        /// <summary>
        /// Get tweets a user retweeted
        /// </summary>
        RetweetedBy,

        /// <summary>
        /// Perform a user search
        /// </summary>
        Search,

        /// <summary>
        /// Search users by username
        /// </summary>
        UsernameLookup,

        /// <summary>
        /// Get list of people that are following user
        /// </summary>
        Followers,

        /// <summary>
        /// Get list of people that user is following 
        /// </summary>
        Following,
    }
}
