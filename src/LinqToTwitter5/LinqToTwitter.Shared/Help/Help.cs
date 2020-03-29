using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Help
    {
        /// <summary>
        /// Help Type (Test, Configuration, or Languages)
        /// </summary>
        public HelpType Type { get; set; }

        /// <summary>
        /// Comma-separated list of resources for rate limit status request (setting to null returns all)
        /// </summary>
        public string Resources { get; set; }

        /// <summary>
        /// Will be true if help Test succeeds
        /// </summary>
        public bool OK { get; set; }

        /// <summary>
        /// Terms of service or Privacy
        /// </summary>
        public string Policies { get; set; }

        /// <summary>
        /// Populated for Help Configuration query
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// List of languages, codes, and statuses
        /// </summary>
        public List<Language> Languages { get; set; }

        /// <summary>
        /// Access token for which rate limit applies
        /// </summary>
        public string RateLimitAccountContext { get; set; }

        /// <summary>
        /// Rate limit statuses
        /// </summary>
        public Dictionary<string, List<RateLimits>> RateLimits { get; set; }
    }
}
