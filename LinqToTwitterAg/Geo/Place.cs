using System;
using System.Collections.Generic;
using System.Linq;

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
