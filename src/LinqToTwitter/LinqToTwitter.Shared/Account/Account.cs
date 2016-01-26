using System.Xml.Serialization;
namespace LinqToTwitter
{
    /// <summary>
    /// information for account queries
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Account
    {
        /// <summary>
        /// Type of account query (VerifyCredentials or Settings)
        /// </summary>
        public AccountType Type { get; set; }

        /// <summary>
        /// Don't include statuses in response (input)
        /// </summary>
        public bool SkipStatus { get; set; }

        /// <summary>
        /// Removes entities when set to false (true by default)
        /// </summary>
        public bool IncludeEntities { get; set; }

        /// <summary>
        /// Includes the user's email address in response (requires whitelisting,
        /// see https://dev.twitter.com/rest/reference/get/account/verify_credentials)
        /// </summary>
        public bool IncludeEmail { get; set; }

        /// <summary>
        /// User returned by VerifyCredentials Queries
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Current Totals
        /// </summary>
        public Totals Totals { get; set; }

        /// <summary>
        /// Account settings, such as trend location, geo enabled, and sleep time
        /// </summary>
        public Settings Settings { get; set; }
    }
}
