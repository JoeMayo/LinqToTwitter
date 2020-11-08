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
    public class CoordinateTests
    {
        public CoordinateTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void Coordinate_Ctor_Parses_JsonData()
        {
            var coordJson = JsonDocument.Parse(CoordinateJson).RootElement;

            var coord = new Coordinate(coordJson);

            Assert.AreEqual(-122.51368188, coord.Longitude);
            Assert.AreEqual(37.70813196, coord.Latitude);
        }

        [TestMethod]
        public void Coordinate_Ctor_Returns_Null_JsonData()
        {
            var coord = new Coordinate(JsonDocument.Parse("null").RootElement);

            Assert.AreEqual(0, coord.Longitude);
            Assert.AreEqual(0, coord.Latitude);
        }

        const string CoordinateJson = @"[
                              -122.51368188,
                              37.70813196
                           ]";
    }
}
