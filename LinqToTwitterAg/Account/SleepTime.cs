using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Times to stop and start sending notifications
    ///     - Times are null when Enabled is false
    /// </summary>
    public class SleepTime
    {
        /// <summary>
        /// Converts XML into a new sleep-time object
        /// </summary>
        /// <param name="sleep_time">XML</param>
        /// <returns>SleepTime</returns>
        public static SleepTime CreateSleepTime(XElement sleep_time)
        {
            if (sleep_time != null)
            {
                return new SleepTime
                {

                    StartHour = sleep_time.Element("start_time").Value == string.Empty ? null :
                        (int?)int.Parse(sleep_time.Element("start_time").Value),
                    EndHour = sleep_time.Element("end_time").Value == string.Empty ? null :
                        (int?)int.Parse(sleep_time.Element("end_time").Value),
                    Enabled = bool.Parse(sleep_time.Element("enabled").Value)
                };
            }

            return null;
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
