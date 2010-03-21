using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// type of social graph
    /// </summary>
    public enum SocialGraphType
    {
        /// <summary>
        /// people user is following
        /// </summary>
        Friends,

        /// <summary>
        /// people following user
        /// </summary>
        Followers
    }
}
