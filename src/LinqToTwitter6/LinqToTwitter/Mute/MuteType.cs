using System;
using System.Linq;

namespace LinqToTwitter
{
    public enum MuteType
    {
        /// <summary>
        /// IDs of friends the authorizing user has muted.
        /// </summary>
        IDs,

        /// <summary>
        /// User entities of friends the authorizing user has muted.
        /// </summary>
        List
    }
}
