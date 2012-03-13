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
    
    
    /// <summary>
    ///This is a test class for SavedSearchRequestProcessorTest and is intended
    ///to contain all SavedSearchRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SavedSearchRequestProcessorTest
    {
        private TestContext testContextInstance;

        private string m_testQueryResponse = @"<saved_searches type=""array"">
  <saved_search>
    <id>176136</id>
    <name>#csharp</name>
    <query>#csharp</query>
    <position></position>
    <created_at>Mon May 18 20:27:59 +0000 2009</created_at>
  </saved_search>
  <saved_search>
    <id>210448</id>
    <name>#twitterapi</name>
    <query>#twitterapi</query>
    <position></position>
    <created_at>Sat May 23 01:53:52 +0000 2009</created_at>
  </saved_search>
  <saved_search>
    <id>314612</id>
    <name>dotnet</name>
    <query>dotnet</query>
    <position></position>
    <created_at>Fri Jun 05 01:03:52 +0000 2009</created_at>
  </saved_search>
</saved_searches>";

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

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsTest()
        {
            SavedSearchRequestProcessor<SavedSearch> target = new SavedSearchRequestProcessor<SavedSearch>();

            IList actual = target.ProcessResults(m_testQueryResponse);

            Assert.AreEqual(3, actual.Count);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new SavedSearchRequestProcessor<SavedSearch>();

            var searches = reqProc.ProcessResults(string.Empty);

            Assert.AreEqual(0, searches.Count);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            SavedSearchRequestProcessor<SavedSearch> target = new SavedSearchRequestProcessor<SavedSearch>() { BaseUrl = "https://api.twitter.com/1/" };
            Expression<Func<SavedSearch, bool>> expression =
                search =>
                    search.Type == SavedSearchType.Show &&
                    search.ID == "123";
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)SavedSearchType.Show).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "123")));
        }

        /// <summary>
        ///A test for BuildShowUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildShowNoIDUrlTest()
        {
            SavedSearchRequestProcessor<SavedSearch> target = new SavedSearchRequestProcessor<SavedSearch>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SavedSearchType.Show.ToString() }
                };
            string expected = "https://api.twitter.com/1/saved_searches/show/123.xml";
            Request req = target.BuildURL(parameters);
            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildShowUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildShowUrlTest()
        {
            SavedSearchRequestProcessor<SavedSearch> target = new SavedSearchRequestProcessor<SavedSearch>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SavedSearchType.Show.ToString() },
                    { "ID", "123" }
                };
            string expected = "https://api.twitter.com/1/saved_searches/show/123.xml";
            Request req = target.BuildURL(parameters);
            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildSearchesUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildSearchesUrlTest()
        {
            SavedSearchRequestProcessor<SavedSearch> target = new SavedSearchRequestProcessor<SavedSearch>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SavedSearchType.Searches.ToString() }
                };
            string expected = "https://api.twitter.com/1/saved_searches.xml";
            Request req = target.BuildURL(parameters);
            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            SavedSearchRequestProcessor<SavedSearch> target = new SavedSearchRequestProcessor<SavedSearch>() { BaseUrl = "https://api.twitter.com/1/" };
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
        [TestMethod()]
        public void NullParametersTest()
        {
            SavedSearchRequestProcessor<SavedSearch> target = new SavedSearchRequestProcessor<SavedSearch>() { BaseUrl = "https://api.twitter.com/1/" };
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
