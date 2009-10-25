using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    public enum ListType
    {
        /// <summary>
        /// Lists your lists
        /// </summary>
        Yours,

        /// <summary>
        /// List the lists the specified user has been added to
        /// </summary>
        UserAdded,

        /// <summary>
        /// Show tweet timeline for members of the specified list
        /// </summary>
        Tweets,

        /// <summary>
        /// Show a specific list you can use the new resource
        /// </summary>
        Specific,

        /// <summary>
        /// Members of the specified list
        /// </summary>
        Members,

        /// <summary>
        /// Check if a user is a member of the specified list
        /// </summary>
        IsMember,

        /// <summary>
        /// Subscribe the authenticated user to the specified list
        /// </summary>
        Subscribe,

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
