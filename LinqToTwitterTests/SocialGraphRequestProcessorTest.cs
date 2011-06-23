using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for SocialGraphRequestProcessorTest and is intended
    ///to contain all SocialGraphRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SocialGraphRequestProcessorTest
    {
        #region Test Data

        private string m_testQueryResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ids>
<id>123456</id>
<id>987654</id>
</ids>";

        private string m_testQueryResponseWithIDList = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<id_list>
<ids>
<id>271935353</id>
<id>39340123</id>
</ids>
<next_cursor>0</next_cursor><previous_cursor>0</previous_cursor>
</id_list>";

        #endregion

        private TestContext testContextInstance;

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
        public void ProcessResults_Gets_Results_With_Only_IDs()
        {
            SocialGraphRequestProcessor<SocialGraph> target = new SocialGraphRequestProcessor<SocialGraph>();

            IList actual = target.ProcessResults(m_testQueryResponse);
            
            var graph = actual.Cast<SocialGraph>().ToList();
            Assert.AreEqual(graph[0].IDs[0], "123456");
            Assert.AreEqual(graph[0].IDs[1], "987654");
        }

        [TestMethod]
        public void ProcessResults_Gets_Results_With_IDList()
        {
            var socialGraph = new SocialGraphRequestProcessor<SocialGraph>();

            IList actual = socialGraph.ProcessResults(m_testQueryResponseWithIDList);

            var graph = actual.Cast<SocialGraph>().ToList();
            Assert.AreEqual(graph[0].IDs[0], "271935353");
            Assert.AreEqual(graph[0].IDs[1], "39340123");
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            SocialGraphRequestProcessor<SocialGraph> target = new SocialGraphRequestProcessor<SocialGraph>();
            Expression<Func<SocialGraph, bool>> expression =
                graph =>
                    graph.Type == SocialGraphType.Followers &&
                    graph.ID == "123" &&
                    graph.ScreenName == "456" &&
                    graph.Cursor == "-1";
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)SocialGraphType.Followers).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "456")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "-1")));
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildURLTest()
        {
            SocialGraphRequestProcessor<SocialGraph> target = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string expected = "http://twitter.com/friends/ids.xml";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildSocialGraphFriendsUrl
        ///</summary>
        [TestMethod]
        public void BuildSocialGraphFriendsUrlTest()
        {
            SocialGraphRequestProcessor<SocialGraph> target = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", "0" },
                    { "ID", "JoeMayo" },
                    { "UserID", "123" },
                    { "ScreenName", "456" },
                    { "Cursor", "1" }
                };
            string expected = "http://twitter.com/friends/ids/JoeMayo.xml?user_id=123&screen_name=456&cursor=1";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildUrl_Defaults_Cursor_When_Not_Specified()
        {
            var socialGraph = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", SocialGraphType.Friends.ToString() },
                    { "ID", "JoeMayo" },
                };
            string expected = "http://twitter.com/friends/ids/JoeMayo.xml?cursor=-1";

            Request req = socialGraph.BuildURL(parameters);
            
            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildURL_Creates_URL_For_Followers()
        {
            var reqProc = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)SocialGraphType.Followers).ToString() },
                    { "ID", "JoeMayo" },
                    { "UserID", "123" },
                    { "ScreenName", "456" },
                    { "Cursor", "1" }
                };
            string expected = "http://twitter.com/followers/ids/JoeMayo.xml?user_id=123&screen_name=456&cursor=1";

            Request req = reqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            SocialGraphRequestProcessor<SocialGraph> target = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "http://twitter.com/" };
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
            SocialGraphRequestProcessor<SocialGraph> target = new SocialGraphRequestProcessor<SocialGraph>() { BaseUrl = "http://twitter.com/" };
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
