namespace LinqToTwitter
{
    public enum HelpType
    {
        /// <summary>
        /// Various settings such as image size, t.co url sizes, and more (should be cached and reused, but refreshed no more than once a day)
        /// </summary>
        Configuration,

        /// <summary>
        /// Languages supported by Twitter
        /// </summary>
        Languages,
        
        /// <summary>
        /// Get Twitter Privacy Policy
        /// </summary>
        Privacy,

        /// <summary>
        /// Provides Rate Limit Status
        /// </summary>
        RateLimits,

        /// <summary>
        /// Get Twitter Terms of Service
        /// </summary>
        Tos
    }
}
