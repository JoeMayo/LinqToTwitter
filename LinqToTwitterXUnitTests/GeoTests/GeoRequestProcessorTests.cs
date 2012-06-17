using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.GeoTests
{
    public class GeoRequestProcessorTests
    {
        public GeoRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void GetParameters_Parses_Parameters()
        {
            var geoReqProc = new GeoRequestProcessor<Geo>();
            Expression<Func<Geo, bool>> expression =
                geo =>
                    geo.Type == GeoType.Reverse &&
                    geo.Accuracy == "10" &&
                    geo.Granularity == "city" &&
                    geo.Latitude == 37.78215 &&
                    geo.Longitude == -122.40060 &&
                    geo.MaxResults == 10 &&
                    geo.ID == "456" &&
                    geo.IP == "168.143.171.180" &&
                    geo.Query == "place" &&
                    geo.ContainedWithin == "abc" &&
                    geo.Attribute == "street_address=123" &&
                    geo.PlaceName == "placeName";

            var lambdaExpression = expression as LambdaExpression;

            var queryParams = geoReqProc.GetParameters(lambdaExpression);

            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)GeoType.Reverse).ToString())));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Accuracy", "10")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Granularity", "city")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Latitude", (37.78215).ToString())));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Longitude", (-122.4006).ToString())));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("MaxResults", "10")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "456")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IP", "168.143.171.180")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Query", "place")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ContainedWithin", "abc")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Attribute", "street_address=123")));
            Assert.True(
                queryParams.Contains(
                    new KeyValuePair<string, string>("PlaceName", "placeName")));
        }

        [Fact]
        public void BuildUrl_Constructs_Reverse_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/geo/reverse_geocode.json?lat=37.78215&long=-122.4006&accuracy=city&granularity=10&max_results=10";
            var geoReqProc = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.Reverse).ToString()},
                 {"Latitude", "37.78215"},
                 {"Longitude", "-122.40060"},
                 {"Accuracy", "city" },
                 {"Granularity", "10" },
                 {"MaxResults", "10" }
             };

            Request req = geoReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_ReverseL_Without_LatLong_Throws()
        {
            const string ExpectedLatLongParamName = "LatLong";
            var geoReqProc = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.Reverse).ToString()},
                 {"Accuracy", "city" },
                 {"Granularity", "10" },
                 {"MaxResults", "10" }
             };

            var ex = Assert.Throws<ArgumentException>(() => geoReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedLatLongParamName, ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Constructs_ID_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/geo/id/7695dd2ec2f86f2b.json";
            var geoReqProc = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.ID).ToString()},
                 {"ID", "7695dd2ec2f86f2b"},
             };

            Request req = geoReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_ID_Without_ID_Throws()
        {
            const string ExpectedIDParamName = "ID";
            var geoReqProc = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.ID).ToString()},
             };

            var ex = Assert.Throws<ArgumentException>(() => geoReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedIDParamName, ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Constructs_Search_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/geo/search.json?lat=37.78215&long=-122.4006&query=Twitter%20HQ&ip=168.143.171.180&accuracy=city&granularity=10&max_results=10&contained_within=123&attribute%3Astreet_address=123";
            var geoReqProc = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.Search).ToString()},
                 {"Latitude", (37.78215).ToString()},
                 {"Longitude", (-122.40060).ToString()},
                 {"Query", "Twitter HQ" },
                 {"IP", "168.143.171.180"},
                 {"Accuracy", "city" },
                 {"Granularity", "10" },
                 {"MaxResults", "10" },
                 {"ContainedWithin", "123" },
                 {"Attribute", "street_address=123" }
             };

            Request req = geoReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_Search_With_Attribute_Missing_Equals_Symbol_Throws()
        {
            const string ExpectedAttributeParam = "Attribute";
            var geoReqProc = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.Search).ToString()},
                 {"IP", "168.143.171.180"},
                 {"Attribute", "street_address" }
             };

             var ex = Assert.Throws<ArgumentException>(() => geoReqProc.BuildUrl(parameters));

             Assert.Equal(ExpectedAttributeParam, ex.ParamName);
        }

        [Fact]
        public void BuildUrl_Constructs_Similar_Places_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1/geo/similar_places.json?lat=37.78215&long=-122.4006&name=placeName&contained_within=123&attribute%3Astreet_address=123";
            var geoReqProc = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.SimilarPlaces).ToString()},
                 {"Latitude", (37.78215).ToString()},
                 {"Longitude", (-122.40060).ToString()},
                 {"PlaceName", "placeName" },
                 {"ContainedWithin", "123" },
                 {"Attribute", "street_address=123" }
             };

            Request req = geoReqProc.BuildUrl(parameters);

            Assert.Equal(ExpectedUrl, req.FullUrl);
        }

        [Fact]
        public void BuildUrl_For_Similar_Places_Requires_Lat_And_Long()
        {
            const string ExpectedParamName = "LatLong";
            var geoReqProc = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.SimilarPlaces).ToString()},
                 //{"Latitude", "37.78215"},
                 //{"Longitude", "-122.40060"},
                 {"PlaceName", "placeName" },
                 {"ContainedWithin", "123" },
                 {"Attribute", "street_address=123" }
             };

            var ex = Assert.Throws<ArgumentException>(() => geoReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void BuildUrl_For_Similar_Places_Requires_PlaceName()
        {
            const string ExpectedParamName = "PlaceName";
            var geoReqProc = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.SimilarPlaces).ToString()},
                 {"Latitude", "37.78215"},
                 {"Longitude", "-122.40060"},
                 //{"PlaceName", "placeName" },
                 {"ContainedWithin", "123" },
                 {"Attribute", "street_address=123" }
             };

            var ex = Assert.Throws<ArgumentException>(() => geoReqProc.BuildUrl(parameters));

            Assert.Equal(ExpectedParamName, ex.ParamName);
        }

        [Fact]
        public void GeoRequestProcessor_Processes_Json_Format_Responses()
        {
            var geoReqProc = new GeoRequestProcessor<Geo>();

            Assert.IsAssignableFrom<IRequestProcessorWantsJson>(geoReqProc);
        }

        [Fact]
        public void ProcessResults_Handles_ReverseGeoCode_Response()
        {
            const int ExpectedPlacesCount = 4;
            var geoReqProc = new GeoRequestProcessor<Geo>();

            List<Geo> geo = geoReqProc.ProcessResults(MultiPlaceResponse);

            Assert.NotNull(geo);
            Assert.Single(geo);
            Assert.NotNull(geo.Single().Places);
            Assert.Equal(ExpectedPlacesCount, geo.Single().Places.Count);
        }

        [Fact]
        public void ProcessResults_Handles_ID_Response()
        {
            const string ExpectedPlaceName = "San Francisco";
            var geoReqProc = new GeoRequestProcessor<Geo> { Type = GeoType.ID };

            List<Geo> geo = geoReqProc.ProcessResults(IDResponse);

            Assert.NotNull(geo);
            Assert.Single(geo);
            Assert.NotNull(geo.Single().Places);
            Assert.Equal(ExpectedPlaceName, geo.Single().Places.First().Name);
        }

        [Fact]
        public void ProcessResults_Handles_Search_Response()
        {
            const int ExpectedPlacesCount = 4;
            const string ExpectedPlaceFullName = "SoMa, San Francisco";
            var geoReqProc = new GeoRequestProcessor<Geo> { Type = GeoType.Search };

            List<Geo> geo = geoReqProc.ProcessResults(MultiPlaceResponse);

            Assert.NotNull(geo);
            Assert.Single(geo);
            Assert.NotNull(geo.Single().Places);
            Assert.Equal(ExpectedPlacesCount, geo.Single().Places.Count);
            Assert.Equal(ExpectedPlaceFullName, geo.Single().Places.First().FullName);
        }

        [Fact]
        public void ProcessResults_Handles_Simlar_Places_Response()
        {
            const int ExpectedPlacesCount = 4;
            const string ExpectedPlaceFullName = "SoMa, San Francisco";
            const string ExpectedToken = "15f3c6f2b94ba19faee70d9d61aaebee";
            var geoReqProc = new GeoRequestProcessor<Geo> { Type = GeoType.SimilarPlaces };

            List<Geo> geoResponse = geoReqProc.ProcessResults(MultiPlaceResponse);

            Assert.NotNull(geoResponse);
            Assert.Single(geoResponse);
            var geo = geoResponse.Single();
            Assert.Equal(ExpectedToken, geo.Token);
            var places = geo.Places;
            Assert.NotNull(places);
            Assert.Equal(ExpectedPlacesCount, places.Count);
            Assert.Equal(ExpectedPlaceFullName, places.First().FullName);
        }

        [Fact]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var geoReqProc = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };

            var geo = geoReqProc.ProcessResults(string.Empty);

            Assert.NotNull(geo);
            Assert.Empty(geo);
        }

        const string MultiPlaceResponse = @"{
   ""query"":{
      ""type"":""reverse_geocode"",
      ""params"":{
         ""accuracy"":0,
         ""granularity"":""neighborhood"",
         ""coordinates"":{
            ""type"":""Point"",
            ""coordinates"":[
               -122.4006,
               37.78215
            ]
         }
      },
      ""url"":""http:\/\/api.twitter.com\/1\/geo\/reverse_geocode.json?long=-122.4006&accuracy=0&granularity=neighborhood&lat=37.78215""
   },
   ""result"":{
      ""token"":""15f3c6f2b94ba19faee70d9d61aaebee"",
      ""places"":[
         {
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
            ""attributes"":{

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
         },
         {
            ""contained_within"":[
               {
                  ""country"":""United States"",
                  ""place_type"":""admin"",
                  ""name"":""California"",
                  ""bounding_box"":{
                     ""type"":""Polygon"",
                     ""coordinates"":[
                        [
                           [
                              -124.482003,
                              32.528832
                           ],
                           [
                              -114.131211,
                              32.528832
                           ],
                           [
                              -114.131211,
                              42.009517
                           ],
                           [
                              -124.482003,
                              42.009517
                           ]
                        ]
                     ]
                  },
                  ""attributes"":{

                  },
                  ""full_name"":""California, US"",
                  ""country_code"":""US"",
                  ""url"":""http:\/\/api.twitter.com\/1\/geo\/id\/fbd6d2f5a4e4a15e.json"",
                  ""id"":""fbd6d2f5a4e4a15e""
               }
            ],
            ""place_type"":""city"",
            ""country"":""United States"",
            ""name"":""San Francisco"",
            ""full_name"":""San Francisco, CA"",
            ""attributes"":{

            },
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
            ""country_code"":""US"",
            ""url"":""http:\/\/api.twitter.com\/1\/geo\/id\/5a110d312052166f.json"",
            ""id"":""5a110d312052166f""
         },
         {
            ""contained_within"":[
               {
                  ""country"":""United States"",
                  ""place_type"":""country"",
                  ""name"":""United States"",
                  ""bounding_box"":null,
                  ""attributes"":{

                  },
                  ""full_name"":""United States"",
                  ""country_code"":""US"",
                  ""url"":""http:\/\/api.twitter.com\/1\/geo\/id\/96683cc9126741d1.json"",
                  ""id"":""96683cc9126741d1""
               }
            ],
            ""place_type"":""admin"",
            ""country"":""United States"",
            ""name"":""California"",
            ""full_name"":""California, US"",
            ""attributes"":{

            },
            ""bounding_box"":{
               ""type"":""Polygon"",
               ""coordinates"":[
                  [
                     [
                        -124.482003,
                        32.528832
                     ],
                     [
                        -114.131211,
                        32.528832
                     ],
                     [
                        -114.131211,
                        42.009517
                     ],
                     [
                        -124.482003,
                        42.009517
                     ]
                  ]
               ]
            },
            ""country_code"":""US"",
            ""url"":""http:\/\/api.twitter.com\/1\/geo\/id\/fbd6d2f5a4e4a15e.json"",
            ""id"":""fbd6d2f5a4e4a15e""
         },
         {
            ""contained_within"":[

            ],
            ""place_type"":""country"",
            ""country"":""United States"",
            ""name"":""United States"",
            ""full_name"":""United States"",
            ""attributes"":{

            },
            ""bounding_box"":null,
            ""country_code"":""US"",
            ""url"":""http:\/\/api.twitter.com\/1\/geo\/id\/96683cc9126741d1.json"",
            ""id"":""96683cc9126741d1""
         }
      ]
   }
}";

        const string IDResponse = @"{
   ""country"":""United States"",
   ""place_type"":""city"",
   ""name"":""San Francisco"",
   ""geometry"":{
      ""type"":""Polygon"",
      ""coordinates"":[
         [
            [
               -122.46628284,
               37.70813196
            ],
            [
               -122.47088292,
               37.70823204
            ],
            [
               -122.48518284,
               37.71123201
            ],
            [
               -122.48948304,
               37.712232
            ],
            [
               -122.497983,
               37.71713196
            ],
            [
               -122.49978012,
               37.71960696
            ],
            [
               -122.50198296,
               37.723131
            ],
            [
               -122.50787796,
               37.73529
            ],
            [
               -122.51368188,
               37.77856803
            ],
            [
               -122.51305692,
               37.78252398
            ],
            [
               -122.47808292,
               37.81082799
            ],
            [
               -122.47738308,
               37.81102797
            ],
            [
               -122.37322716,
               37.83245301
            ],
            [
               -122.36652288,
               37.83081303
            ],
            [
               -122.365395,
               37.82917296
            ],
            [
               -122.36177592,
               37.82153403
            ],
            [
               -122.35845384,
               37.81450503
            ],
            [
               -122.377842,
               37.72782
            ],
            [
               -122.38322616,
               37.70940303
            ],
            [
               -122.39378208,
               37.70823096
            ],
            [
               -122.43398184,
               37.70813196
            ],
            [
               -122.46628284,
               37.70813196
            ]
         ]
      ]
   },
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
      ""189390:id"":""san-francisco-ca"",
      ""162772:pop100"":""776733"",
      ""162772:place_id"":""0667000""
   },
   ""full_name"":""San Francisco, CA"",
   ""polylines"":[
      ""yzceFfenjVSv[wQjxAgEzYs]bt@oNfJ_UvL_kAzc@omGfc@uW\u007BB\u007DoDsyEg@kC\u007BdC\u007DiSfI\u007Dh@fIaFvn@qUzj@ySx|OdxBbrBt`@hF|`ARfzF?zhE""
   ],
   ""country_code"":""US"",
   ""url"":""http:\/\/api.twitter.com\/1\/geo\/id\/5a110d312052166f.json"",
   ""id"":""5a110d312052166f"",
   ""contained_within"":[
      {
         ""country"":""United States"",
         ""place_type"":""admin"",
         ""name"":""California"",
         ""bounding_box"":{
            ""type"":""Polygon"",
            ""coordinates"":[
               [
                  [
                     -124.482003,
                     32.528832
                  ],
                  [
                     -114.131211,
                     32.528832
                  ],
                  [
                     -114.131211,
                     42.009517
                  ],
                  [
                     -124.482003,
                     42.009517
                  ]
               ]
            ]
         },
         ""attributes"":{

         },
         ""full_name"":""California, US"",
         ""country_code"":""US"",
         ""url"":""http:\/\/api.twitter.com\/1\/geo\/id\/fbd6d2f5a4e4a15e.json"",
         ""id"":""fbd6d2f5a4e4a15e""
      }
   ]
}";

    }
}
