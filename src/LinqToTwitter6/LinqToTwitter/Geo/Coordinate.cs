using System.Xml.Serialization;
using System.Text.Json;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    /// <summary>
    /// Geographical coordinates
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Coordinate
    {
        public const int LongitudePos = 0;
        public const int LatitudePos = 1;

        public Coordinate() { }
        internal Coordinate(JsonElement coord)
        {
            if (coord.IsNull())
            {
                IsLocationAvailable = false;
                return;
            }

            IsLocationAvailable = true;

            // TODO: make sure we pull these out correctly and test
            //JsonElement jsonLatitude = coord[LatitudePos];
            //Latitude = jsonLatitude.IsDouble ? (double)jsonLatitude : (int)jsonLatitude;
            //JsonElement jsonLongitude = coord[LongitudePos];
            //Longitude = jsonLongitude.IsDouble ? (double)jsonLongitude : (int)jsonLongitude;
        }

        /// <summary>
        /// Type of Coordinate
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }

        public bool IsLocationAvailable { get; set; }
    }
}
