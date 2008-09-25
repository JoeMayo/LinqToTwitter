using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Linq.Expressions;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for StatusRequestProcessorTest and is intended
    ///to contain all StatusRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StatusRequestProcessorTest
    {
        private TestContext testContextInstance;

        private string m_testQueryResponse = @"
<statuses type=""array"">
  <status>
    <created_at>Mon Sep 22 23:34:05 +0000 2008</created_at>
    <id>123</id>
    <text>Test Text 1</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <user>
      <id>456</id>
      <name>User 1</name>
      <screen_name>User 1</screen_name>
      <location></location>
      <description></description>
      <profile_image_url></profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>15000</followers_count>
    </user>
  </status>
  <status>
    <created_at>Mon Sep 22 00:57:30 +0000 2008</created_at>
    <id>789</id>
    <text>Test Text 2</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id>012</in_reply_to_status_id>
    <in_reply_to_user_id>345</in_reply_to_user_id>
    <favorited>false</favorited>
    <user>
      <id>678</id>
      <name>User 2</name>
      <screen_name>User 2</screen_name>
      <location></location>
      <description></description>
      <profile_image_url></profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>65</followers_count>
    </user>
  </status>
</statuses>";

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
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildURLPublicTest()
        {
            var statProc = new StatusRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters = null;
            string expected = "http://twitter.com/statuses/public_timeline.xml";
            string actual;
            actual = statProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildURLPublicNullTest()
        {
            var statProc = new StatusRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", "Public" }
                    };
            string expected = "http://twitter.com/statuses/public_timeline.xml";
            string actual;
            actual = statProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildURLFriendTest()
        {
            var statProc = new StatusRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", "Friends" }
                    };
            string expected = "http://twitter.com/statuses/friends_timeline.xml";
            string actual;
            actual = statProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsTest()
        {
            var statProc = new StatusRequestProcessor() { BaseUrl = "http://twitter.com/" };
            XElement twitterResponse = XElement.Load(new StringReader(m_testQueryResponse));
            var actual = statProc.ProcessResults(twitterResponse);
            var actualQuery = actual as IQueryable<Status>;
 
            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery.Count(), 2);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            var reqProc = new StatusRequestProcessor();

            var ctx = new TwitterContext();

            var publicQuery =
                from tweet in ctx.Status
                where tweet.Type == "Public"
                select tweet;

            var whereFinder = new InnermostWhereFinder();
            var whereExpression = whereFinder.GetInnermostWhere(publicQuery.Expression);

            var lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            var queryParams = reqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", "Public")));
        }
    }
}
