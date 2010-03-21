using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// type of user request
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// user's friends
        /// </summary>
        Friends,

        /// <summary>
        /// user's followers
        /// </summary>
        Followers,

        /// <summary>
        /// extended information on a user
        /// </summary>
        Show
    }
}
