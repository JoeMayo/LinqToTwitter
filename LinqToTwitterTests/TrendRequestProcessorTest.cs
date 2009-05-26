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
            TrendRequestProcessor target = new TrendRequestProcessor();
            XElement twitterResponse = XElement.Parse(m_testTrendQueryResponse);
            IList actual = target.ProcessResults(twitterResponse);
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
            TrendRequestProcessor target = new TrendRequestProcessor();
            XElement twitterResponse = XElement.Parse(m_testCurrentTrendQueryResponse);
            IList actual = target.ProcessResults(twitterResponse);
            var trends = actual.Cast<Trend>().ToList();
            Assert.AreEqual(10, trends.Count);
            Assert.AreNotEqual(DateTime.MinValue.Date, trends[0].AsOf.Date);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            var currDT = DateTime.Now;

            TrendRequestProcessor target = new TrendRequestProcessor();
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
            TrendRequestProcessor target = new TrendRequestProcessor() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Trend).ToString() }
                };
            string expected = "http://search.twitter.com/trends.json";
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
            TrendRequestProcessor target = new TrendRequestProcessor() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Weekly).ToString() },
                    { "Date", "2009-01-01" },
                    { "ExcludeHashtags", "true" }
                };
            string expected = "http://search.twitter.com/trends/weekly.json?date=2009-01-01&exclude=hashtags";
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
            TrendRequestProcessor target = new TrendRequestProcessor() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Current).ToString() },
                    { "Date", "2009-01-01" },
                    { "ExcludeHashtags", "true" }
                };
            string expected = "http://search.twitter.com/trends/current.json?date=2009-01-01&exclude=hashtags";
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
            TrendRequestProcessor target = new TrendRequestProcessor() { BaseUrl = "http://search.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)TrendType.Daily).ToString() },
                    { "Date", "2009-01-01" },
                    { "ExcludeHashtags", "true" }
                };
            string expected = "http://search.twitter.com/trends/daily.json?date=2009-01-01&exclude=hashtags";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }
    }
}
