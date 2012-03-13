using System.Globalization;

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

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new ListRequestProcessor<List>();

            var results = reqProc.ProcessResults(string.Empty);

            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void GetParameters_Parses_All_Available_Parameters()
        {
            ListRequestProcessor<List> target = new ListRequestProcessor<List>();
            Expression<Func<List, bool>> expression =
                list =>
                    list.Type == ListType.Members &&
                    list.UserID == "456" &&
                    list.ScreenName == "JoeMayo" &&
                    list.ListID == "456" &&
                    list.Slug == "test" &&
                    list.OwnerID == "789" &&
                    list.OwnerScreenName == "JoeMayo" &&
                    list.Cursor == "123" &&
                    list.MaxID == 789 &&
                    list.Page == 1 &&
                    list.Count == 10 &&
                    list.SinceID == 123 &&
                    list.FilterToOwnedLists == true &&
                    list.TrimUser == true &&
                    list.IncludeEntities == true &&
                    list.IncludeRetweets == true;

            var queryParams = target.GetParameters(expression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)ListType.Members).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("UserID", "456")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "JoeMayo")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ListID", "456")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Slug", "test")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("OwnerID", "789")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("OwnerScreenName", "JoeMayo")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("MaxID", "789")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Page", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Count", "10")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SinceID", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("FilterToOwnedLists", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("TrimUser", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeEntities", "True")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IncludeRetweets", "True")));
        }

        [TestMethod]
        public void BuildURL_Creates_URL()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters =
                new Dictionary<string, string>
                {
                    { "Type", ((int)ListType.Lists).ToString() },
                    { "ScreenName", "JoeMayo" }
                };
            string expected = "https://api.twitter.com/1/lists.xml?screen_name=JoeMayo";

            Request req = reqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
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

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
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

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            ListRequestProcessor<List> target = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
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
            ListRequestProcessor<List> target = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
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

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildAllListsUrl_Returns_Url_With_UserID()
        {
            var listReqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.All).ToString(CultureInfo.InvariantCulture)},
                {"UserID", "123"},
            };
            const string expected = "https://api.twitter.com/1/lists/all.xml?user_id=123";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildListUrl_Requires_UserID_Or_ScreenName()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.Lists).ToString()},
                //{"UserID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("UserIdOrScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildListsUrl_Returns_Url()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Lists).ToString() },
                { "UserID", "123" },
                { "ScreenName", "JoeMayo" },
                { "Cursor", "456" }
            };
            string expected = "https://api.twitter.com/1/lists.xml?user_id=123&screen_name=JoeMayo&cursor=456";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildMembershipsUrl_Requires_UserID_Or_ScreenName()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.Memberships).ToString()},
                //{"UserID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("UserIdOrScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildMembershipsUrl_Returns_Url()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Memberships).ToString() },
                { "UserID", "123" },
                { "ScreenName", "JoeMayo" },
                { "Cursor", "456" },
                { "FilterToOwnedLists", "true" }
            };
            string expected = "https://api.twitter.com/1/lists/memberships.xml?user_id=123&screen_name=JoeMayo&cursor=456&filter_to_owned_lists=true";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildMembershipsUrl_Does_Not_Add_False_Filter_To_Url()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Memberships).ToString() },
                { "UserID", "123" },
                { "ScreenName", "JoeMayo" },
                { "Cursor", "456" },
                { "FilterToOwnedLists", "false" }
            };
            string expected = "https://api.twitter.com/1/lists/memberships.xml?user_id=123&screen_name=JoeMayo&cursor=456";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildSubscriptionsUrl_Requires_UserID_Or_ScreenName()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                {"Type", ((int) ListType.Subscriptions).ToString()},
                //{"UserID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("UserIdOrScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildSubscriptionsUrl_Returns_Url()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscriptions).ToString() },
                { "UserID", "123" },
                { "ScreenName", "JoeMayo" },
                { "Cursor", "456" }
            };
            string expected = "https://api.twitter.com/1/lists/subscriptions.xml?user_id=123&screen_name=JoeMayo&cursor=456";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildShowUrl_Requires_ListID_Or_Slug()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Show).ToString()},
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildShowUrl_Requires_Non_Null_And_Not_Empty_ListID_Or_Slug()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Show).ToString()},
                { "ListID", null },
                { "Slug", "" }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildShowUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Show).ToString()},
                { "Slug", "test" }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildShowUrl_Returns_Url()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Show).ToString() },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "ListID", "456" }
            };
            string expected = "https://api.twitter.com/1/lists/show.xml?slug=test&owner_id=123&owner_screen_name=JoeMayo&list_id=456";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildStatusesUrl_Requires_ListID_Or_Slug()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Statuses).ToString()},
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildStatusesUrl_Requires_Non_Null_And_Not_Empty_ListID_Or_Slug()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Statuses).ToString()},
                { "ListID", "" },
                { "Slug", null }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildStatusesUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Statuses).ToString()},
                { "Slug", "test" }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildStatusesUrl_Returns_Url()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Statuses).ToString() },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "ListID", "456" },
                { "SinceID", "789" },
                { "MaxID", "234" },
                { "Count", "25" },
                { "Page", "3" },
                { "TrimUser", "true" },
                { "IncludeEntities", "true" },
                { "IncludeRetweets", "true" },
            };
            string expected = "https://api.twitter.com/1/lists/statuses.xml?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&since_id=789&max_id=234&count=25&per_page=25&page=3&trim_user=true&include_entities=true&include_rts=true";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildStatusesUrl_Does_Not_Include_False_Parameters()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Statuses).ToString() },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "ListID", "456" },
                { "SinceID", "789" },
                { "MaxID", "234" },
                { "Count", "25" },
                { "Page", "3" },
                { "TrimUser", "false" },
                { "IncludeEntities", "false" },
                { "IncludeRetweets", "false" }
            };
            string expected = "https://api.twitter.com/1/lists/statuses.xml?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&since_id=789&max_id=234&count=25&per_page=25&page=3";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildMembersUrl_Requires_ListID_Or_Slug()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Members).ToString()},
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildMembersUrl_Requires_Non_Null_And_Not_Empty_ListID_Or_Slug()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Members).ToString()},
                { "ListID", "" },
                { "Slug", null }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildMembersUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Members).ToString()},
                { "Slug", "test" }
                //{"UserID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildMembersUrl_Returns_Url()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Members).ToString() },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "Cursor", "789" },
                { "IncludeEntities", "true" }
            };
            string expected = "https://api.twitter.com/1/lists/members.xml?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&cursor=789&include_entities=true";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildMembersUrl_Does_Not_Include_False_Parameters()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Members).ToString() },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "Cursor", "789" },
                { "IncludeEntities", "false" }
            };
            string expected = "https://api.twitter.com/1/lists/members.xml?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&cursor=789";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildIsMemberUrl_Requires_ListID_Or_Slug()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsMember).ToString()},
                { "ScreenName", "JoeMayo" },
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildIsMemberUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsMember).ToString()},
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" }
                //{"UserID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildIsMemberUrl_Requires_UserID_Or_ScreenName()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsMember).ToString()},
                { "Slug", "test" },
                {"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("UserIdOrScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildIsMemberUrl_Returns_Url()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsMember).ToString() },
                { "UserID", "789" },
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "IncludeEntities", "true" }
            };
            string expected = "https://api.twitter.com/1/lists/members/show.xml?user_id=789&screen_name=JoeMayo&slug=test&owner_id=123&owner_screen_name=JoeMayo&list_id=456&include_entities=true";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildIsMemberUrl_Does_Not_Include_False_Parameters()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsMember).ToString() },
                { "UserID", "789" },
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "IncludeEntities", "false" }
            };
            string expected = "https://api.twitter.com/1/lists/members/show.xml?user_id=789&screen_name=JoeMayo&slug=test&owner_id=123&owner_screen_name=JoeMayo&list_id=456";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildSubscribersUrl_Requires_ListID_Or_Slug()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscribers).ToString()},
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildSubscribersUrl_Requires_Non_Null_And_Not_Empty_ListID_Or_Slug()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscribers).ToString()},
                { "ListID", "" },
                { "Slug", null }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildSubscribersUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscribers).ToString()},
                { "Slug", "test" }
                //{"UserID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildSubscribersUrl_Returns_Url()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscribers).ToString() },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "Cursor", "789" },
                { "IncludeEntities", "true" }
            };
            string expected = "https://api.twitter.com/1/lists/subscribers.xml?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&cursor=789&include_entities=true";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildSubscribersUrl_Does_Not_Include_False_Parameters()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.Subscribers).ToString() },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "Cursor", "789" },
                { "IncludeEntities", "false" }
            };
            string expected = "https://api.twitter.com/1/lists/subscribers.xml?owner_id=123&owner_screen_name=JoeMayo&slug=test&list_id=456&cursor=789";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildIsSubscribedUrl_Requires_ListID_Or_Slug()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsSubscribed).ToString()},
                { "ScreenName", "JoeMayo" },
                //{ "Slug", "test" }
                //{"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildIsSubscriberUrl_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsSubscribed).ToString()},
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" }
                //{"UserID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildIsSubscriberUrl_Requires_UserID_Or_ScreenName()
        {
            var reqProc = new ListRequestProcessor<List> { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsSubscribed).ToString()},
                { "Slug", "test" },
                {"OwnerID", "123"},
            };

            try
            {
                reqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("UserIdOrScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void BuildIsSubscriberUrl_Returns_Url()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsSubscribed).ToString() },
                { "UserID", "789" },
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "IncludeEntities", "true" }
            };
            string expected = "https://api.twitter.com/1/lists/subscribers/show.xml?user_id=789&screen_name=JoeMayo&slug=test&owner_id=123&owner_screen_name=JoeMayo&list_id=456&include_entities=true";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildIsSubscriberUrl_Does_Not_Include_False_Parameters()
        {
            var listReqProc = new ListRequestProcessor<List>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
            {
                { "Type", ((int) ListType.IsSubscribed).ToString() },
                { "UserID", "789" },
                { "ScreenName", "JoeMayo" },
                { "Slug", "test" },
                { "OwnerID", "123" },
                { "OwnerScreenName", "JoeMayo" },
                { "ListID", "456" },
                { "IncludeEntities", "false" }
            };
            string expected = "https://api.twitter.com/1/lists/subscribers/show.xml?user_id=789&screen_name=JoeMayo&slug=test&owner_id=123&owner_screen_name=JoeMayo&list_id=456";

            Request req = listReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }
    }
}
