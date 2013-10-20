using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using LitJson;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.GeoTests
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
            var coordJson = JsonMapper.ToObject(CoordinateJson);

            var coord = new Coordinate(coordJson);

            Assert.AreEqual(-122.51368188, coord.Longitude);
            Assert.AreEqual(37.70813196, coord.Latitude);
        }

        [TestMethod]
        public void Coordinate_Ctor_Returns_Null_JsonData()
        {
            var coord = new Coordinate(null);

            Assert.AreEqual(0, coord.Longitude);
            Assert.AreEqual(0, coord.Latitude);
        }

        const string CoordinateJson = @"[
                              -122.51368188,
                              37.70813196
                           ]";
    }
}
