using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
