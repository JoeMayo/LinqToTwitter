using LinqToTwitter.Common;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Times to stop and start sending notifications
    ///     - Times are null when Enabled is false
    /// </summary>
    public class SleepTime
    {
        public SleepTime() {}
        internal SleepTime(JsonData sleepTime)
        {
            StartHour = sleepTime.GetValue<int>("start_time");
            EndHour = sleepTime.GetValue<int>("end_time");
            Enabled = sleepTime.GetValue<bool>("enabled");
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
