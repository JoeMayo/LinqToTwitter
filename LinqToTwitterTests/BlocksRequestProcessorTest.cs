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
    ///This is a test class for BlocksRequestProcessorTest and is intended
    ///to contain all BlocksRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BlocksRequestProcessorTest
    {
        private TestContext testContextInstance;

        private string m_blockedUsersXml = @"<users type=""array"">
  <user>
    <id>35142832</id>
    <name>salon equipment</name>
    <screen_name>salon_equipment</screen_name>
    <location></location>
    <description></description>
    <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/182354012/110378188596_0_normal.jpg</profile_image_url>
    <url>http://salon-equipment.trackdeal.net/</url>
    <protected>false</protected>
    <followers_count>81</followers_count>
    <profile_background_color>0099B9</profile_background_color>
    <profile_text_color>3C3940</profile_text_color>
    <profile_link_color>0099B9</profile_link_color>
    <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
    <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
    <friends_count>344</friends_count>
    <created_at>Sat Apr 25 04:14:37 +0000 2009</created_at>
    <favourites_count>0</favourites_count>
    <utc_offset>-21600</utc_offset>
    <time_zone>Central Time (US &amp; Canada)</time_zone>
    <profile_background_image_url>http://static.twitter.com/images/themes/theme4/bg.gif</profile_background_image_url>
    <profile_background_tile>false</profile_background_tile>
    <statuses_count>3</statuses_count>
    <notifications>false</notifications>
    <following>false</following>
    <status>
      <created_at>Sun Apr 26 00:12:37 +0000 2009</created_at>
      <id>1616799882</id>
      <text>Amazing deals on hair cutting shears http://tinyurl.com/hair-shears</text>
      <source>web</source>
      <truncated>false</truncated>
      <in_reply_to_status_id></in_reply_to_status_id>
      <in_reply_to_user_id></in_reply_to_user_id>
      <favorited>false</favorited>
      <in_reply_to_screen_name></in_reply_to_screen_name>
    </status>
  </user>
  <user>
    <id>40182496</id>
    <name>Elisa</name>
    <screen_name>Elisaluv873</screen_name>
    <location>USA</location>
    <description>Im 21. I just graduated from college. I love clubbing, partying, shopping, hanging out with my girls :D I work hard for $$$</description>
    <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/212745340/5_normal.jpg</profile_image_url>
    <url>http://N/A</url>
    <protected>false</protected>
    <followers_count>66</followers_count>
    <profile_background_color>9ae4e8</profile_background_color>
    <profile_text_color>000000</profile_text_color>
    <profile_link_color>0000ff</profile_link_color>
    <profile_sidebar_fill_color>e0ff92</profile_sidebar_fill_color>
    <profile_sidebar_border_color>87bc44</profile_sidebar_border_color>
    <friends_count>738</friends_count>
    <created_at>Fri May 15 05:19:44 +0000 2009</created_at>
    <favourites_count>0</favourites_count>
    <utc_offset></utc_offset>
    <time_zone></time_zone>
    <profile_background_image_url>http://s3.amazonaws.com/twitter_production/profile_background_images/12961771/bg.jpg</profile_background_image_url>
    <profile_background_tile>false</profile_background_tile>
    <statuses_count>7</statuses_count>
    <notifications>false</notifications>
    <following>false</following>
  </user>
</users>";

        private string m_blockedIDsXml = @"<ids>
  <id>37654276</id>
  <id>37644797</id>
  <id>36910814</id>
  <id>35142832</id>
  <id>38161481</id>
  <id>40182496</id>
  <id>16761255</id>
</ids>";

        private string m_blockExistsXml = @"<user>
    <id>35142832</id>
    <name>salon equipment</name>
    <screen_name>salon_equipment</screen_name>
    <location></location>
    <description></description>
    <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/182354012/110378188596_0_normal.jpg</profile_image_url>
    <url>http://salon-equipment.trackdeal.net/</url>
    <protected>false</protected>
    <followers_count>81</followers_count>
    <profile_background_color>0099B9</profile_background_color>
    <profile_text_color>3C3940</profile_text_color>
    <profile_link_color>0099B9</profile_link_color>
    <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
    <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
    <friends_count>344</friends_count>
    <created_at>Sat Apr 25 04:14:37 +0000 2009</created_at>
    <favourites_count>0</favourites_count>
    <utc_offset>-21600</utc_offset>
    <time_zone>Central Time (US &amp; Canada)</time_zone>
    <profile_background_image_url>http://static.twitter.com/images/themes/theme4/bg.gif</profile_background_image_url>
    <profile_background_tile>false</profile_background_tile>
    <statuses_count>3</statuses_count>
    <notifications>false</notifications>
    <following>false</following>
    <status>
      <created_at>Sun Apr 26 00:12:37 +0000 2009</created_at>
      <id>1616799882</id>
      <text>Amazing deals on hair cutting shears http://tinyurl.com/hair-shears</text>
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
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessExistsResultsTest()
        {
            BlocksRequestProcessor<Blocks> target = new BlocksRequestProcessor<Blocks>();

            IList actual = target.ProcessResults(m_blockExistsXml);

            var actualQuery = actual as IList<Blocks>;
            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery.Count(), 1);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessIDsResultsTest()
        {
            BlocksRequestProcessor<Blocks> target = new BlocksRequestProcessor<Blocks>();

            IList actual = target.ProcessResults(m_blockedIDsXml);
            var actualQuery = actual as IList<Blocks>;

            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery[0].IDs.Count, 7);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessBlockingResultsTest()
        {
            BlocksRequestProcessor<Blocks> target = new BlocksRequestProcessor<Blocks>();

            IList actual = target.ProcessResults(m_blockedUsersXml);

            var actualQuery = actual as IList<Blocks>;
            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery[0].Users.Count, 2);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessIdsResultsTest()
        {
            BlocksRequestProcessor<Blocks> target = new BlocksRequestProcessor<Blocks>();

            IList actual = target.ProcessResults(m_blockedUsersXml);

            var actualQuery = actual as IList<Blocks>;
            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery[0].Users.Count, 2);
        }


        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            BlocksRequestProcessor<Blocks> target = new BlocksRequestProcessor<Blocks>();
            Expression<Func<Blocks, bool>> expression =
                graph =>
                    graph.Type == BlockingType.Blocking &&
                    graph.ID == "123" &&
                    graph.ScreenName == "456" &&
                    graph.Page == 1;
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)BlockingType.Blocking).ToString())));
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
        public void BuildExistsURLTest()
        {
            BlocksRequestProcessor<Blocks> target = new BlocksRequestProcessor<Blocks>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)BlockingType.Exists).ToString() },
                    { "ID", "123" },
                    { "UserID", "456" },
                    { "ScreenName", "789" }
                };
            string expected = "http://twitter.com/blocks/exists/123.xml?user_id=456&screen_name=789";
            string actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildBlockingURLTest()
        {
            BlocksRequestProcessor<Blocks> target = new BlocksRequestProcessor<Blocks>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)BlockingType.Blocking).ToString() },
                    { "Page", "2" }
                };
            string expected = "http://twitter.com/blocks/blocking.xml?page=2";
            string actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildIDsURLTest()
        {
            BlocksRequestProcessor<Blocks> target = new BlocksRequestProcessor<Blocks>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)BlockingType.IDS).ToString() }
                };
            string expected = "http://twitter.com/blocks/blocking/ids.xml";
            string actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            BlocksRequestProcessor<Blocks> target = new BlocksRequestProcessor<Blocks>() { BaseUrl = "http://twitter.com/" };
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
            BlocksRequestProcessor<Blocks> target = new BlocksRequestProcessor<Blocks>() { BaseUrl = "http://twitter.com/" };
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
