using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Type of trend to query
    /// </summary>
    public enum TrendType
    {
        /// <summary>
        /// top ten topics that are currently trending 
        /// </summary>
        Trend,

        /// <summary>
        /// current top 10 trending topics 
        /// </summary>
        Current,

        /// <summary>
        /// top 20 trending topics for every hour of specified day
        /// </summary>
        Daily,

        /// <summary>
        /// top 30 trending topics for every day of specified week
        /// </summary>
        Weekly,

        /// <summary>
        /// Locations of where trends are occurring
        /// </summary>
        Available,

        /// <summary>
        /// Top 10 topics for a location
        /// </summary>
        Location
    }
}
