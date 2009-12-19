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
    ///This is a test class for SocialGraphRequestProcessorTest and is intended
    ///to contain all SocialGraphRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SocialGraphRequestProcessorTest
    {
        private string m_testQueryResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ids>
<id>123456</id>
<id>987654</id>
</ids>";

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
        public void ProcessResultsTest()
        {
            SocialGraphRequestProcessor target = new SocialGraphRequestProcessor();
            XElement twitterResponse = XElement.Parse(m_testQueryResponse);
            IList actual = target.ProcessResults(twitterResponse);
            var graph = actual.Cast<SocialGraph>().ToList();
            Assert.AreEqual(graph[0].IDs[0], "123456");
            Assert.AreEqual(graph[0].IDs[1], "987654");
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            SocialGraphRequestProcessor target = new SocialGraphRequestProcessor();
            Expression<Func<SocialGraph, bool>> expression =
                graph => 
                    graph.Type == SocialGraphType.Followers && 
                    graph.ID == "123" && 
                    graph.ScreenName == "456" &&
                    graph.Page == 1;
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
                    new KeyValuePair<string, string>("Page", "1")));
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildURLTest()
        {
            SocialGraphRequestProcessor target = new SocialGraphRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string expected = "http://twitter.com/friends/ids.xml";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildSocialGraphFriendsUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildSocialGraphFriendsUrlTest()
        {
            SocialGraphRequestProcessor target = new SocialGraphRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", "0" },
                    { "ID", "JoeMayo" },
                    { "UserID", "123" },
                    { "ScreenName", "456" },
                    { "Page", "1" }
                };
            string expected = "http://twitter.com/friends/ids/JoeMayo.xml?user_id=123&screen_name=456&page=1";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildSocialGraphFollowersUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildSocialGraphFollowersUrlTest()
        {
            SocialGraphRequestProcessor target = new SocialGraphRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", "1" },
                    { "ID", "JoeMayo" },
                    { "UserID", "123" },
                    { "ScreenName", "456" },
                    { "Page", "1" }
                };
            string expected = "http://twitter.com/followers/ids/JoeMayo.xml?user_id=123&screen_name=456&page=1";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            SocialGraphRequestProcessor target = new SocialGraphRequestProcessor() { BaseUrl = "http://twitter.com/" };
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
            SocialGraphRequestProcessor target = new SocialGraphRequestProcessor() { BaseUrl = "http://twitter.com/" };
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
