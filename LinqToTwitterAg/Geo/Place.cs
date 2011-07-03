using System.Linq;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// A general description of a geographical location in Twitter
    /// </summary>
    public class Place
    {
        /// <summary>
        /// Converts XML to Place
        /// </summary>
        /// <param name="place">XML containing place info</param>
        /// <returns>Place populated from XML</returns>
        public static Place CreatePlace(XElement place)
        {
            if (place == null || place.Descendants().Count() == 0)
            {
                return null;
            }

            var geometry = new Geometry();

            return new Place
            {
                ID = place.Element("id").Value,
                Name = place.Element("name").Value,
                Country = 
                    place.Element("country") == null ?
                        string.Empty :
                        place.Element("country").Value,
                CountryCode = 
                    place.Element("country_code") == null ?
                        place.Element("country") != null &&
                        place.Element("country").Attribute("code") != null ?
                            place.Element("country").Attribute("code").Value :
                            string.Empty :
                        place.Element("country_code").Value,
                FullName = place.Element("full_name").Value,
                PlaceType = place.Element("place_type").Value,
                Url = place.Element("url").Value,
                BoundingBox = geometry.CreateGeometry(place.Element("bounding_box")),
                ContainedWithin = CreatePlace(place.Element("item")),
                Geometry = geometry.CreateGeometry(place.Element("geometry")),
                PolyLines = 
                    place.Element("polylines") == null ?
                        string.Empty :
                        place.Element("polylines").Element("item").Value
            };
        }

        /// <summary>
        /// Name of place
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Country code abbreviation
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Place ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Name of country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Type of place (i.e. neighborhood, city, country, etc.)
        /// </summary>
        public string PlaceType { get; set; }

        /// <summary>
        /// Url to get more details on place
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Full name of place
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public Geometry BoundingBox { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public Geometry Geometry { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public string PolyLines { get; set; }

        /// <summary>
        /// Containing place (i.e. a neighborhood is contained within a city)
        /// </summary>
        public Place ContainedWithin { get; set; }
    }
}
