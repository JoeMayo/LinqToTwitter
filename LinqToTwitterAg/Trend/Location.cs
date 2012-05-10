using System.Globalization;
using LinqToTwitter.Common;

using LitJson;

namespace LinqToTwitter
{
    public class Location
    {
        public Location() {}
        internal Location(JsonData trendLocation)
        {
            var placeType = trendLocation.GetValue<JsonData>("placeType");

            Country = trendLocation.GetValue<string>("country");
            Name = trendLocation.GetValue<string>("name");
            CountryCode = trendLocation.GetValue<string>("countryCode");
            ParentID = trendLocation.GetValue<int>("parentid").ToString(CultureInfo.InvariantCulture);
            PlaceTypeName = placeType.GetValue<string>("name");
            PlaceTypeNameCode = placeType.GetValue<int>("code");
            Url = trendLocation.GetValue<string>("url");
            WoeID = trendLocation.GetValue<int>("woeid").ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Yahoo Where On Earth ID
        /// </summary>
        public string WoeID { get; set; }

        /// <summary>
        /// Name of location
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of location
        /// </summary>
        public string PlaceTypeName { get; set; }

        /// <summary>
        /// Code for PlaceTypeName
        /// </summary>
        public int PlaceTypeNameCode { get; set; }

        /// <summary>
        /// Country of Location
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Country Code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Yahoo Location URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Parent location relative to current location.
        /// Set to null if current location is World.
        /// </summary>
        public string ParentID { get; set; }
    }
}
