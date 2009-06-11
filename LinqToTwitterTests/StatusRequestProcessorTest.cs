using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System.Linq.Expressions;
using System;

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
                        { "Type", ((int)StatusType.Public).ToString() }
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
                        { "Type", ((int)StatusType.Friends).ToString() }
                    };
            string expected = "http://twitter.com/statuses/friends_timeline.xml";
            string actual;
            actual = statProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildURLMentionsTest()
        {
            var statProc = new StatusRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)StatusType.Mentions).ToString() },
                        { "SinceID", "123" },
                        { "MaxID", "145" },
                        { "Count", "50" },
                        { "Page", "1" }
                    };
            string expected = "http://twitter.com/statuses/mentions.xml?since_id=123&max_id=145&count=50&page=1";
            string actual;
            actual = statProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsMultipleResultsTest()
        {
            var statProc = new StatusRequestProcessor() { BaseUrl = "http://twitter.com/" };
            XElement twitterResponse = XElement.Load(new StringReader(m_testQueryResponse));
            var actual = statProc.ProcessResults(twitterResponse);
            var actualQuery = actual as IList<Status>;
 
            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery.Count(), 2);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsSingleResultTest()
        {
            var statProc = new StatusRequestProcessor() { BaseUrl = "http://twitter.com/" };
            XElement twitterResponse = XElement.Load(new StringReader(m_testQueryResponse));
            var actual = statProc.ProcessResults(twitterResponse.Descendants("status").First());
            var actualQuery = actual as IList<Status>;

            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery.Count(), 1);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void TwypocalypseProcessResultsSingleResultTest()
        {
            var statProc = new StatusRequestProcessor() { BaseUrl = "http://twitter.com/" };
            XElement twitterResponse = XElement.Load(new StringReader(m_testQueryResponse));
            twitterResponse.Element("status").Element("id").Value = ulong.MaxValue.ToString();
            var actual = statProc.ProcessResults(twitterResponse.Descendants("status").First());
            var actualQuery = actual as IList<Status>;

            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery.Count(), 1);
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
                where tweet.Type == StatusType.Public
                select tweet;

            var whereFinder = new FirstWhereClauseFinder();
            var whereExpression = whereFinder.GetFirstWhere(publicQuery.Expression);

            var lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            var queryParams = reqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)StatusType.Public).ToString())));
        }


        /// <summary>
        ///A test for BuildMentionsUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void TwypocalypseStatusIDUrlTest()
        {
            var reqProc = new StatusRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            var twypocalypseID = ulong.MaxValue.ToString();
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "ID", twypocalypseID }
                    };
            string expected = "http://twitter.com/statuses/show/18446744073709551615.xml";
            var actual = reqProc.BuildShowUrl(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildMentionsUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void TwypocalypseSinceIDUrlTest()
        {
            var reqProc = new StatusRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            var twypocalypseID = ulong.MaxValue.ToString();
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)StatusType.User).ToString() },
                        { "ID", "15411837" },
                        { "UserID", "15411837" },
                        { "SinceID", twypocalypseID },
                        { "ScreenName", "JoeMayo" },
                    };
            string expected = "http://twitter.com/statuses/user_timeline/15411837.xml?user_id=15411837&screen_name=JoeMayo&since_id=18446744073709551615";
            var actual = reqProc.BuildUserUrl(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildMentionsUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildMentionsUrlTest()
        {
            var reqProc = new StatusRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Page", "0" },
                        { "SinceID", "934818247" }
                    };
            string expected = "http://twitter.com/statuses/mentions.xml?since_id=934818247&page=0";
            var actual = reqProc.BuildMentionsUrl(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildShowUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildShowUrlTest()
        {
            var reqProc = new StatusRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)StatusType.Show).ToString() },
                        { "ID", "945932078" }
                    };
            string expected = "http://twitter.com/statuses/show/945932078.xml";
            var actual = reqProc.BuildShowUrl(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildUserUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildUserUrlTest()
        {
            var reqProc = new StatusRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)StatusType.User).ToString() },
                        { "ID", "15411837" },
                        { "UserID", "15411837" },
                        { "ScreenName", "JoeMayo" },
                    };
            string expected = "http://twitter.com/statuses/user_timeline/15411837.xml?user_id=15411837&screen_name=JoeMayo";
            var actual = reqProc.BuildUserUrl(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildFriendAndUrlParameters
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildFriendAndUrlParametersTest()
        {
            var reqProc = new StatusRequestProcessor_Accessor();
            var url = "http://twitter.com/statuses/user_timeline/15411837.xml";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Page", "0" },
                        { "Count", "21" },
                        { "SinceID", "934818247" }
                    };
            string expected = "http://twitter.com/statuses/user_timeline/15411837.xml?since_id=934818247&count=21&page=0";
            var actual = reqProc.BuildFriendRepliesAndUrlParameters(parameters, url);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildFriendUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildFriendUrlTest()
        {
            var reqProc = new StatusRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Page", "0" },
                        { "Count", "21" },
                        { "SinceID", "934818247" }
                    };
            string expected = "http://twitter.com/statuses/friends_timeline.xml?since_id=934818247&count=21&page=0";
            var actual = reqProc.BuildFriendUrl(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildPublicUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildPublicUrlTest()
        {
            var reqProc = new StatusRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            string expected = "http://twitter.com/statuses/public_timeline.xml";
            var actual = reqProc.BuildPublicUrl();
            Assert.AreEqual(expected, actual);
        }
    }
}
