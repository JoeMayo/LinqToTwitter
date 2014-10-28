using System;

namespace LinqToTwitter
{
    public class RateLimits
    {
        /// <summary>
        /// Url Segment representing resource that rate limits apply to
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// Number of requests left in this time period
        /// </summary>
        public int Remaining { get; set; }

        /// <summary>
        /// Epoch seconds when rate limits reset
        /// </summary>
        public ulong Reset { get; set; }

        /// <summary>
        /// Number of requests allowed
        /// </summary>
        public int Limit { get; set; }
    }
}
