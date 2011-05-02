using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Geo info for querying and reading results
    /// </summary>
    public class Geo
    {
        // TODO: add Created method and replace manual Geo parsing elsewhere
        /// <summary>
        /// Type of Geo query (Reverse or ID)
        /// </summary>
        public GeoType Type { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }


        /// <summary>
        /// IP address to find nearby places
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// How accurate the results should be.
        ///     - A number defaults to meters
        ///     - Default is 0m
        ///     - Feet is ft (as in 10ft)
        /// </summary>
        public string Accuracy { get; set; }

        /// <summary>
        /// Size of place (i.e. neighborhood is default or city)
        /// </summary>
        public string Granularity { get; set; }

        /// <summary>
        /// Number of places to return
        /// </summary>
        public int MaxResults { get; set; }

        /// <summary>
        /// Any text you want to add to help find a place
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Place ID to restrict search to
        /// </summary>
        public string ContainedWithin { get; set; }

        /// <summary>
        /// Name/value pair separated by "=" (i.e. "street_address=123 4th Street")
        /// </summary>
        public string Attribute { get; set; }

        /// <summary>
        /// Results showing places matching query
        /// </summary>
        public List<Place> Places { get; set; }

        /// <summary>
        /// Place ID
        /// </summary>
        public string ID { get; set; }
    }
}
