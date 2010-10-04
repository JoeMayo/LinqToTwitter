using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Times to stop and start sending notifications
    ///     - Times are null when Enabled is false
    /// </summary>
    public class SleepTime
    {
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
