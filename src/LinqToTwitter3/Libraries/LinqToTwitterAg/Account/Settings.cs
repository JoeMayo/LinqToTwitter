namespace LinqToTwitter
{
    /// <summary>
    /// Account Settings
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Location to display trends for
        /// </summary>
        public Location TrendLocation { get; set; }

        /// <summary>
        /// Is Geo Tracking On?
        /// </summary>
        public bool GeoEnabled { get; set; }

        /// <summary>
        /// Times to not notify (i.e. when you're sleeping)
        /// </summary>
        public SleepTime SleepTime { get; set; }

        /// <summary>
        /// The user's UI language selection on Twitter
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Should they always use https protocol
        /// </summary>
        public bool AlwaysUseHttps { get; set; }

        /// <summary>
        /// Can this user be found by email address?
        /// </summary>
        public bool DiscoverableByEmail { get; set; }

        /// <summary>
        /// The user's timezone selection on Twitter
        /// </summary>
        public TZInfo TimeZone { get; set; }
    }
}
