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
    ///This is a test class for FavoritesRequestProcessorTest and is intended
    ///to contain all FavoritesRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FavoritesRequestProcessorTest
    {
        private TestContext testContextInstance;

        #region Test Data

        string m_twitterResponse = @"<statuses type=""array"">
  <status>
    <created_at>Sat Apr 18 19:35:19 +0000 2009</created_at>
    <id>1552797863</id>
    <text>Tip: Follow liberally to start. Follow at least 50 people. Also, a MUST: Add a profile photo ASAP. #newtotwitter</text>
    <source>&lt;a href=""http://www.atebits.com/""&gt;Tweetie&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>true</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>5676102</id>
      <name>Scott Hanselman</name>
      <screen_name>shanselman</screen_name>
      <location>Oregon</location>
      <description>Programmer, author, speaker, web guy, podcaster, starving stand-up comic, diabetic, Microsoft shill.</description>
      <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/60143428/hanselman_larger_head_shot_normal.jpg</profile_image_url>
      <url>http://hanselman.com</url>
      <protected>false</protected>
      <followers_count>10957</followers_count>
      <profile_background_color>9ae4e8</profile_background_color>
      <profile_text_color>696969</profile_text_color>
      <profile_link_color>72412c</profile_link_color>
      <profile_sidebar_fill_color>b8aa9c</profile_sidebar_fill_color>
      <profile_sidebar_border_color>b8aa9c</profile_sidebar_border_color>
      <friends_count>763</friends_count>
      <created_at>Tue May 01 05:55:26 +0000 2007</created_at>
      <favourites_count>59</favourites_count>
      <utc_offset>-28800</utc_offset>
      <time_zone>Pacific Time (US &amp; Canada)</time_zone>
      <profile_background_image_url>http://s3.amazonaws.com/twitter_production/profile_background_images/2036752/background.gif</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>7901</statuses_count>
      <notifications>false</notifications>
      <following>0</following>
    </user>
  </status>
  <status>
    <created_at>Sat Apr 18 17:42:32 +0000 2009</created_at>
    <id>1552070127</id>
    <text>Safe URL Shortener. good for facebook and for twitter. give your friends a heads-up before sending them to a generic link.http://safeURL.to</text>
    <source>&lt;a href=""http://www.tweetdeck.com/""&gt;TweetDeck&lt;/a&gt;</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>true</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>21276610</id>
      <name>Reuven</name>
      <screen_name>ETZION</screen_name>
      <location>Jerusalem - City of Gold</location>
      <description>click the link for coolest twitter page idea ever..</description>
      <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/183664543/bg-blu_normal.png</profile_image_url>
      <url>http://retzion.com/?site=twitter</url>
      <protected>false</protected>
      <followers_count>427</followers_count>
      <profile_background_color>3b627e</profile_background_color>
      <profile_text_color>000000</profile_text_color>
      <profile_link_color>17163c</profile_link_color>
      <profile_sidebar_fill_color>6F9BCC</profile_sidebar_fill_color>
      <profile_sidebar_border_color>000000</profile_sidebar_border_color>
      <friends_count>927</friends_count>
      <created_at>Thu Feb 19 04:44:37 +0000 2009</created_at>
      <favourites_count>2</favourites_count>
      <utc_offset>-10800</utc_offset>
      <time_zone>Greenland</time_zone>
      <profile_background_image_url>http://s3.amazonaws.com/twitter_production/profile_background_images/10005208/twitter-bg-tweety.jpg</profile_background_image_url>
      <profile_background_tile>false</profile_background_tile>
      <statuses_count>92</statuses_count>
      <notifications>false</notifications>
      <following>0</following>
    </user>
  </status>
</statuses>";

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
            FavoritesRequestProcessor<Favorites> target = new FavoritesRequestProcessor<Favorites>();

            IList actual = target.ProcessResults(m_twitterResponse);

            var results = actual.Cast<Favorites>();
            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new FavoritesRequestProcessor<Favorites>() { BaseUrl = "http://api.twitter.com/1/" };

            var favorites = reqProc.ProcessResults(string.Empty);

            Assert.AreEqual(0, favorites.Count);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            FavoritesRequestProcessor<Favorites> target = new FavoritesRequestProcessor<Favorites>() { BaseUrl = "http://twitter.com/" };
            Expression<Func<Favorites, bool>> expression =
                fav =>
                    fav.Type == FavoritesType.Favorites &&
                    fav.Page == 1 &&
                    fav.ID == "123";
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)FavoritesType.Favorites).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Page", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "123")));
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildURLTest()
        {
            FavoritesRequestProcessor<Favorites> target = new FavoritesRequestProcessor<Favorites>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FavoritesType.Favorites.ToString() },
                    { "ID", "123" },
                    { "Page", "1" },
                };
            string expected = "http://twitter.com/favorites.xml?page=1&id=123";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            FavoritesRequestProcessor<Favorites> target = new FavoritesRequestProcessor<Favorites>() { BaseUrl = "http://twitter.com/" };
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
            FavoritesRequestProcessor<Favorites> target = new FavoritesRequestProcessor<Favorites>() { BaseUrl = "http://twitter.com/" };
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
