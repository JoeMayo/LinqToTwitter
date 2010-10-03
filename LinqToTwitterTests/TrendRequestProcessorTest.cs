using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Collections;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for TrendRequestProcessorTest and is intended
    ///to contain all TrendRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TrendRequestProcessorTest
    {
        private TestContext testContextInstance;

        private string m_testTrendQueryResponse = @"<root type=""object"">
  <trends type=""array"">
    <item type=""object"">
      <name type=""string"">Wolverine</name>
      <url type=""string"">http://search.twitter.com/search?q=Wolverine+OR+%23Wolverine</url>
    </item>
    <item type=""object"">
      <name type=""string"">Swine Flu</name>
      <url type=""string"">http://search.twitter.com/search?q=%22Swine+Flu%22</url>
    </item>
    <item type=""object"">
      <name type=""string"">#swineflu</name>
      <url type=""string"">http://search.twitter.com/search?q=%23swineflu</url>
    </item>
    <item type=""object"">
      <name type=""string"">Dollhouse</name>
      <url type=""string"">http://search.twitter.com/search?q=Dollhouse+OR+%23dollhouse</url>
    </item>
    <item type=""object"">
      <name type=""string"">Hamthrax</name>
      <url type=""string"">http://search.twitter.com/search?q=Hamthrax+OR+%23hamthrax</url>
    </item>
    <item type=""object"">
      <name type=""string"">H1N1</name>
      <url type=""string"">http://search.twitter.com/search?q=H1N1</url>
    </item>
    <item type=""object"">
      <name type=""string"">X-Men Origins</name>
      <url type=""string"">http://search.twitter.com/search?q=%22X-Men+Origins%22</url>
    </item>
    <item type=""object"">
      <name type=""string"">#outdoorplay</name>
      <url type=""string"">http://search.twitter.com/search?q=%23outdoorplay</url>
    </item>
    <item type=""object"">
      <name type=""string"">Earthquake</name>
      <url type=""string"">http://search.twitter.com/search?q=Earthquake+OR+%23earthquake</url>
    </item>
    <item type=""object"">
      <name type=""string"">#jonaslive</name>
      <url type=""string"">http://search.twitter.com/search?q=%23jonaslive</url>
    </item>
  </trends>
  <as_of type=""string"">Sat, 02 May 2009 02:38:00 +0000</as_of>
</root>";

        private string m_testCurrentTrendQueryResponse = @"<root type=""object"">
  <trends type=""object"">
    <a:item xmlns:a=""item"" item=""2009-05-02 03:07:50"" type=""array"">
      <item type=""object"">
        <query type=""string"">Wolverine OR #wolverine</query>
        <name type=""string"">Wolverine</name>
      </item>
      <item type=""object"">
        <query type=""string"">""Swine Flu""</query>
        <name type=""string"">Swine Flu</name>
      </item>
      <item type=""object"">
        <query type=""string"">#SwineFlu</query>
        <name type=""string"">#SwineFlu</name>
      </item>
      <item type=""object"">
        <query type=""string"">H1N1</query>
        <name type=""string"">H1N1</name>
      </item>
      <item type=""object"">
        <query type=""string"">Dollhouse OR #dollhouse</query>
        <name type=""string"">Dollhouse</name>
      </item>
      <item type=""object"">
        <query type=""string"">Hamthrax</query>
        <name type=""string"">Hamthrax</name>
      </item>
      <item type=""object"">
        <query type=""string"">""X-Men Origins""</query>
        <name type=""string"">X-Men Origins</name>
      </item>
      <item type=""object"">
        <query type=""string"">Hawks</query>
        <name type=""string"">Hawks</name>
      </item>
      <item type=""object"">
        <query type=""string"">#jonaslive</query>
        <name type=""string"">#jonaslive</name>
      </item>
      <item type=""object"">
        <query type=""string"">#fitfam</query>
        <name type=""string"">#fitfam</name>
      </item>
    </a:item>
  </trends>
  <as_of type=""number"">1241233670</as_of>
</root>";

        string m_testAvailableQueryResponse = @"<locations type=""array"">
  <location>
    <woeid>23424900</woeid>
    <name>Mexico</name>
    <placeTypeName code=""12"">Country</placeTypeName>
    <country type=""Country"" code=""MX"">Mexico</country>
    <url>http://where.yahooapis.com/v1/place/23424900</url>
  </location>
  <location>
    <woeid>23424975</woeid>
    <name>United Kingdom</name>
    <placeTypeName code=""12"">Country</placeTypeName>
    <country type=""Country"" code=""GB"">United Kingdom</country>
    <url>http://where.yahooapis.com/v1/place/23424975</url>
  </location>
  <location>
    <woeid>23424803</woeid>
    <name>Ireland</name>
    <placeTypeName code=""12"">Country</placeTypeName>
    <country type=""Country"" code=""IE"">Ireland</country>
    <url>http://where.yahooapis.com/v1/place/23424803</url>
  </location>
  <location>
    <woeid>2367105</woeid>
    <name>Boston</name>
    <placeTypeName code=""7"">Town</placeTypeName>
    <country type=""Country"" code=""US"">United States</country>
    <url>http://where.yahooapis.com/v1/place/2367105</url>
  </location>
  <location>
    <woeid>2514815</woeid>
    <name>Washington</name>
    <placeTypeName code=""7"">Town</placeTypeName>
    <country type=""Country"" code=""US"">United States</country>
    <url>http://where.yahooapis.com/v1/place/2514815</url>
  </location>
  <location>
    <woeid>2358820</woeid>
    <name>Baltimore</name>
    <placeTypeName code=""7"">Town</placeTypeName>
    <country type=""Country"" code=""US"">United States</country>
    <url>http://where.yahooapis.com/v1/place/2358820</url>
  </location>
  <location>
    <woeid>455827</woeid>
    <name>Sao Paulo</name>
    <placeTypeName code=""7"">Town</placeTypeName>
    <country type=""Country"" code=""BR"">Brazil</country>
    <url>http://where.yahooapis.com/v1/place/455827</url>
  </location>
  <location>
    <woeid>2459115</woeid>
    <name>New York</name>
    <placeTypeName code=""7"">Town</placeTypeName>
    <country type=""Country"" code=""US"">United States</country>
    <url>http://where.yahooapis.com/v1/place/2459115</url>
  </location>
  <location>
    <woeid>2487796</woeid>
    <name>San Antonio</name>
    <placeTypeName code=""7"">Town</placeTypeName>
    <country type=""Country"" code=""US"">United States</country>
    <url>http://where.yahooapis.com/v1/place/2487796</url>
  </location>
  <location>
    <woeid>23424977</woeid>
    <name>United States</name>
    <placeTypeName code=""12"">Country</placeTypeName>
    <country type=""Country"" code=""US"">United States</country>
    <url>http://where.yahooapis.com/v1/place/23424977</url>
  </location>
</locations>";

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
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
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

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessTrendResultsTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>();

            IList actual = target.ProcessResults(m_testTrendQueryResponse);

            var trends = actual.Cast<Trend>().ToList();
            Assert.AreEqual(10, trends.Count);
            Assert.AreNotEqual(DateTime.MinValue.Date, trends[0].AsOf.Date);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessCurrentTrendResultsTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>();

            IList actual = target.ProcessResults(m_testCurrentTrendQueryResponse);

            var trends = actual.Cast<Trend>().ToList();
            Assert.AreEqual(10, trends.Count);
            Assert.AreNotEqual(DateTime.MinValue.Date, trends[0].AsOf.Date);
        }

         /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessAvailableTrendResultsTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>();

            IList actual = target.ProcessResults(m_testAvailableQueryResponse);

            var trends = actual.Cast<Trend>().ToList();
            Assert.AreEqual(10, trends[0].Locations.Count);
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
                    trend.Type == TrendType.Current &&
                    trend.ExcludeHashtags == true;
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)TrendType.Current).ToString())));
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
            string expected = "https://api.twitter.com/1/trends.json";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildCurrentTrendsURLTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Current).ToString() },
                    { "Date", "2009-01-01" },
                    { "ExcludeHashtags", "true" }
                };
            string expected = "https://api.twitter.com/1/trends/current.json?date=2009-01-01&exclude=hashtags";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string expected = "http://api.twitter.com/1/trends/available.xml?lat=37.78215&long=-122.40060";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string expected = "http://api.twitter.com/1/trends/1.xml";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string actual = target.BuildURL(parameters);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters = new Dictionary<string, string> { };
            string actual;
            try
            {
                actual = target.BuildURL(parameters);
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
        [TestMethod()]
        public void NullParametersTest()
        {
            TrendRequestProcessor<Trend> target = new TrendRequestProcessor<Trend>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters = null;
            string actual;
            try
            {
                actual = target.BuildURL(parameters);
                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual<string>("Type", ae.ParamName);
            }
        }
    }
}
