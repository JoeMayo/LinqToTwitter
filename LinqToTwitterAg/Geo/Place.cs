using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using System.Xml.Serialization;
using LinqToTwitter.Common;

using LitJson;

namespace LinqToTwitter
{
    /// <summary>
    /// A general description of a geographical location in Twitter
    /// </summary>
    public class Place
    {
        public Place() {}
        internal Place(JsonData place)
        {
            if (place == null) return;

            ID = place.GetValue<string>("id");
            Name = place.GetValue<string>("name");
            Country = place.GetValue<string>("country");
            CountryCode = place.GetValue<string>("country_code");
            FullName = place.GetValue<string>("full_name");
            PlaceType = place.GetValue<string>("place_type");
            Url = place.GetValue<string>("url");
            BoundingBox = new Geometry(place.GetValue<JsonData>("bounding_box"));
            Geometry = new Geometry(place.GetValue<JsonData>("geometry"));

            var containedWithin = place.GetValue<JsonData>("contained_within");
            ContainedWithin = 
                containedWithin != null && containedWithin.Count > 0 ? 
                    new Place(containedWithin[0]) :
                    null;

            var polyLines = place.GetValue<JsonData>("polylines");
            PolyLines = 
                polyLines == null ? 
                    new List<string>() 
                        : 
                    (from JsonData line in polyLines
                     select line.ToString())
                    .ToList();

            var attrDict = place.GetValue<JsonData>("attributes") as IDictionary<string, JsonData>;
            Attributes =
                attrDict == null ?
                    new Dictionary<string, string>() 
                        :
                    (from string key in attrDict.Keys
                     select new 
                     { 
                         Key = key, 
                         Val = attrDict[key].ToString()
                     })
                    .ToDictionary(
                        attr => attr.Key,
                        attr => attr.Val);
        }

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
                //PolyLines = 
                //    place.Element("polylines") == null ?
                //        string.Empty :
                //        place.Element("polylines").Element("item").Value
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
