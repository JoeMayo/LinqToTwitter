using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// helps to work with trends
    /// </summary>
    public class Trend
    {
        /// <summary>
        /// type of trend to query (Trend (all), Current, Daily, or Weekly)
        /// </summary>
        public TrendType Type { get; set; }

        /// <summary>
        /// exclude all trends with hastags if set to true 
        /// (i.e. include "Wolverine" but not "#Wolverine")
        /// </summary>
        public bool ExcludeHashtags { get; set; }

        /// <summary>
        /// date to start
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// time of request
        /// </summary>
        public DateTime AsOf { get; set; }

        /// <summary>
        /// twitter search query on topic
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// name of trend topic
        /// </summary>
        public string Name { get; set; }
    }
}
