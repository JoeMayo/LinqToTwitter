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

            ID = place.GetProperty("id").GetString();
            Name = place.GetProperty("name").GetString();
            Country = place.GetProperty("country").GetString();
            CountryCode = place.GetProperty("country_code").GetString();
            FullName = place.GetProperty("full_name").GetString();
            PlaceType = place.GetProperty("place_type").GetString();
            Url = place.GetProperty("url").GetString();
            BoundingBox = new Geometry(place.GetProperty("bounding_box"));
            Geometry = new Geometry(place.GetProperty("geometry"));

            var containedWithin = place.GetProperty("contained_within");
            ContainedWithin = 
                !containedWithin.IsNull() && containedWithin.GetArrayLength() > 0 ? 
                    new Place(containedWithin[0]) :
                    null;

            var polyLines = place.GetProperty("polylines");
            PolyLines = 
                polyLines.IsNull() ? 
                    new List<string>() 
                        : 
                    (from line in polyLines.EnumerateArray()
                     select line.GetString())
                    .ToList();

            // TODO: re-write as JsonElement
            //var attrDict = place.GetProperty("attributes") as IDictionary<string, JsonElement>;
            //Attributes =
            //    attrDict == null ?
            //        new Dictionary<string, string>() 
            //            :
            //        (from string key in attrDict.Keys
            //         select new 
            //         { 
            //             Key = key, 
            //             Val = attrDict[key].ToString()
            //         })
            //        .ToDictionary(
            //            attr => attr.Key,
            //            attr => attr.Val);
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
        /// Place related metadata
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Geographical outline of place
        /// </summary>
        public Geometry BoundingBox { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public Geometry Geometry { get; set; }

        /// <summary>
        /// ?
        /// </summary>
        public List<string> PolyLines { get; set; }

        /// <summary>
        /// Containing place (i.e. a neighborhood is contained within a city)
        /// </summary>
        public Place ContainedWithin { get; set; }
    }
}
