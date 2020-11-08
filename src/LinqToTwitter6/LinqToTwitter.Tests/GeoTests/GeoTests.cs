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
    public class GeoTests
    {
        public GeoTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void Geo_Ctor_Parses_JsonData()
        {
            var geoJson = JsonDocument.Parse(ReverseResponse).RootElement;

            var geo = new Geo(geoJson);

            Assert.IsNotNull(geo.Places);
            var places = geo.Places;
            Assert.AreEqual(4, places.Count);
            Assert.AreEqual("neighborhood", places.First().PlaceType);
        }

        [TestMethod]
        public void Geo_Ctor_Returns_On_Null_JsonData()
        {
            var geo = new Geo(JsonDocument.Parse("null").RootElement);

            Assert.IsNull(geo.Places);
        }

        const string ReverseResponse = @"{
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
    }
}
