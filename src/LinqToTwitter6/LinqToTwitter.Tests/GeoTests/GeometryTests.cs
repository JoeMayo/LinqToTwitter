using LinqToTwitter.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LinqToTwitter.Tests.GeoTests
{
    [TestClass]
    public class GeometryTests
    {
        public GeometryTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void Geometry_Ctor_Parses_JsonData()
        {
            var geometryJson = JsonDocument.Parse(GeometryJson).RootElement;

            var geometry = new Geometry(geometryJson);

            Assert.AreEqual("Polygon", geometry.Type);
            Assert.IsNotNull(geometry.Coordinates);
            Assert.AreEqual(4, geometry.Coordinates.Count);
            Coordinate coord = geometry.Coordinates.First();
            Assert.AreEqual(-122.51368188, coord.Longitude);
            Assert.AreEqual(37.70813196, coord.Latitude);
        }

        [TestMethod]
        public void Geometry_Ctor_Returns_On_Null_JsonData()
        {
            var geometry = new Geometry(JsonDocument.Parse("null").RootElement);

            Assert.AreEqual(null, geometry.Type);
        }

        const string GeometryJson = @"{
                     ""type"":""Polygon"",
                     ""coordinates"":[
                        [
                           [
                              -122.51368188,
                              37.70813196
                           ],
                           [
                              -122.35845384,
                              37.70813196
                           ],
                           [
                              -122.35845384,
                              37.83245301
                           ],
                           [
                              -122.51368188,
                              37.83245301
                           ]
                        ]
                     ]
                  }";

    }
}
