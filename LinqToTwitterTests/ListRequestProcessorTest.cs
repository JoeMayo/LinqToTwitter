using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for ListRequestProcessorTest and is intended
    ///to contain all ListRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ListRequestProcessorTest
    {
        private TestContext testContextInstance;

        #region Test Data

        private string m_testQueryResponse = @"<lists_list>
  <lists type=""array"">
    <list>
      <id>3897042</id>
      <name>Privatelist</name>
      <full_name>@LinqToTweeter/privatelist</full_name>
      <slug>privatelist</slug>
      <description>This is a private list for testing</description>
      <subscriber_count>0</subscriber_count>
      <member_count>1</member_count>
      <uri>/LinqToTweeter/privatelist</uri>
      <mode>private</mode>
      <user>
        <id>16761255</id>
        <name>LtoT Test</name>
        <screen_name>LinqToTweeter</screen_name>
        <location>Anywhere In The World</location>
        <description>Testing the LINQ to Twitter Account Profile Update.</description>
        <profile_image_url>http://a3.twimg.com/profile_images/197870807/JoeTwitterBW_normal.jpg</profile_image_url>
        <url>http://linqtotwitter.codeplex.com</url>
        <protected>false</protected>
        <followers_count>26</followers_count>
        <profile_background_color>0099B9</profile_background_color>
        <profile_text_color>3C3940</profile_text_color>
        <profile_link_color>0099B9</profile_link_color>
        <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
        <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
        <friends_count>1</friends_count>
        <created_at>Wed Oct 15 05:15:40 +0000 2008</created_at>
        <favourites_count>0</favourites_count>
        <utc_offset>-25200</utc_offset>
        <time_zone>Mountain Time (US &amp; Canada)</time_zone>
        <profile_background_image_url>http://a3.twimg.com/profile_background_images/56043243/200xColor_2.png</profile_background_image_url>
        <profile_background_tile>false</profile_background_tile>
        <statuses_count>77</statuses_count>
        <notifications>false</notifications>
        <geo_enabled>false</geo_enabled>
        <verified>false</verified>
        <following>false</following>
      </user>
    </list>
    <list>
      <id>3897016</id>
      <name>MVC</name>
      <full_name>@LinqToTweeter/mvc</full_name>
      <slug>mvc</slug>
      <description>Developers Interested in MVC</description>
      <subscriber_count>0</subscriber_count>
      <member_count>1</member_count>
      <uri>/LinqToTweeter/mvc</uri>
      <mode>public</mode>
      <user>
        <id>16761255</id>
        <name>LtoT Test</name>
        <screen_name>LinqToTweeter</screen_name>
        <location>Anywhere In The World</location>
        <description>Testing the LINQ to Twitter Account Profile Update.</description>
        <profile_image_url>http://a3.twimg.com/profile_images/197870807/JoeTwitterBW_normal.jpg</profile_image_url>
        <url>http://linqtotwitter.codeplex.com</url>
        <protected>false</protected>
        <followers_count>26</followers_count>
        <profile_background_color>0099B9</profile_background_color>
        <profile_text_color>3C3940</profile_text_color>
        <profile_link_color>0099B9</profile_link_color>
        <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
        <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
        <friends_count>1</friends_count>
        <created_at>Wed Oct 15 05:15:40 +0000 2008</created_at>
        <favourites_count>0</favourites_count>
        <utc_offset>-25200</utc_offset>
        <time_zone>Mountain Time (US &amp; Canada)</time_zone>
        <profile_background_image_url>http://a3.twimg.com/profile_background_images/56043243/200xColor_2.png</profile_background_image_url>
        <profile_background_tile>false</profile_background_tile>
        <statuses_count>77</statuses_count>
        <notifications>false</notifications>
        <geo_enabled>false</geo_enabled>
        <verified>false</verified>
        <following>false</following>
      </user>
    </list>
    <list>
      <id>3897006</id>
      <name>LINQ</name>
      <full_name>@LinqToTweeter/linq</full_name>
      <slug>linq</slug>
      <description>People who specialize in LINQ</description>
      <subscriber_count>0</subscriber_count>
      <member_count>1</member_count>
      <uri>/LinqToTweeter/linq</uri>
      <mode>public</mode>
      <user>
        <id>16761255</id>
        <name>LtoT Test</name>
        <screen_name>LinqToTweeter</screen_name>
        <location>Anywhere In The World</location>
        <description>Testing the LINQ to Twitter Account Profile Update.</description>
        <profile_image_url>http://a3.twimg.com/profile_images/197870807/JoeTwitterBW_normal.jpg</profile_image_url>
        <url>http://linqtotwitter.codeplex.com</url>
        <protected>false</protected>
        <followers_count>26</followers_count>
        <profile_background_color>0099B9</profile_background_color>
        <profile_text_color>3C3940</profile_text_color>
        <profile_link_color>0099B9</profile_link_color>
        <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
        <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
        <friends_count>1</friends_count>
        <created_at>Wed Oct 15 05:15:40 +0000 2008</created_at>
        <favourites_count>0</favourites_count>
        <utc_offset>-25200</utc_offset>
        <time_zone>Mountain Time (US &amp; Canada)</time_zone>
        <profile_background_image_url>http://a3.twimg.com/profile_background_images/56043243/200xColor_2.png</profile_background_image_url>
        <profile_background_tile>false</profile_background_tile>
        <statuses_count>77</statuses_count>
        <notifications>false</notifications>
        <geo_enabled>false</geo_enabled>
        <verified>false</verified>
        <following>false</following>
      </user>
    </list>
  </lists>
  <next_cursor>0</next_cursor>
  <previous_cursor>0</previous_cursor>
</lists_list>";

        private string m_listAllResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<lists type=""array"">
<list>
  <id>4557337</id>
  <name>DotNetTwittterDevs</name>
  <full_name>@JoeMayo/dotnettwittterdevs</full_name>
  <slug>dotnettwittterdevs</slug>
  <description>.NET Developers who use the Twitter API</description>
  <subscriber_count>14</subscriber_count>
  <member_count>182</member_count>
  <uri>/JoeMayo/dotnettwittterdevs</uri>
  <following>true</following>
  <mode>public</mode>
  <user>
    <id>15411837</id>
    <name>Joe Mayo</name>
    <screen_name>JoeMayo</screen_name>
    <location>Denver, CO</location>
    <description>Created LINQ to Twitter, author of 6 .NET books, .NET Consultant, and C# MVP</description>
    <profile_image_url>http://a3.twimg.com/profile_images/520626655/JoeTwitterBW_-_150_x_150_normal.jpg</profile_image_url>
    <url>http://linqtotwitter.codeplex.com/</url>
    <protected>false</protected>
    <followers_count>569</followers_count>
    <profile_background_color>0099B9</profile_background_color>
    <profile_text_color>3C3940</profile_text_color>
    <profile_link_color>0099B9</profile_link_color>
    <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
    <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
    <friends_count>44</friends_count>
    <created_at>Sun Jul 13 04:35:50 +0000 2008</created_at>
    <favourites_count>92</favourites_count>
    <utc_offset>-25200</utc_offset>
    <time_zone>Mountain Time (US &amp; Canada)</time_zone>
    <profile_background_image_url>http://a3.twimg.com/profile_background_images/13330711/200xColor_2.png</profile_background_image_url>
    <profile_background_tile>false</profile_background_tile>
    <profile_use_background_image>true</profile_use_background_image>
    <notifications>true</notifications>
    <geo_enabled>true</geo_enabled>
    <verified>false</verified>
    <following>true</following>
    <statuses_count>1202</statuses_count>
    <lang>en</lang>
    <contributors_enabled>false</contributors_enabled>
    <follow_request_sent>false</follow_request_sent>
    <listed_count>81</listed_count>
    <show_all_inline_media>false</show_all_inline_media>
  </user>
</list>
<list>
  <id>3897042</id>
  <name>Privatelist</name>
  <full_name>@LinqToTweeter/privatelist</full_name>
  <slug>privatelist</slug>
  <description>This is a private list for testing</description>
  <subscriber_count>0</subscriber_count>
  <member_count>1</member_count>
  <uri>/LinqToTweeter/privatelist</uri>
  <following>false</following>
  <mode>private</mode>
  <user>
    <id>16761255</id>
    <name>LINQ to Tweeter Test</name>
    <screen_name>LinqToTweeter</screen_name>
    <location>Anywhere In The World</location>
    <description>Testing the LINQ to Twitter Account Profile Update.</description>
    <profile_image_url>http://a3.twimg.com/profile_images/197870807/JoeTwitterBW_normal.jpg</profile_image_url>
    <url>http://linqtotwitter.codeplex.com</url>
    <protected>false</protected>
    <followers_count>26</followers_count>
    <profile_background_color>0099B9</profile_background_color>
    <profile_text_color>3C3940</profile_text_color>
    <profile_link_color>0099B9</profile_link_color>
    <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
    <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
    <friends_count>1</friends_count>
    <created_at>Wed Oct 15 05:15:40 +0000 2008</created_at>
    <favourites_count>2</favourites_count>
    <utc_offset>-25200</utc_offset>
    <time_zone>Mountain Time (US &amp; Canada)</time_zone>
    <profile_background_image_url>http://a3.twimg.com/profile_background_images/100000765/linq2twitter_v3_300x90.png</profile_background_image_url>
    <profile_background_tile>true</profile_background_tile>
    <profile_use_background_image>true</profile_use_background_image>
    <notifications>false</notifications>
    <geo_enabled>true</geo_enabled>
    <verified>false</verified>
    <following>false</following>
    <statuses_count>77</statuses_count>
    <lang>en</lang>
    <contributors_enabled>false</contributors_enabled>
    <follow_request_sent>false</follow_request_sent>
    <listed_count>0</listed_count>
    <show_all_inline_media>false</show_all_inline_media>
  </user>
</list>
<list>
  <id>3897016</id>
  <name>MVC</name>
  <full_name>@LinqToTweeter/mvc</full_name>
  <slug>mvc</slug>
  <description>Developers Interested in MVC</description>
  <subscriber_count>0</subscriber_count>
  <member_count>1</member_count>
  <uri>/LinqToTweeter/mvc</uri>
  <following>false</following>
  <mode>public</mode>
  <user>
    <id>16761255</id>
    <name>LINQ to Tweeter Test</name>
    <screen_name>LinqToTweeter</screen_name>
    <location>Anywhere In The World</location>
    <description>Testing the LINQ to Twitter Account Profile Update.</description>
    <profile_image_url>http://a3.twimg.com/profile_images/197870807/JoeTwitterBW_normal.jpg</profile_image_url>
    <url>http://linqtotwitter.codeplex.com</url>
    <protected>false</protected>
    <followers_count>26</followers_count>
    <profile_background_color>0099B9</profile_background_color>
    <profile_text_color>3C3940</profile_text_color>
    <profile_link_color>0099B9</profile_link_color>
    <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
    <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
    <friends_count>1</friends_count>
    <created_at>Wed Oct 15 05:15:40 +0000 2008</created_at>
    <favourites_count>2</favourites_count>
    <utc_offset>-25200</utc_offset>
    <time_zone>Mountain Time (US &amp; Canada)</time_zone>
    <profile_background_image_url>http://a3.twimg.com/profile_background_images/100000765/linq2twitter_v3_300x90.png</profile_background_image_url>
    <profile_background_tile>true</profile_background_tile>
    <profile_use_background_image>true</profile_use_background_image>
    <notifications>false</notifications>
    <geo_enabled>true</geo_enabled>
    <verified>false</verified>
    <following>false</following>
    <statuses_count>77</statuses_count>
    <lang>en</lang>
    <contributors_enabled>false</contributors_enabled>
    <follow_request_sent>false</follow_request_sent>
    <listed_count>0</listed_count>
    <show_all_inline_media>false</show_all_inline_media>
  </user>
</list>
<list>
  <id>3897006</id>
  <name>LINQ</name>
  <full_name>@LinqToTweeter/linq</full_name>
  <slug>linq</slug>
  <description>People who specialize in LINQ</description>
  <subscriber_count>0</subscriber_count>
  <member_count>2</member_count>
  <uri>/LinqToTweeter/linq</uri>
  <following>false</following>
  <mode>public</mode>
  <user>
    <id>16761255</id>
    <name>LINQ to Tweeter Test</name>
    <screen_name>LinqToTweeter</screen_name>
    <location>Anywhere In The World</location>
    <description>Testing the LINQ to Twitter Account Profile Update.</description>
    <profile_image_url>http://a3.twimg.com/profile_images/197870807/JoeTwitterBW_normal.jpg</profile_image_url>
    <url>http://linqtotwitter.codeplex.com</url>
    <protected>false</protected>
    <followers_count>26</followers_count>
    <profile_background_color>0099B9</profile_background_color>
    <profile_text_color>3C3940</profile_text_color>
    <profile_link_color>0099B9</profile_link_color>
    <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
    <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
    <friends_count>1</friends_count>
    <created_at>Wed Oct 15 05:15:40 +0000 2008</created_at>
    <favourites_count>2</favourites_count>
    <utc_offset>-25200</utc_offset>
    <time_zone>Mountain Time (US &amp; Canada)</time_zone>
    <profile_background_image_url>http://a3.twimg.com/profile_background_images/100000765/linq2twitter_v3_300x90.png</profile_background_image_url>
    <profile_background_tile>true</profile_background_tile>
    <profile_use_background_image>true</profile_use_background_image>
    <notifications>false</notifications>
    <geo_enabled>true</geo_enabled>
    <verified>false</verified>
    <following>false</following>
    <statuses_count>77</statuses_count>
    <lang>en</lang>
    <contributors_enabled>false</contributors_enabled>
    <follow_request_sent>false</follow_request_sent>
    <listed_count>0</listed_count>
    <show_all_inline_media>false</show_all_inline_media>
  </user>
</list>
</lists>";

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

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsTest()
        {
            ListRequestProcessor<List> listProc = new ListRequestProcessor<List>();
            XElement twitterResponse = XElement.Load(new StringReader(m_testQueryResponse));

            var actual = listProc.ProcessResults(twitterResponse.Descendants("list").First().ToString());

            var actualQuery = actual as IList<List>;
            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(actualQuery.Count(), 1);
        }

        [TestMethod]
        public void ProcessResults_Transforms_All_Query_Results()
        {
            var listProc = new ListRequestProcessor<List>();
            XElement twitterResponse = XElement.Load(new StringReader(m_listAllResponse));

            var actual = listProc.ProcessResults(twitterResponse.ToString());

            Assert.IsNotNull(actual);
            Assert.AreEqual(4, actual.Count);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            ListRequestProcessor<List> target = new ListRequestProcessor<List>();
            Expression<Func<List, bool>> expression =
                list =>
                    list.Type == ListType.Members &&
                    list.ScreenName == "JoeMayo" &&
                    list.Cursor == "123" &&
                    list.ListID == "456" &&
                    list.MaxID == 789 &&
                    list.Page == 1 &&
                    list.PerPage == 10 &&
                    list.SinceID == 123 &&
                    list.ID == "456";
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)ListType.Members).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "JoeMayo")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ListID", "456")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("MaxID", "789")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Page", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("PerPage", "10")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SinceID", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "456")));
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildURLTest()
        {
            ListRequestProcessor<List> target = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ListType.Lists).ToString() },
                    { "ScreenName", "JoeMayo" }
                };
            string expected = "https://api.twitter.com/1/JoeMayo/lists.xml";
            string actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        ///<summary>
        /// A test for buildUrl using parameters
        /// </summary>
        [TestMethod]
        public void BuildStatusUrlTestWithParameters()
        {
            ListRequestProcessor<List> target = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.Statuses).ToString()},
                {"ScreenName", "JoeMayo"},
                {"ListID", "1234567"},
                {"MaxID", "9876543"},
                {"SinceID", "2345678"},
                {"PerPage", "100"},
                {"Page", "2"}
            };
            string expected =
                "https://api.twitter.com/1/JoeMayo/lists/1234567/statuses.xml?max_id=9876543&since_id=2345678&per_page=100&page=2";
            
            string actual = target.BuildURL(parameters);
            
            Assert.AreEqual(expected, actual);                       
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildURLTestMissingType()
        {
            ListRequestProcessor<List> target = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "ScreenName", "JoeMayo" }
                };
            string expected = "https://api.twitter.com/1/JoeMayo/lists.xml";
            string actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildURLTestMissingScreenName()
        {
            ListRequestProcessor<List> target = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", "0" },
                };
            string expected = "https://api.twitter.com/1/JoeMayo/lists.xml";
            string actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            ListRequestProcessor<List> target = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
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
            ListRequestProcessor<List> target = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
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

        [TestMethod]
        public void BuildAllListsUrl_Returns_Url_With_ScreenName()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.All).ToString()},
                {"ScreenName", "JoeMayo"},
            };
            string expected =
                "https://api.twitter.com/1/lists/all.xml?screen_name=JoeMayo";

            string actual = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, actual);
        }

        [Ignore]
        [TestMethod]
        public void BuildAllListsUrl_Returns_Url_With_UserID()
        {
            // TODO: ScreenName check in BuilUrl breaks this test. All other list APIs requires ScreenName, except for this one
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.All).ToString()},
                {"ID", "123"},
            };
            string expected =
                "https://api.twitter.com/1/JoeMayo/lists/all.xml?user_id=123";

            string actual = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, actual);
        }
    }
}
