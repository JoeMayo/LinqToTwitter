using System.Collections.Generic;
using System.Linq;

using LinqToTwitter.Common;
using System.Xml.Serialization;
using System.Text.Json;

namespace LinqToTwitter
{
    /// <summary>
    /// Geographical area
    /// </summary>
    [XmlType(Namespace = "LinqToTwitter")]
    public class Geometry
    {
        public Geometry() {}
        public Geometry(JsonElement geometry)
        {
            if (geometry.IsNull()) return;

            Type = geometry.GetString("type");

            geometry.TryGetProperty("coordinates", out JsonElement coordinates);
            Coordinates =
                (from outer in coordinates.EnumerateArray()
                 from coord in outer.EnumerateArray()
                 select new Coordinate(coord))
                .ToList();
        }

        /// <summary>
        /// Type of bouding box
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Coordinates for bounding box
        /// </summary>
        public List<Coordinate>? Coordinates { get; set; }
    }
}
