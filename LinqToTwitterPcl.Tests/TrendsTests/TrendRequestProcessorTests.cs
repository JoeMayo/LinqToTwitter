using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterPcl.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.TrendTests
{
    [TestClass]
    public class TrendRequestProcessorTests
    {
        public TrendRequestProcessorTests()
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void GetParameters_Collects_Parameters()
        {
            var trendReqProc = new TrendRequestProcessor<Trend>();
            Expression<Func<Trend, bool>> expression =
                trend =>
                    trend.Type == TrendType.Available &&
                    trend.WoeID == 1 &&
                    trend.Latitude == 1.2 &&
                    trend.Longitude == 3.4 &&
                    trend.Exclude == true;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = trendReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("WoeID", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Latitude", "1.2")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Longitude", "3.4")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Exclude", "True")));
        }

        [TestMethod]
        public void ProcessResults_Populates_Input_Parameters()
        {
            var trendProc = new TrendRequestProcessor<Trend>()
            {
                BaseUrl = "https://api.twitter.com/1.1/",
                Type = TrendType.Place,
                Exclude = true,
                Latitude = 1.1,
                Longitude = 2.2,
                WoeID = 1
            };

            var trends = trendProc.ProcessResults(TestTrendQueryResponse);

            Assert.IsNotNull(trends);
            Assert.IsTrue(trends.Any());
            var trend = trends.First();
            Assert.IsNotNull(trend);
            Assert.IsTrue(trend.Exclude);
            Assert.AreEqual(1.1, trend.Latitude);
            Assert.AreEqual(2.2, trend.Longitude);
            Assert.AreEqual(1, trend.WoeID);
        }

        [TestMethod]
        public void BuildUrl_Handles_Available_Trends()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/trends/available.json";
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture) }
                };

            Request req = trendReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Constructs_AvailableTrends_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/trends/available.json";
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture) }
                };

            Request req = trendReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Constructs_ClosestTrends_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/trends/closest.json?lat=37.78215&long=-122.40060";
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Closest).ToString(CultureInfo.InvariantCulture) },
                    { "Latitude", "37.78215" },
                    { "Longitude", "-122.40060" }
                };

            Request req = trendReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Place_Trends_Url()
        {
            const string ExpectedUrl = "https://api.twitter.com/1.1/trends/place.json?id=1";
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Place).ToString(CultureInfo.InvariantCulture) },
                    { "WoeID", "1" }
                };

            Request req = trendReqProc.BuildUrl(parameters);

            Assert.AreEqual(ExpectedUrl, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Throws_On_Place_Trends_WithoutWoeID()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Place).ToString(CultureInfo.InvariantCulture) },
                };

            var ex = L2TAssert.Throws<ArgumentException>(() => trendReqProc.BuildUrl(parameters));

            Assert.AreEqual("WoeID", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Throws_When_Type_Not_Provided()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };
            var parameters = new Dictionary<string, string>();

            var ex = L2TAssert.Throws<ArgumentException>(() => trendReqProc.BuildUrl(parameters));

            Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void BuildUrl_Throws_With_No_Parameters()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1.1/" };

            var ex = L2TAssert.Throws<ArgumentException>(() => trendReqProc.BuildUrl(null));

            Assert.AreEqual("Type", ex.ParamName);
        }

        [TestMethod]
        public void ProcessResults_Handles_Available()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { Type = TrendType.Available };

            List<Trend> trendList = trendReqProc.ProcessResults(TestAvailableOrClosestQueryResponse);

            Assert.IsNotNull(trendList);
            var trends = trendList.SingleOrDefault();
            Assert.IsNotNull(trends);
            var locations = trends.Locations;
            Assert.IsNotNull(locations);
            Assert.AreEqual(2, locations.Count);
            Location location = locations[0];
            Assert.AreEqual("Country", location.PlaceTypeName);
            Assert.AreEqual(12, location.PlaceTypeNameCode);
            Assert.AreEqual("Turkey", location.Name);
            Assert.AreEqual("http://where.yahooapis.com/v1/place/23424969", location.Url);
            Assert.AreEqual(23424969, location.WoeID);
            Assert.AreEqual("TR", location.CountryCode);
            Assert.AreEqual("1", location.ParentID);
            Assert.AreEqual("Turkey", location.Country);
        }

        [TestMethod]
        public void ProcessResults_Handles_Closest()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { Type = TrendType.Closest };

            List<Trend> trendList = trendReqProc.ProcessResults(TestAvailableOrClosestQueryResponse);

            Assert.IsNotNull(trendList);
            var trends = trendList.SingleOrDefault();
            Assert.IsNotNull(trends);
            var locations = trends.Locations;
            Assert.IsNotNull(locations);
            Assert.AreEqual(2, locations.Count);
            Location location = locations[1];
            Assert.IsNotNull(location);
            Assert.AreEqual("Town", location.PlaceTypeName);
            Assert.AreEqual(7, location.PlaceTypeNameCode);
            Assert.AreEqual("Birmingham", location.Name);
            Assert.AreEqual("http://where.yahooapis.com/v1/place/2364559", location.Url);
            Assert.AreEqual(2364559, location.WoeID);
            Assert.AreEqual("US", location.CountryCode);
            Assert.AreEqual("23424977", location.ParentID);
            Assert.AreEqual("United States", location.Country);
        }

        [TestMethod]
        public void ProcessResults_Parses_Location()
        {
            var trendProc = new TrendRequestProcessor<Trend> { Type = TrendType.Place };

            List<Trend> trendList = trendProc.ProcessResults(TestTrendQueryResponse);

            Assert.IsNotNull(trendList);
            var trends = trendList.FirstOrDefault();
            Assert.IsNotNull(trends);
            var locations = trends.Locations;
            Assert.IsNotNull(locations);
            Assert.AreEqual(1, locations.Count);
            Location location = locations[0];
            Assert.AreEqual("Worldwide", location.Name);
            Assert.AreEqual(1, location.WoeID);
            Assert.AreEqual("%22Julianna%20Margulies%22", trends.Query);
            Assert.AreEqual("Julianna Margulies", trends.Name);
            Assert.IsNull(trends.PromotedContent);
            Assert.AreEqual("http://twitter.com/search/%22Julianna%20Margulies%22", trends.SearchUrl);
            Assert.IsNull(trends.Events);
            Assert.AreEqual(new DateTime(2011, 9, 19, 2, 4, 39), trends.AsOf);
            Assert.AreEqual(new DateTime(2011, 9, 19, 2, 0, 17), trends.CreatedAt);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var trendProc = new TrendRequestProcessor<Trend>();

            var trends = trendProc.ProcessResults(string.Empty);

            Assert.AreEqual(0, trends.Count);
        }

        const string TestTrendQueryResponse = @"[
   {
      ""as_of"":""2011-09-19T02:04:39Z"",
      ""locations"":[
         {
            ""name"":""Worldwide"",
            ""woeid"":1
         }
      ],
      ""trends"":[
         {
            ""query"":""%22Julianna%20Margulies%22"",
            ""name"":""Julianna Margulies"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Julianna%20Margulies%22"",
            ""events"":null
         },
         {
            ""query"":""%23ThingsThatGetMeUpset"",
            ""name"":""#ThingsThatGetMeUpset"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%23ThingsThatGetMeUpset"",
            ""events"":null
         },
         {
            ""query"":""%23ReasonsWeCantBeTogether"",
            ""name"":""#ReasonsWeCantBeTogether"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%23ReasonsWeCantBeTogether"",
            ""events"":null
         },
         {
            ""query"":""%22Peter%20Dinklage%22"",
            ""name"":""Peter Dinklage"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Peter%20Dinklage%22"",
            ""events"":null
         },
         {
            ""query"":""%22Connie%20Britton%22"",
            ""name"":""Connie Britton"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Connie%20Britton%22"",
            ""events"":null
         },
         {
            ""query"":""%23100BestGogoSongs"",
            ""name"":""#100BestGogoSongs"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%23100BestGogoSongs"",
            ""events"":null
         },
         {
            ""query"":""%22Kyle%20Chandler%22"",
            ""name"":""Kyle Chandler"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Kyle%20Chandler%22"",
            ""events"":null
         },
         {
            ""query"":""%22Tony%20Gonzalez%22"",
            ""name"":""Tony Gonzalez"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Tony%20Gonzalez%22"",
            ""events"":null
         },
         {
            ""query"":""%22Drew%20Barrymore%22"",
            ""name"":""Drew Barrymore"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Drew%20Barrymore%22"",
            ""events"":null
         },
         {
            ""query"":""%22Elisabeth%20Moss%22"",
            ""name"":""Elisabeth Moss"",
            ""promoted_content"":null,
            ""url"":""http:\/\/twitter.com\/search\/%22Elisabeth%20Moss%22"",
            ""events"":null
         }
      ],
      ""created_at"":""2011-09-19T02:00:17Z""
   }
]";

        const string TestAvailableOrClosestQueryResponse = @"[
   {
      ""placeType"":{
         ""name"":""Country"",
         ""code"":12
      },
      ""name"":""Turkey"",
      ""url"":""http:\/\/where.yahooapis.com\/v1\/place\/23424969"",
      ""woeid"":23424969,
      ""countryCode"":""TR"",
      ""parentid"":1,
      ""country"":""Turkey""
   },
   {
      ""name"":""Birmingham"",
      ""url"":""http:\/\/where.yahooapis.com\/v1\/place\/2364559"",
      ""parentid"":23424977,
      ""woeid"":2364559,
      ""countryCode"":""US"",
      ""placeType"":{
         ""name"":""Town"",
         ""code"":7
      },
      ""country"":""United States""
   }
]";

    }
}
