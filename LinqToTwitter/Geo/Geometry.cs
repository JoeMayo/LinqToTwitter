using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Geographical area
    /// </summary>
    public class Geometry
    {
        /// <summary>
        /// Converts XML to a new Geometry
        /// </summary>
        /// <param name="geometry">XML to convert</param>
        /// <returns>Geometry containing info from XML</returns>
        public Geometry CreateGeometry(XElement geometry)
        {
            if (geometry == null)
            {
                return null;
            }

            var coordinate = new Coordinate();

            return new Geometry
            {
                Type = geometry.Element("type").Value,
                Coordinates =
                    (from coord in geometry.Element("coordinates").Element("item").Elements("item")
                     select coordinate.CreateCoordinate(coord))
                     .ToList()
            };
        }

        /// <summary>
        /// Type of bouding box
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Coordinates for bounding box
        /// </summary>
        public List<Coordinate> Coordinates { get; set; }
    }
}
