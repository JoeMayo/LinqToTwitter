using System;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Location Info
    /// </summary>
    [Serializable]
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
                    Country = 
                        country == null ?
                            string.Empty :
                            country.Value,
                    CountryCode =
                        country == null ?
                            string.Empty :
                            country.Attribute("code") == null ?
                                string.Empty :
                                country.Attribute("code").Value,
                    CountryType =
                        country == null ?
                            string.Empty :    
                            country.Attribute("type") == null ?
                                string.Empty :
                                country.Attribute("type").Value,
                    Name = 
                        loc.Element("name") == null ?
                            string.Empty :
                            loc.Element("name").Value,
                    PlaceTypeName =
                        placeType == null ?
                            string.Empty :
                            placeType.Value,
                    PlaceTypeNameCode = int.Parse(
                        placeType == null ?
                            "0" :
                            placeType.Attribute("code") == null ?
                                "0" :
                                placeType.Attribute("code").Value),
                    Url = 
                        loc.Element("url") == null ?
                            string.Empty :
                            loc.Element("url").Value,
                    WoeID = 
                        loc.Element("woeid") == null ?
                            string.Empty :
                            loc.Element("woeid").Value
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
        /// Country type
        /// </summary>
        public string CountryType { get; set; }

        /// <summary>
        /// Country Code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Yahoo Location URL
        /// </summary>
        public string Url { get; set; }
    }
}
