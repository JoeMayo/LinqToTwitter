using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using LitJson;
using Xunit;

namespace LinqToTwitterXUnitTests.GeoTests
{
    public class GeometryTests
    {
        public GeometryTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void Geometry_Ctor_Parses_JsonData()
        {
            var geometryJson = JsonMapper.ToObject(GeometryJson);

            var geometry = new Geometry(geometryJson);

            Assert.Equal("Polygon", geometry.Type);
            Assert.NotNull(geometry.Coordinates);
            Assert.Equal(4, geometry.Coordinates.Count);
            Coordinate coord = geometry.Coordinates.First();
            Assert.Equal(-122.51368188, coord.Longitude);
            Assert.Equal(37.70813196, coord.Latitude);
        }

        [Fact]
        public void Geometry_Ctor_Returns_On_Null_JsonData()
        {
            var geometry = new Geometry(null);

            Assert.Equal(null, geometry.Type);
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
