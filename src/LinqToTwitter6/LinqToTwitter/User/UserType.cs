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
        /// Get list of people that are following user
        /// </summary>
        Followers,

        /// <summary>
        /// Get list of people that user is following 
        /// </summary>
        Following,

        /// <summary>
        /// Search users by ids
        /// </summary>
        IdLookup,

        /// <summary>
        /// Users who liked a tweet
        /// </summary>
        Liking,

        /// <summary>
        /// Gets users who follow a list
        /// </summary>
        ListFollowers,

        /// <summary>
        /// Get users who are members of a list
        /// </summary>
        ListMembers,

        /// <summary>
        /// Gets user information for the currently authenticated user
        /// </summary>
        Me,

        /// <summary>
        /// Get tweets a user retweeted
        /// </summary>
        RetweetedBy,

        /// <summary>
        /// Perform a user search
        /// </summary>
        Search,

        /// <summary>
        /// People who bought a ticket to a space
        /// </summary>
        SpaceBuyers,

        /// <summary>
        /// Search users by username
        /// </summary>
        UsernameLookup,
    }
}
