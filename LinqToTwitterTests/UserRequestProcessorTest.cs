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

        private string m_testCategoriesResponse = @"<suggestions type=""array"">
  <category>
    <name>Art &amp; Design</name>
    <slug>art-design</slug>
  </category>
  <category>
    <name>Books</name>
    <slug>books</slug>
  </category>
  <category>
    <name>Business</name>
    <slug>business</slug>
  </category>
  <category>
    <name>Charity</name>
    <slug>charity</slug>
  </category>
  <category>
    <name>Entertainment</name>
    <slug>entertainment</slug>
  </category>
</suggestions>";

        private string m_usersInCategoryResponse = @"<category>
  <name>Funny</name>
  <slug>funny</slug>
  <categories type=""array""></categories>
  <users type=""array"">
    <user>
      <id>44039298</id>
      <name>Seth Meyers</name>
      <screen_name>sethmeyers21</screen_name>
      <location>New York</location>
      <description></description>
      <profile_image_url>http://a1.twimg.com/profile_images/267298914/n700068668_5523_normal.jpg</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>98119</followers_count>
      <profile_background_color>9ae4e8</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>0000ff</profile_link_color>
      <profile_sidebar_fill_color>e0ff92</profile_sidebar_fill_color>
      <profile_sidebar_border_color>87bc44</profile_sidebar_border_color>
      <friends_count>61</friends_count>
      <created_at>Tue Jun 02 02:35:39 +0000 2009</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset></utc_offset>
      <time_zone></time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1269020849/images/themes/theme1/bg.png</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>true</verified>
      <following>false</following>
      <statuses_count>668</statuses_count>
      <lang>en</lang>
      <contributors_enabled>false</contributors_enabled>
    </user>
    <user>
      <id>17453527</id>
      <name>Todd Barry</name>
      <screen_name>toddbarry</screen_name>
      <location>New York City</location>
      <description>Steven Dewall took my profile photo.</description>
      <profile_image_url>http://a1.twimg.com/profile_images/304302794/ToddBarry_John28_normal.jpg</profile_image_url>
      <url>http://www.toddbarry.com</url>
      <protected>false</protected>
      <followers_count>26065</followers_count>
      <profile_background_color>022330</profile_background_color>
      <profile_text_color>333333</profile_text_color>
      <profile_link_color>0084B4</profile_link_color>
      <profile_sidebar_fill_color>C0DFEC</profile_sidebar_fill_color>
      <profile_sidebar_border_color>a8c7f7</profile_sidebar_border_color>
      <friends_count>244</friends_count>
      <created_at>Mon Nov 17 23:32:30 +0000 2008</created_at>
      <favourites_count>36</favourites_count>
      <utc_offset>-18000</utc_offset>
      <time_zone>Eastern Time (US &amp; Canada)</time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1269020849/images/themes/theme14/bg.gif</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>true</verified>
      <following>false</following>
      <statuses_count>555</statuses_count>
      <lang>en</lang>
      <contributors_enabled>false</contributors_enabled>
    </user>
  </users>
</category>";

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
            UserRequestProcessor<User> reqProc = new UserRequestProcessor<User>();
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
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
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
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
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
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
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
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
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
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
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
        ///A test for BuildFollowersUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildSuggestionCategoriesUrlTest()
        {
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
            reqProc.BaseUrl = "https://api.twitter.com/1/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Categories).ToString() }
                    };
            string expected = "https://api.twitter.com/1/users/suggestions.xml";
            var actual = reqProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildCategoriesUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildSuggestionCategoryUsersUrlTest()
        {
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
            reqProc.BaseUrl = "https://api.twitter.com/1/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Category).ToString() },
                        { "Slug", "twitter" }
                    };
            string expected = "https://api.twitter.com/1/users/suggestions/twitter.xml";
            var actual = reqProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildUsersInCategoryUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildSuggestionCategoryUsersUrlWithoutSlugTest()
        {
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
            reqProc.BaseUrl = "https://api.twitter.com/1/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Category).ToString() },
                    };
            var actual = reqProc.BuildURL(parameters);
        }

        /// <summary>
        ///A test for BuildLookupsUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildLookupScreenNameUrlTest()
        {
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
            reqProc.BaseUrl = "https://api.twitter.com/1/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Lookup).ToString() },
                        { "ScreenName", "JoeMayo,LinqToTweeter" }
                    };
            string expected = "https://api.twitter.com/1/users/lookup.xml?screen_name=JoeMayo,LinqToTweeter";
            var actual = reqProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildLookupsUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildLookupUserIDUrlTest()
        {
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
            reqProc.BaseUrl = "https://api.twitter.com/1/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Lookup).ToString() },
                        { "UserID", "1,2" }
                    };
            string expected = "https://api.twitter.com/1/users/lookup.xml?user_id=1,2";
            var actual = reqProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildLookupsUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildLookupScreenNameUrlWithoutParamsTest()
        {
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
            reqProc.BaseUrl = "https://api.twitter.com/1/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Lookup).ToString() },
                    };
            var actual = reqProc.BuildURL(parameters);
        }

        /// <summary>
        ///A test for BuildLookupsUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildLookupScreenNameUrlWithUserIDAndScreenNameTest()
        {
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
            reqProc.BaseUrl = "https://api.twitter.com/1/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Lookup).ToString() },
                        { "ScreenName", "JoeMayo,LinqToTweeter" },
                        { "UserID", "1,2" }
                   };
            var actual = reqProc.BuildURL(parameters);
        }

        /// <summary>
        ///A test for BuildSearchUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildSearchUrlTest()
        {
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
            reqProc.BaseUrl = "https://api.twitter.com/1/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Search).ToString() },
                        { "Query", "Joe Mayo" },
                        { "Page", "2" },
                        { "PerPage", "10" }
                    };
            string expected = "https://api.twitter.com/1/users/search.xml?q=Joe+Mayo&page=2&per_page=10";
            var actual = reqProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildSearchUrl
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildSearchUrlWithoutQueryTest()
        {
            UserRequestProcessor_Accessor<User> reqProc = new UserRequestProcessor_Accessor<User>();
            reqProc.BaseUrl = "https://api.twitter.com/1/";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                    {
                        { "Type", ((int)UserType.Search).ToString() },
                    };
            var actual = reqProc.BuildURL(parameters);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            var reqProc = new UserRequestProcessor<User>();

            Expression<Func<User, bool>> expression =
            user =>
                user.Type == UserType.Friends &&
                user.ID == "10" &&
                user.UserID == "10" &&
                user.ScreenName == "JoeMayo" &&
                user.Cursor == "10819235" &&
                user.Slug == "twitter" &&
                user.Query == "Joe Mayo" &&
                user.Page == 2 &&
                user.PerPage == 10;

            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = reqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)UserType.Friends).ToString())));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>("ID", "10")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("UserID", "10")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>("ScreenName", "JoeMayo")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>("ID", "10")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Cursor", "10819235")));
            Assert.IsTrue(
               queryParams.Contains(
                   new KeyValuePair<string, string>("Slug", "twitter")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Query", "Joe Mayo")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("Page", "2")));
            Assert.IsTrue(
              queryParams.Contains(
                  new KeyValuePair<string, string>("PerPage", "10")));
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsSingleResultTest()
        {
            var statProc = new UserRequestProcessor<User>() { BaseUrl = "http://twitter.com/" };
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
            var statProc = new UserRequestProcessor<User>() { BaseUrl = "http://twitter.com/" };
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
        public void ProcessCategoresResultsTest()
        {
            var statProc = new UserRequestProcessor<User>() { BaseUrl = "http://api.twitter.com/1/" };
            XElement twitterResponse = XElement.Load(new StringReader(m_testCategoriesResponse));
            var actual = (List<User>)statProc.ProcessResults(twitterResponse);
            Assert.AreEqual(5, actual[0].Categories.Count);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessUsersInCategoryResultsTest()
        {
            var statProc = new UserRequestProcessor<User>() { BaseUrl = "http://api.twitter.com/1/" };
            XElement twitterResponse = XElement.Load(new StringReader(m_usersInCategoryResponse));
            var actual = (List<User>)statProc.ProcessResults(twitterResponse);
            Assert.AreEqual(2, actual.Count);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            UserRequestProcessor<User> target = new UserRequestProcessor<User>() { BaseUrl = "http://twitter.com/" };
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
            UserRequestProcessor<User> target = new UserRequestProcessor<User>() { BaseUrl = "http://twitter.com/" };
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
