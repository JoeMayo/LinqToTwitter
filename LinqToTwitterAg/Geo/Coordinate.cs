using System.Linq;
using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// Geographical coordinates
    /// </summary>
    public class Coordinate
    {
        public const int LatitudePos = 0;
        public const int LongitudePos = 1;

        public Coordinate() { }
        internal Coordinate(JsonData coord)
        {
            if (coord == null) return;
            var jsonLatitude = coord[LatitudePos];
            Latitude = jsonLatitude.IsDouble ? (double)jsonLatitude : (int)jsonLatitude;
            var jsonLongitude = coord[LongitudePos];
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
    }
}
