using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using LitJson;
using Xunit;

namespace LinqToTwitterXUnitTests.GeoTests
{
    public class CoordinateTests
    {
        public CoordinateTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void Coordinate_Ctor_Parses_JsonData()
        {
            var coordJson = JsonMapper.ToObject(CoordinateJson);

            var coord = new Coordinate(coordJson);

            Assert.Equal(-122.51368188, coord.Longitude);
            Assert.Equal(37.70813196, coord.Latitude);
        }

        [Fact]
        public void Coordinate_Ctor_Returns_Null_JsonData()
        {
            var coord = new Coordinate(null);

            Assert.Equal(0, coord.Longitude);
            Assert.Equal(0, coord.Latitude);
        }

        const string CoordinateJson = @"[
                              -122.51368188,
                              37.70813196
                           ]";
    }
}
