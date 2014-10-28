namespace LinqToTwitter
{
    /// <summary>
    /// type of friendship actions
    /// </summary>
    public enum FriendshipType
    {
        /// <summary>
        /// Detailed information on the relationship between two people
        /// </summary>
        Show,

        /// <summary>
        /// Show IDs of all users requesting friendship with logged in user
        /// </summary>
        Incoming,

        /// <summary>
        /// Show IDs of all users logged in user is requesting friendship with
        /// </summary>
        Outgoing,

        /// <summary>
        /// Allows you to examine the relationship of a list of users, ScreenName,
        /// to the logged in user
        /// </summary>
        Lookup,

        /// <summary>
        /// List of user IDs logged in user doesn't want to receive retweets for
        /// </summary>
        NoRetweetIDs,

        /// <summary>
        /// List of User entities of friends (people the authenticated user follows)
        /// </summary>
        FriendsList,

        /// <summary>
        /// List of User entities of followers (people who follow the authenticated user)
        /// </summary>
        FollowersList
    }
}
