using System.Linq;
using LitJson;
using System.Xml.Serialization;

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
        internal Coordinate(JsonData coord)
        {
            if (coord == null)
            {
                IsLocationAvailable = false;
                return;
            }

            IsLocationAvailable = true;

            JsonData jsonLatitude = coord[LatitudePos];
            Latitude = jsonLatitude.IsDouble ? (double)jsonLatitude : (int)jsonLatitude;
            JsonData jsonLongitude = coord[LongitudePos];
            Longitude = jsonLongitude.IsDouble ? (double)jsonLongitude : (int)jsonLongitude;
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
