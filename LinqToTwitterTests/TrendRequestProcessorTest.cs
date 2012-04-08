using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterTests
{
    [TestClass]
    public class TrendRequestProcessorTest
    {
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void ProcessResults_Handles_Trends()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { Type = TrendType.Trend };

            List<Trend> trends = trendReqProc.ProcessResults(TestTrendQueryResponse);

            Assert.AreEqual(10, trends.Count);
            Assert.AreEqual(DateTime.Parse("2011-09-19").Date, trends[0].AsOf.Date);
        }

        [TestMethod]
        public void ProcessResults_Handles_Available()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { Type = TrendType.Available };

            List<Trend> trends = trendReqProc.ProcessResults(TestAvailableQueryResponse);

            Assert.AreEqual(2, trends[0].Locations.Count);
            Location loc = trends[0].Location;
            Assert.AreEqual("Country", loc.PlaceTypeName);
            Assert.AreEqual(12, loc.PlaceTypeNameCode);
            Assert.AreEqual("Turkey", loc.Name);
            Assert.AreEqual("http://where.yahooapis.com/v1/place/23424969", loc.Url);
            Assert.AreEqual("23424969", loc.WoeID);
            Assert.AreEqual("TR", loc.CountryCode);
            Assert.AreEqual("1", loc.ParentID);
            Assert.AreEqual("Turkey", loc.Country);
        }

        [TestMethod]
        public void ProcessResults_Handles_Daily()
        {
            var expectedDateTime = new DateTime(2011, 9, 5, 11, 0, 0);
            var trendReqProc = new TrendRequestProcessor<Trend> { Type = TrendType.Daily };

            List<Trend> trends = trendReqProc.ProcessResults(TestDailyQueryResponse);

            Assert.AreEqual(4, trends.Count);
            Assert.AreEqual(expectedDateTime, trends.First().TrendDate);
        }

        [TestMethod]
        public void ProcessResults_Parses_Location()
        {
            var trendProc = new TrendRequestProcessor<Trend> { Type = TrendType.Location };

            List<Trend> trends = trendProc.ProcessResults(TestTrendQueryResponse);

            Assert.AreEqual(1, trends[0].Locations.Count);

            Location loc = trends[0].Locations.First();
            Assert.AreEqual("Worldwide", loc.Name);
            Assert.AreEqual("1", loc.WoeID);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var trendProc = new TrendRequestProcessor<Trend>();

            var trends = trendProc.ProcessResults(string.Empty);

            Assert.AreEqual(0, trends.Count);
        }

        [TestMethod]
        public void GetParameters_Collects_Parameters()
        {
            const bool shouldExcludeHashtags = true;
            var trendReqProc = new TrendRequestProcessor<Trend>();
            Expression<Func<Trend, bool>> expression =
                trend =>
                    trend.Type == TrendType.Available &&
                    trend.ExcludeHashtags == shouldExcludeHashtags;
            var lambdaExpression = expression as LambdaExpression;

            var queryParams = trendReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture))));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ExcludeHashtags", "True")));
        }

        [TestMethod]
        public void BuildUrl_Constructs_Trends_URL()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Trend).ToString(CultureInfo.InvariantCulture) }
                };
            const string expected = "https://api.twitter.com/1/trends/1.json";

            Request req = trendReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildWeeklyTrendsURLTest()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Weekly).ToString(CultureInfo.InvariantCulture) },
                    { "Date", "2009-01-01" },
                    { "ExcludeHashtags", "true" }
                };
            const string expected = "https://api.twitter.com/1/trends/weekly.json?date=2009-01-01&exclude=hashtags";

            Request req = trendReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Handles_Available_Trends()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture) }
                };
            const string expected = "https://api.twitter.com/1/trends/available.json";

            Request req = trendReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Constructs_Daily_Trends()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Daily).ToString(CultureInfo.InvariantCulture) },
                    { "Date", "2009-01-01" },
                    { "ExcludeHashtags", "true" }
                };
            const string expected = "https://api.twitter.com/1/trends/daily.json?date=2009-01-01&exclude=hashtags";

            Request req = trendReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Constructs_AvailableTrends_URL()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "http://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture) },
                    { "Latitude", "37.78215" },
                    { "Longitude", "-122.40060" }
                };
            const string expected = "http://api.twitter.com/1/trends/available.json?lat=37.78215&long=-122.40060";

            Request req = trendReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildUrl_Throws_On_AvailableTrends_Without_Latitude()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "http://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString(CultureInfo.InvariantCulture) },
                    { "Longitude", "-122.40060" }
                };

            trendReqProc.BuildURL(parameters);
        }

        [TestMethod]
        public void BuildUrl_Constructs_LocationTrends_URL()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "http://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Location).ToString(CultureInfo.InvariantCulture) },
                    { "WeoID", "1" }
                };
            const string expected = "http://api.twitter.com/1/trends/1.json";

            Request req = trendReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildUrl_Throws_On_LocationTrends_WithoutWeoID()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "http://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Location).ToString(CultureInfo.InvariantCulture) },
                };

            trendReqProc.BuildURL(parameters);
        }

        [TestMethod]
        public void BuildUrl_Throws_When_Type_Not_Provided()
        {
            var trendReqProc = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>();

            try
            {
                trendReqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Type", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildUrl_Throws_With_No_Parameters()
        {
            var target = new TrendRequestProcessor<Trend> { BaseUrl = "https://api.twitter.com/1/" };

            try
            {
                target.BuildURL(null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("Type", ae.ParamName);
            }
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

        const string TestAvailableQueryResponse = @"[
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

        const string TestDailyQueryResponse = @"{
           ""trends"":{
              ""2011-09-05 11:00"":[
                 {
                    ""name"":""#Youshouldbeashamedif"",
                    ""query"":""#Youshouldbeashamedif"",
                    ""events"":null,
                    ""promoted_content"":null
                 },
                 {
                    ""name"":""#iReallyHateWhen"",
                    ""query"":""#iReallyHateWhen"",
                    ""events"":null,
                    ""promoted_content"":null
                 }
              ],
              ""2011-09-05 10:00"":[
                 {
                    ""name"":""#iReallyHateWhen"",
                    ""query"":""#iReallyHateWhen"",
                    ""events"":null,
                    ""promoted_content"":null
                 },
                 {
                    ""name"":""#Youshouldbeashamedif"",
                    ""query"":""#Youshouldbeashamedif"",
                    ""events"":null,
                    ""promoted_content"":null
                 }
              ]
           },
           ""as_of"":1315440609
         }";
    }
}
