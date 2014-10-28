using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using LitJson;
using Xunit;

namespace LinqToTwitterXUnitTests.GeoTests
{
    public class PlaceTests
    {
        public PlaceTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void Place_Ctor_Parses_JsonData()
        {
            const string ExpectedPolyLine = "yzceFfenjVSv[wQjxAgEzYs]bt@oNfJ_UvL_kAzc@omGfc@uW\u007BB\u007DoDsyEg@kC\u007BdC\u007DiSfI\u007Dh@fIaFvn@qUzj@ySx|OdxBbrBt`@hF|`ARfzF?zhE";
            var placeJson = JsonMapper.ToObject(PlaceJson);

            var place = new Place(placeJson);

            Assert.NotNull(place.ContainedWithin);
            Assert.Equal("United States", place.ContainedWithin.Country);

            Assert.Equal("neighborhood", place.PlaceType);
            Assert.Equal("United States", place.Country);
            Assert.Equal("SoMa", place.Name);
            Assert.Equal("SoMa, San Francisco", place.FullName);

            Assert.NotNull(place.Attributes);
            var attrDict = place.Attributes;
            Assert.Equal(3, attrDict.Count);
            Assert.Equal(attrDict["street_address"], "795 Folsom St");
            Assert.Equal(attrDict["623:id"], "210176");
            Assert.Equal(attrDict["twitter"], "JoeMayo");

            Assert.NotNull(place.BoundingBox);
            Assert.Equal("Polygon", place.BoundingBox.Type);

            Assert.Equal("US", place.CountryCode);
            Assert.Equal("http://api.twitter.com/1/geo/id/2b6ff8c22edd9576.json", place.Url);
            Assert.Equal("2b6ff8c22edd9576", place.ID);

            Assert.NotNull(place.PolyLines);
            Assert.Single(place.PolyLines);
            Assert.Equal(ExpectedPolyLine, place.PolyLines.First());
        }

        [Fact]
        public void Place_Ctor_Returns_On_Null_JsonData()
        {
            var place = new Place(null);

            Assert.Null(place.Country);
        }

        const string PlaceJson = @"{
            ""contained_within"":[
               {
                  ""country"":""United States"",
                  ""place_type"":""city"",
                  ""name"":""San Francisco"",
                  ""bounding_box"":{
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
                  },
                  ""attributes"":{

                  },
                  ""full_name"":""San Francisco, CA"",
                  ""country_code"":""US"",
                  ""url"":""http:\/\/api.twitter.com\/1\/geo\/id\/5a110d312052166f.json"",
                  ""id"":""5a110d312052166f""
               }
            ],
            ""place_type"":""neighborhood"",
            ""country"":""United States"",
            ""name"":""SoMa"",
            ""full_name"":""SoMa, San Francisco"",
            ""polylines"":[
               ""yzceFfenjVSv[wQjxAgEzYs]bt@oNfJ_UvL_kAzc@omGfc@uW\u007BB\u007DoDsyEg@kC\u007BdC\u007DiSfI\u007Dh@fIaFvn@qUzj@ySx|OdxBbrBt`@hF|`ARfzF?zhE""
            ],
            ""attributes"":{
               ""street_address"": ""795 Folsom St"",
                ""623:id"": ""210176"",
                ""twitter"": ""JoeMayo""
            },
            ""bounding_box"":{
               ""type"":""Polygon"",
               ""coordinates"":[
                  [
                     [
                        -122.42284884,
                        37.76893497
                     ],
                     [
                        -122.3964,
                        37.76893497
                     ],
                     [
                        -122.3964,
                        37.78752897
                     ],
                     [
                        -122.42284884,
                        37.78752897
                     ]
                  ]
               ]
            },
            ""country_code"":""US"",
            ""url"":""http:\/\/api.twitter.com\/1\/geo\/id\/2b6ff8c22edd9576.json"",
            ""id"":""2b6ff8c22edd9576""
         }";
    }
}
