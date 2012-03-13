using System.Xml.Linq;

using LinqToTwitter.Common;

namespace LinqToTwitter
{
    /// <summary>
    /// Location Info
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Converts XML into a new location object
        /// </summary>
        /// <param name="loc">XML</param>
        /// <returns>Location</returns>
        public static Location CreateLocation(XElement loc)
        {
            if (loc != null)
            {
                XElement country = loc.Element("country");
                XElement placeType = loc.Element("placeTypeName");

                return new Location
                {
                    Country = country.GetString(null, string.Empty),
                    CountryCode =
                        country == null ?
                            string.Empty :
                            country.Attribute("code") == null ?
                                string.Empty :
                                country.Attribute("code").Value,
                    Name = loc.GetString("name"),
                    PlaceTypeName = placeType.GetString(null, string.Empty),
                    PlaceTypeNameCode = int.Parse(
                        placeType == null ?
                            "0" :
                            placeType.Attribute("code") == null ?
                                "0" :
                                placeType.Attribute("code").Value),
                    Url = loc.GetString("url"),
                    WoeID =  loc.GetString("woeid"),
                    ParentID = loc.GetString("parentid")
                };
            }

            return null;
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
