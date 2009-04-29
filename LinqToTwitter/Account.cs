using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// information for account queries
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Type of account query (VerifyCredentials or RateLimitStatus)
        /// </summary>
        public AccountType Type { get; set; }

        /// <summary>
        /// User returned by VerifyCredentials Queries
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// RateLimitStatus returned by RateLimitStatus queries
        /// </summary>
        public RateLimitStatus RateLimitStatus { get; set; }

        /// <summary>
        /// Response from request to end session
        /// </summary>
        public EndSessionStatus EndSessionStatus { get; set; }
    }
}
