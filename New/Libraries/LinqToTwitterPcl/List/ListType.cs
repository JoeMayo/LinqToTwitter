using System;

namespace LinqToTwitter
{
    /// <summary>
    /// Available types of queries for the Twitter Lists API
    /// </summary>
    public enum ListType
    {
        /// <summary>
        /// Show specified list
        /// </summary>
        Show,

        /// <summary>
        /// Show lists user is subscribed to
        /// </summary>
        List,

        /// <summary>
        /// Show tweet timeline for members of the specified list
        /// </summary>
        Statuses,

        /// <summary>
        /// Show lists that specified user is a member of
        /// </summary>
        Memberships,

        /// <summary>
        /// Shows the lists a user is subscribed to
        /// </summary>
        Subscriptions,

        /// <summary>
        /// Members of the specified list
        /// </summary>
        Members,

        /// <summary>
        /// Check if a user is a member of the specified list
        /// </summary>
        IsMember,

        /// <summary>
        /// List the users subscribed to the specified list
        /// </summary>
        Subscribers,

        /// <summary>
        /// Check if a user subscribes to the specified list
        /// </summary>
        IsSubscriber,

        /// <summary>
        /// Get lists belonging to the specified user
        /// </summary>
        Ownerships
    }
}
