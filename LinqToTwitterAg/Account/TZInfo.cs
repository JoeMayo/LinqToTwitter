using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Time zone informtion
    /// </summary>
    public class TZInfo
    {
        /// <summary>
        /// Human-readable timezone name
        /// </summary>
        /// <example>Pacific Time (US &amp; Canada)</example>
        public string Name { get; set; }

        /// <summary>
        /// Rails/unix TZINFO name
        /// </summary>
        /// <example>America/Los_Angeles</example>
        public string TzInfoName { get; set; }

        /// <summary>
        /// Seconds to subtract from UTC time
        /// </summary>
        /// <example>-28800</example>
        public int? UtcOffset { get; set; }
    }
}
