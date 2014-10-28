using System.Collections.Generic;
using System.Linq;
using LinqToTwitter.Common;
using LitJson;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    /// <summary>
    /// Geo info for querying and reading results
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Geo
    {
        public Geo() { }
        public Geo(JsonData geo)
        {
            if (geo == null) return;

            JsonData result = geo.GetValue<JsonData>("result");
            JsonData places = result.GetValue<JsonData>("places");

            Token = result.GetValue<string>("token");

            if (places != null)
            {
                Places =
                    (from JsonData place in places
                        select new Place(place))
                    .ToList(); 
            }
        }

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
        /// Name of place in similar places query
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// Place token returned from a Similar Places query and used in CreatePlaceAsync
        /// </summary>
        public string Token { get; set; }

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
