using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for UserRequestProcessorTest and is intended
    ///to contain all UserRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserRequestProcessorTest
    {
        private TestContext testContextInstance;

        private string m_testQueryResponse =
        @"<user>
          <id>15411837</id>
          <name>Joe Mayo</name>
          <screen_name>JoeMayo</screen_name>
          <location>Denver, CO</location>
          <description>Author/entrepreneur, specializing in custom .NET software development</description>
          <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/62569644/JoeTwitter_normal.jpg</profile_image_url>
          <url>http://www.csharp-station.com</url>
          <protected>false</protected>
          <followers_count>25</followers_count>
          <profile_background_color>C6E2EE</profile_background_color>
          <profile_text_color>663B12</profile_text_color>
          <profile_link_color>1F98C7</profile_link_color>
          <profile_sidebar_fill_color>DAECF4</profile_sidebar_fill_color>
          <profile_sidebar_border_color>C6E2EE</profile_sidebar_border_color>
          <friends_count>1</friends_count>
          <created_at>Sun Jul 13 04:35:50 +0000 2008</created_at>
          <favourites_count>0</favourites_count>
          <utc_offset>-25200</utc_offset>
          <time_zone>Mountain Time (US &amp; Canada)</time_zone>
          <profile_background_image_url>http://static.twitter.com/images/themes/theme2/bg.gif</profile_background_image_url>
          <profile_background_tile>false</profile_background_tile>
          <statuses_count>81</statuses_count>
          <status>
            <created_at>Sun Jan 18 21:58:24 +0000 2009</created_at>
            <id>1128977017</id>
            <text>New schedule for #SoCalCodeCamp by @DanielEgan - http://tinyurl.com/9gv5zp</text>
            <source>web</source>
            <truncated>false</truncated>
            <in_reply_to_status_id></in_reply_to_status_id>
            <in_reply_to_user_id></in_reply_to_user_id>
            <favorited>false</favorited>
            <in_reply_to_screen_name></in_reply_to_screen_name>
          </status>
        </user>";

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
        public void BuildURLTest()
        {
            UserRequestProcessor reqProc = new UserRequestProcessor();
            reqProc.BaseUrl = "http://twitter.com/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Friends).ToString() },
                        { "ID", "15411837" }
                    };
            string expected = "http://twitter.com/statuses/friends/15411837.xml";
            var actual = reqProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildShowUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildShowUrlTest()
        {
            UserRequestProcessor_Accessor reqProc = new UserRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Friends).ToString() },
                        { "ID", "15411837" },
                        { "UserID", "15411837" },
                        { "ScreenName", "JoeMayo" }
                    };
            string expected = "http://twitter.com/users/show/15411837.json?user_id=15411837&screen_name=JoeMayo";
            var actual = reqProc.BuildShowUrl(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildShowUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildShowUrlWithNoParametersTest()
        {
            UserRequestProcessor_Accessor reqProc = new UserRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Show).ToString() },
                        { "ID", "15411837" }
                    };
            string expected = "http://twitter.com/users/show/15411837.json";
            var actual = reqProc.BuildShowUrl(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildFriendsUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildFriendsUrlTest()
        {
            UserRequestProcessor_Accessor reqProc = new UserRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Friends).ToString() },
                        { "ID", "15411837" }
                    };
            string expected = "http://twitter.com/statuses/friends/15411837.xml";
            var actual = reqProc.BuildFriendsUrl(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildFriendsAndFollowersUrlParameters
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildFriendsAndFollowersUrlParametersTest()
        {
            UserRequestProcessor_Accessor reqProc = new UserRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Friends).ToString() },
                        { "ID", "15411837" }
                    };
            var url = "http://twitter.com/statuses/friends.xml";
            string expected = "http://twitter.com/statuses/friends/15411837.xml";
            var actual = reqProc.BuildFriendsAndFollowersUrlParameters(parameters, url);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildFollowersUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildFollowersUrlTest()
        {
            UserRequestProcessor_Accessor reqProc = new UserRequestProcessor_Accessor();
            reqProc.BaseUrl = "http://twitter.com/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Followers).ToString() },
                        { "ID", "15411837" },
                        { "UserID", "123" },
                        { "ScreenName", "JoeMayo" },
                        { "Page", "2" }
                    };
            string expected = "http://twitter.com/statuses/followers/15411837.xml?user_id=123&screen_name=JoeMayo&page=2";
            var actual = reqProc.BuildFollowersUrl(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            var reqProc = new UserRequestProcessor();

            var ctx = new TwitterContext();

            var publicQuery =
                from tweet in ctx.User
                where tweet.Type == UserType.Friends
                select tweet;

            var whereFinder = new FirstWhereClauseFinder();
            var whereExpression = whereFinder.GetFirstWhere(publicQuery.Expression);

            var lambdaExpression = (LambdaExpression)((UnaryExpression)(whereExpression.Arguments[1])).Operand;

            lambdaExpression = (LambdaExpression)Evaluator.PartialEval(lambdaExpression);

            var queryParams = reqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)UserType.Friends).ToString())));
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsSingleResultTest()
        {
            var statProc = new UserRequestProcessor() { BaseUrl = "http://twitter.com/" };
            XElement twitterResponse = XElement.Load(new StringReader(m_testQueryResponse));
            var actual = statProc.ProcessResults(twitterResponse);
            var actualQuery = actual as IList<User>;

            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery.Count(), 1);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsMultipleResultsTest()
        {
            var statProc = new UserRequestProcessor() { BaseUrl = "http://twitter.com/" };
            XElement twitterResponse = XElement.Load(new StringReader(m_testQueryResponse));
            var actual = statProc.ProcessResults(twitterResponse);
            var actualQuery = actual as IList<User>;

            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery.Count(), 1);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            UserRequestProcessor target = new UserRequestProcessor() { BaseUrl = "http://twitter.com/" };
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
            UserRequestProcessor target = new UserRequestProcessor() { BaseUrl = "http://twitter.com/" };
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
