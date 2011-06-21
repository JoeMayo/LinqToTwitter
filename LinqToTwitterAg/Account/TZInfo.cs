using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Time zone informtion
    /// </summary>
    public class TZInfo
    {
        /// <summary>
        /// Converts XML into a new sleep-time object
        /// </summary>
        /// <param name="sleep_time">XML</param>
        /// <returns>SleepTime</returns>
        public static TZInfo CreateTZInfo(XElement time_zone)
        {
            if (time_zone != null)
            {
                return new TZInfo
                {
                    Name = time_zone.Element("name").Value ,
                    TzInfoName = time_zone.Element("tzinfo_name").Value,
                    UtcOffset = time_zone.Element("utc_offset").Value == string.Empty ? null :
                        (int?)int.Parse(time_zone.Element("utc_offset").Value)
                };
            }

            return null;
        }

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
