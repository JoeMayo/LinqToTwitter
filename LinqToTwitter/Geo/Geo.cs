using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    /// <summary>
    /// Geo info for querying and reading results
    /// </summary>
    [Serializable]
    public class Geo
    {
        /// <summary>
        /// Type of Geo query (Reverse or ID)
        /// </summary>
        public GeoType Type { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public decimal Longitude { get; set; }


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
        /// Results showing places matching query
        /// </summary>
        public List<Place> Places { get; set; }

        /// <summary>
        /// Place ID
        /// </summary>
        public string ID { get; set; }
    }
}
