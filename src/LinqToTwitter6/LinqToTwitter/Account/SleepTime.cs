using LinqToTwitter.Common;
using System.Text.Json;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Times to stop and start sending notifications
    ///     - Times are null when Enabled is false
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class SleepTime
    {
        public SleepTime() {}
        internal SleepTime(JsonElement sleepTime)
        {
            StartHour = sleepTime.GetInt("start_time");
            EndHour = sleepTime.GetInt("end_time");
            Enabled = sleepTime.GetBool("enabled");
        }

        /// <summary>
        /// Stop sending notifications at this time
        /// </summary>
        public int? StartHour { get; set; }

        /// <summary>
        /// Resume sending notifications at this time
        /// </summary>
        public int? EndHour { get; set; }

        /// <summary>
        /// Is sleep times turned on
        /// </summary>
        public bool Enabled { get; set; }
    }
}
