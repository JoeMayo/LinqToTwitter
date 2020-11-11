using LinqToTwitter.Common;
using System.Globalization;
using System.Text.Json;
using System.Xml.Serialization;

namespace LinqToTwitter
{
    [XmlType(Namespace = "LinqToTwitter")]
    public class Location
    {
        public Location() {}
        internal Location(JsonElement trendLocation)
        {
            if (trendLocation.TryGetProperty("placeType", out JsonElement placeType))
            {
                PlaceTypeName = placeType.GetString("name");
                PlaceTypeNameCode = placeType.GetInt("code");
            }

            Country = trendLocation.GetString("country");
            Name = trendLocation.GetString("name");
            CountryCode = trendLocation.GetString("countryCode");
            ParentID = trendLocation.GetInt("parentid").ToString(CultureInfo.InvariantCulture);
            Url = trendLocation.GetString("url");
            WoeID = trendLocation.GetInt("woeid");
        }

        /// <summary>
        /// Yahoo Where On Earth ID
        /// </summary>
        public int WoeID { get; set; }

        /// <summary>
        /// Name of location
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Type of location
        /// </summary>
        public string? PlaceTypeName { get; set; }

        /// <summary>
        /// Code for PlaceTypeName
        /// </summary>
        public int PlaceTypeNameCode { get; set; }

        /// <summary>
        /// Country of Location
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Country Code
        /// </summary>
        public string? CountryCode { get; set; }

        /// <summary>
        /// Yahoo Location URL
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Parent location relative to current location.
        /// Set to null if current location is World.
        /// </summary>
        public string? ParentID { get; set; }
    }
}
