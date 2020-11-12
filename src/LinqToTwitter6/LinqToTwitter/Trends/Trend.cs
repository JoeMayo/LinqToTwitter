using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// helps to work with trends
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Trend
    {
        /// <summary>
        /// type of trend to query (Trend (all), Current, Daily, or Weekly)
        /// </summary>
        public TrendType Type { get; set; }

        /// <summary>
        /// When place trend was created
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// time of request
        /// </summary>
        public DateTime? AsOf { get; set; }

        /// <summary>
        /// twitter search query on topic
        /// </summary>
        public string? Query { get; set; }

        /// <summary>
        /// Search URL returned from Local Trends
        /// </summary>
        public string? SearchUrl { get; set; }

        /// <summary>
        /// name of trend topic
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Yahoo Where On Earth ID
        /// </summary>
        public int WoeID { get; set; }

        /// <summary>
        /// Set to true to omit hashtags from results
        /// </summary>
        public bool Exclude { get; set; }

        /// <summary>
        /// No idea, perhaps the event associated with a trend
        /// </summary>
        public string? Events { get; set; }

        /// <summary>
        /// Flag indicating this is a promoted trend (as opposed to organic)
        /// </summary>
        public string? PromotedContent { get; set; }

        /// <summary>
        /// Return value for Avalable query listing locations of trending topics
        /// </summary>
        public List<Location>? Locations { get; set; }

        /// <summary>
        /// Number of tweets in a trend
        /// </summary>
        public int TweetVolume { get; set; }
    }
}
