using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Serialization;
using LinqToTwitter.Common;

namespace LinqToTwitter
{
    /// <summary>
    /// A general description of a geographical location in Twitter
    /// </summary>
    [XmlType(Namespace="LinqToTwitter")]
    public class Place
    {
        public Place() {}
        internal Place(JsonElement place)
        {
            if (place.IsNull()) return;

            ID = place.GetString("id");
            Name = place.GetString("name");
            Country = place.GetString("country");
            CountryCode = place.GetString("country_code");
            FullName = place.GetString("full_name");
            PlaceType = place.GetString("place_type");
            Url = place.GetString("url");
            place.TryGetProperty("bounding_box", out JsonElement boundingBoxValue);
            BoundingBox = new Geometry(boundingBoxValue);
            place.TryGetProperty("geometry", out JsonElement geometryValue);
            Geometry = new Geometry(geometryValue);

            place.TryGetProperty("contained_within", out JsonElement containedWithin);
            ContainedWithin = 
                !containedWithin.IsNull() && containedWithin.GetArrayLength() > 0 ? 
                    new Place(containedWithin[0]) :
                    null;

            place.TryGetProperty("polylines", out JsonElement polyLines);
            PolyLines = 
                polyLines.IsNull() ? 
                    new List<string>() 
                        : 
                    (from line in polyLines.EnumerateArray()
                     select line.GetString())
                    .ToList();

            place.TryGetProperty("attributes", out JsonElement attributes);
            Attributes =
                attributes
                    .EnumerateObject()
                    .ToDictionary(
                        attr => attr.Name,
                        attr => attr.Value.GetString() ?? string.Empty);
        }

        /// <summary>
        /// Name of place
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Country code abbreviation
        /// </summary>
        public string? CountryCode { get; set; }

        /// <summary>
        /// Place ID
        /// </summary>
        public string? ID { get; set; }

        /// <summary>
        /// Name of country
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Type of place (i.e. neighborhood, city, country, etc.)
        /// </summary>
        public string? PlaceType { get; set; }

        /// <summary>
        /// Url to get more details on place
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Full name of place
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Place related metadata
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, string>? Attributes { get; set; }

        /// <summary>
        /// Geographical outline of place
        /// </summary>
        public Geometry? BoundingBox { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public Geometry? Geometry { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public List<string>? PolyLines { get; set; }

        /// <summary>
        /// Containing place (i.e. a neighborhood is contained within a city)
        /// </summary>
        public Place? ContainedWithin { get; set; }
    }
}
