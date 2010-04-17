using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// type of friendship actions
    /// </summary>
    public enum FriendshipType
    {
        /// <summary>
        /// does freindship exist between two users
        /// </summary>
        Exists,

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
        Outgoing
    }
}
