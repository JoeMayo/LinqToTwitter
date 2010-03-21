using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace LinqToTwitter
{
    /// <summary>
    /// Geographical coordinates
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// Converts XML to a Coordinate
        /// </summary>
        /// <param name="coordinate">XML to convert</param>
        /// <returns>Coordinate holding info from XML</returns>
        public Coordinate CreateCoordinate(XElement coordinate)
        {
            const int Latitude = 1;
            const int Longitude = 0;

            List<XElement> coords = coordinate.Elements("item").ToList();

            return new Coordinate
            {
                Latitude = decimal.Parse(coords[Latitude].Value),
                Longitude = decimal.Parse(coords[Longitude].Value)
            };
        }

        /// <summary>
        /// Latitude
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public decimal Longitude { get; set; }
    }
}
