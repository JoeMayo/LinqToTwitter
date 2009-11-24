using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    public enum ListType
    {
        /// <summary>
        /// Lists for specified user
        /// </summary>
        Lists,

        /// <summary>
        /// Show specified list
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
        IsSubscribed
    }
}
