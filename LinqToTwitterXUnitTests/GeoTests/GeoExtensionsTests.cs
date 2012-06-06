using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.GeoTests
{
    public class GeoExtensionsTests
    {
        [Fact]
        public void GeoRequestProcessor_Works_With_Actions()
        {
            var geoReqProc = new GeoRequestProcessor<Geo>();

            Assert.IsAssignableFrom<IRequestProcessorWithAction<Geo>>(geoReqProc);
        }
  
        TwitterContext InitTwitterContext()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => exec.ExecuteTwitter(
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<Func<string, Place>>()))
                    .Returns(PlaceResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            return ctx;
        }

        [Fact]
        public void CreatePlace_Calls_Executor_Execute()
        {
            const string Name = "Twitter HQ Bird Feeder";
            const string ContainedWithin = "5a110d312052166f";
            const string Token = "15f3c6f2b94ba19faee70d9d61aaebee";
            const double Latitude = 37.78215;
            const double Longitude = -122.4006;
            const string Attribute = "street_address=123";
            const string ExpectedName = "SoMa";
            var ctx = InitTwitterContext();

            Place place = ctx.CreatePlace(Name, ContainedWithin, Token, Latitude, Longitude, Attribute);

            Assert.Equal(ExpectedName, place.Name);
        }

        [Fact]
        public void CreatePlace_Requires_Name()
        {
            //const string Name = "Twitter HQ Bird Feeder";
            const string ContainedWithin = "5a110d312052166f";
            const string Token = "15f3c6f2b94ba19faee70d9d61aaebee";
            const double Latitude = -122.4006;
            const double Longitude = 37.78215;
            const string Attribute = "street_address=123";
            const string ExpectedParamName = "name";
            var ctx = InitTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.CreatePlace(null, ContainedWithin, Token, Latitude, Longitude, Attribute));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void CreatePlace_Requires_ContainedWithin()
        {
            const string Name = "Twitter HQ Bird Feeder";
            //const string ContainedWithin = "5a110d312052166f";
            const string Token = "15f3c6f2b94ba19faee70d9d61aaebee";
            const double Latitude = -122.4006;
            const double Longitude = 37.78215;
            const string Attribute = "street_address=123";
            const string ExpectedParamName = "containedWithin";
            var ctx = InitTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.CreatePlace(Name, null, Token, Latitude, Longitude, Attribute));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void CreatePlace_Requires_Token()
        {
            const string Name = "Twitter HQ Bird Feeder";
            const string ContainedWithin = "5a110d312052166f";
            //const string Token = "15f3c6f2b94ba19faee70d9d61aaebee";
            const double Latitude = -122.4006;
            const double Longitude = 37.78215;
            const string Attribute = "street_address=123";
            const string ExpectedParamName = "token";
            var ctx = InitTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.CreatePlace(Name, ContainedWithin, null, Latitude, Longitude, Attribute));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void CreatePlace_Requires_Latitude()
        {
            const string Name = "Twitter HQ Bird Feeder";
            const string ContainedWithin = "5a110d312052166f";
            const string Token = "15f3c6f2b94ba19faee70d9d61aaebee";
            //const double Latitude = -122.4006;
            const double Longitude = 37.78215;
            const string Attribute = "street_address=123";
            const string ExpectedParamName = "latitude";
            var ctx = InitTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.CreatePlace(Name, ContainedWithin, Token, 0, Longitude, Attribute));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void CreatePlace_Requires_Longitude()
        {
            const string Name = "Twitter HQ Bird Feeder";
            const string ContainedWithin = "5a110d312052166f";
            const string Token = "15f3c6f2b94ba19faee70d9d61aaebee";
            const double Latitude = -122.4006;
            //const double Longitude = 37.78215;
            const string Attribute = "street_address=123";
            const string ExpectedParamName = "longitude";
            var ctx = InitTwitterContext();

            var ex = Assert.Throws<ArgumentException>(() => ctx.CreatePlace(Name, ContainedWithin, Token, Latitude, 0, Attribute));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        const string PlaceResponse = @"{
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
