using System.Xml.Serialization;
namespace LinqToTwitter
{
    /// <summary>
    /// Account Settings
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
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

        /// <summary>
        /// Account screen name
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Use cookie personalization
        /// </summary>
        public bool UseCookiePersonalization { get; set; }

        /// <summary>
        /// Can other users find you on Twitter via your mobile phone
        /// </summary>
        public bool DiscoverableByMobilePhone { get; set; }

        /// <summary>
        /// User can see media marked as sensitive
        /// </summary>
        public bool DisplaySensitiveMedia { get; set; }

        /// <summary>
        /// Allow contributor requests
        /// </summary>
        public string AllowContributorRequest { get; set; }

        /// <summary>
        /// Who is allowed to send DMs
        /// </summary>
        public string AllowDmsFrom { get; set; }

        /// <summary>
        /// Who is allowed to send grouped DMs
        /// </summary>
        public string AllowDmGroupsFrom { get; set; }
    }
}
