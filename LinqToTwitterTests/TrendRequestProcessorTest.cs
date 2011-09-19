using System;
using System.Collections;
using System.Collections.Generic;
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
        private TestContext testContextInstance;

        #region Test Data

//        private string m_testTrendQueryResponse = @"<root type=""object"">
//  <trends type=""array"">
//    <item type=""object"">
//      <name type=""string"">Wolverine</name>
//      <url type=""string"">http://search.twitter.com/search?q=Wolverine+OR+%23Wolverine</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">Swine Flu</name>
//      <url type=""string"">http://search.twitter.com/search?q=%22Swine+Flu%22</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">#swineflu</name>
//      <url type=""string"">http://search.twitter.com/search?q=%23swineflu</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">Dollhouse</name>
//      <url type=""string"">http://search.twitter.com/search?q=Dollhouse+OR+%23dollhouse</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">Hamthrax</name>
//      <url type=""string"">http://search.twitter.com/search?q=Hamthrax+OR+%23hamthrax</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">H1N1</name>
//      <url type=""string"">http://search.twitter.com/search?q=H1N1</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">X-Men Origins</name>
//      <url type=""string"">http://search.twitter.com/search?q=%22X-Men+Origins%22</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">#outdoorplay</name>
//      <url type=""string"">http://search.twitter.com/search?q=%23outdoorplay</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">Earthquake</name>
//      <url type=""string"">http://search.twitter.com/search?q=Earthquake+OR+%23earthquake</url>
//    </item>
//    <item type=""object"">
//      <name type=""string"">#jonaslive</name>
//      <url type=""string"">http://search.twitter.com/search?q=%23jonaslive</url>
//    </item>
//  </trends>
//  <as_of type=""string"">Sat, 02 May 2009 02:38:00 +0000</as_of>
//</root>";

//        private string m_testTrendQueryResponse = @"[
//   {
//      ""placeType"":{
//         ""name"":""Country"",
//         ""code"":12
//      },
//      ""name"":""Turkey"",
//      ""url"":""http:\/\/where.yahooapis.com\/v1\/place\/23424969"",
//      ""woeid"":23424969,
//      ""countryCode"":""TR"",
//      ""parentid"":1,
//      ""country"":""Turkey""
//   },
//   {
//      ""name"":""Birmingham"",
//      ""url"":""http:\/\/where.yahooapis.com\/v1\/place\/2364559"",
//      ""parentid"":23424977,
//      ""woeid"":2364559,
//      ""countryCode"":""US"",
//      ""placeType"":{
//         ""name"":""Town"",
//         ""code"":7
//      },
//      ""country"":""United States""
//   }
//]";

        private string m_testTrendQueryResponse = @"[
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

//        string m_testAvailableQueryResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
//<locations type=""array"">
//<location>
//  <woeid>23424969</woeid>
//  <name>Turkey</name>
//  <placeTypeName code=""12"">Country</placeTypeName>
//  <country type=""Country"" code=""TR"">Turkey</country>
//  <url>http://where.yahooapis.com/v1/place/23424969</url>
//  <parentid>1</parentid>
//</location>
//<location>
//  <woeid>2364559</woeid>
//  <name>Birmingham</name>
//  <placeTypeName code=""7"">Town</placeTypeName>
//  <country type=""Country"" code=""US"">United States</country>
//  <url>http://where.yahooapis.com/v1/place/2364559</url>
//  <parentid>23424977</parentid>
//</location>
//<location>
//  <woeid>395269</woeid>
//  <name>Caracas</name>
//  <placeTypeName code=""7"">Town</placeTypeName>
//  <country type=""Country"" code=""VE"">Venezuela</country>
//  <url>http://where.yahooapis.com/v1/place/395269</url>
//  <parentid>23424982</parentid>
//</location>
//</locations>
//";

        string m_testAvailableQueryResponse = @"[
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

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void ProcessResults_Handles_Trends()
        {
            var trendReqProc = new TrendRequestProcessor_Accessor<Trend> { Type = TrendType.Trend };

            IList actual = trendReqProc.ProcessResults(m_testTrendQueryResponse);

            var trends = actual.Cast<Trend>().ToList();
            Assert.AreEqual(10, trends.Count);
            Assert.AreEqual(DateTime.Parse("2011-09-19").Date, trends[0].AsOf.Date);
        }

        [TestMethod]
        public void ProcessResults_Handles_Available()
        {
            var trendReqProc = new TrendRequestProcessor_Accessor<Trend> { Type = TrendType.Available };

            IList actual = trendReqProc.ProcessResults(m_testAvailableQueryResponse);

            var trends = actual.Cast<Trend>().ToList();
            Assert.AreEqual(2, trends[0].Locations.Count);
        }

        [TestMethod]
        public void ProcessResults_Parses_Location()
        {
            var trendProc = new TrendRequestProcessor_Accessor<Trend> { Type = TrendType.Location };

            List<Trend> trends = trendProc.ProcessResults(m_testTrendQueryResponse);

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

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            var currDT = DateTime.Now;

            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>();
            Expression<Func<Trend, bool>> expression =
                trend =>
                    trend.Type == TrendType.Available &&
                    trend.ExcludeHashtags == true;
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)TrendType.Available).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ExcludeHashtags", "True")));
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildTrendsURLTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Trend).ToString() }
                };
            string expected = "https://api.twitter.com/1/trends/1.json";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildWeeklyTrendsURLTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Weekly).ToString() },
                    { "Date", "2009-01-01" },
                    { "ExcludeHashtags", "true" }
                };
            string expected = "https://api.twitter.com/1/trends/weekly.json?date=2009-01-01&exclude=hashtags";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod()]
        public void BuildUrl_Handles_Available_Trends()
        {
            var trendReqProc = new TrendRequestProcessor<Trend>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString() }
                };
            string expected = "https://api.twitter.com/1/trends/available.json";

            Request req = trendReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildDailyTrendsURLTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Daily).ToString() },
                    { "Date", "2009-01-01" },
                    { "ExcludeHashtags", "true" }
                };
            string expected = "https://api.twitter.com/1/trends/daily.json?date=2009-01-01&exclude=hashtags";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildAvailableTrendsURLTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "http://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString() },
                    { "Latitude", "37.78215" },
                    { "Longitude", "-122.40060" }
                };
            string expected = "http://api.twitter.com/1/trends/available.json?lat=37.78215&long=-122.40060";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildAvailableTrendsWithoutLatitudeURLTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "http://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Available).ToString() },
                    { "Longitude", "-122.40060" }
                };

            target.BuildURL(parameters);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildLocationTrendsURLTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "http://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Location).ToString() },
                    { "WeoID", "1" }
                };
            string expected = "http://api.twitter.com/1/trends/1.json";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildLocationTrendsWithoutWeoIDURLTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "http://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Location).ToString() },
                };

            target.BuildURL(parameters);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters = new Dictionary<string, string> { };

            try
            {
                target.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual<string>("Type", ae.ParamName);
            }
        }

        /// <summary>
        ///A test for null parameters
        ///</summary>
        [TestMethod]
        public void NullParametersTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters = null;

            try
            {
                target.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual<string>("Type", ae.ParamName);
            }
        }
    }
}
