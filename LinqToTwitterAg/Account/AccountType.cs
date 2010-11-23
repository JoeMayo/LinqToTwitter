using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// actions for querying accounts
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// Allows you to check user credentails
        /// </summary>
        VerifyCredentials,

        /// <summary>
        /// Gets current rate limits
        /// </summary>
        RateLimitStatus,

        /// <summary>
        /// Gets friend, follower, update, and favorites totals
        /// </summary>
        Totals,

        /// <summary>
        /// Gets trend, geo, and sleep settings
        /// </summary>
        Settings
    }
}
