using System;

namespace LinqToTwitter
{
    /// <summary>
    /// holds rate limit info
    /// </summary>
    public class RateLimitStatus
    {
        public int RemainingHits { get; set; }

        public int HourlyLimit { get; set; }

        public DateTime ResetTime { get; set; }

        public int ResetTimeInSeconds { get; set; }
    }
}
