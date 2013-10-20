using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using LitJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.GeoTests
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
            var geometryJson = JsonMapper.ToObject(GeometryJson);

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
            var geometry = new Geometry(null);

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
