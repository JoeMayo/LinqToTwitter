using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterTests
{
    
    
    /// <summary>
    ///This is a test class for AccountRequestProcessorTest and is intended
    ///to contain all AccountRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AccountRequestProcessorTest
    {
        private TestContext testContextInstance;

        #region Test Data

//        private string m_testVerifyCredentialsQueryResponse = @"<user>
//  <id>15411837</id>
//  <name>Joe Mayo</name>
//  <screen_name>JoeMayo</screen_name>
//  <location>Denver, CO</location>
//  <description>Created LINQ to Twitter, author of LINQ Programming/McGraw-Hill, .NET Consulting, and Microsoft MVP</description>
//  <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/62569644/JoeTwitter_normal.jpg</profile_image_url>
//  <url>http://linqtotwitter.codeplex.com/</url>
//  <protected>false</protected>
//  <followers_count>61</followers_count>
//  <profile_background_color>0099B9</profile_background_color>
//  <profile_text_color>3C3940</profile_text_color>
//  <profile_link_color>0099B9</profile_link_color>
//  <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
//  <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
//  <friends_count>25</friends_count>
//  <created_at>Sun Jul 13 04:35:50 +0000 2008</created_at>
//  <favourites_count>2</favourites_count>
//  <utc_offset>-25200</utc_offset>
//  <time_zone>Mountain Time (US &amp; Canada)</time_zone>
//  <profile_background_image_url>http://static.twitter.com/images/themes/theme4/bg.gif</profile_background_image_url>
//  <profile_background_tile>false</profile_background_tile>
//  <statuses_count>155</statuses_count>
//  <notifications>false</notifications>
//  <following>false</following>
//  <status>
//    <created_at>Sun Apr 26 16:06:04 +0000 2009</created_at>
//    <id>1621219524</id>
//    <text>Flutter might overtake twitter: http://safeurl.to/8U1W; video at bottom of article.</text>
//    <source>web</source>
//    <truncated>false</truncated>
//    <in_reply_to_status_id></in_reply_to_status_id>
//    <in_reply_to_user_id></in_reply_to_user_id>
//    <favorited>false</favorited>
//    <in_reply_to_screen_name></in_reply_to_screen_name>
//  </status>
//</user>";

        private string m_testVerifyCredentialsQueryResponse = @"{
   ""statuses_count"":1624,
   ""favourites_count"":65,
   ""protected"":false,
   ""profile_text_color"":""3C3940"",
   ""profile_image_url"":""http:\/\/a2.twimg.com\/profile_images\/520626655\/JoeTwitterBW_-_150_x_150_normal.jpg"",
   ""name"":""Joe Mayo"",
   ""profile_sidebar_fill_color"":""95E8EC"",
   ""default_profile"":false,
   ""listed_count"":101,
   ""following"":false,
   ""profile_background_tile"":false,
   ""utc_offset"":-25200,
   ""description"":""Independent .NET Consultant; author of 6 books; MVP; interested in C#, MVC, and Language Integrated Query (LINQ)"",
   ""location"":""Denver, CO"",
   ""contributors_enabled"":false,
   ""verified"":false,
   ""profile_link_color"":""0099B9"",
   ""followers_count"":875,
   ""url"":""http:\/\/linqtotwitter.codeplex.com\/"",
   ""status"":{
      ""in_reply_to_status_id_str"":null,
      ""truncated"":false,
      ""in_reply_to_user_id_str"":null,
      ""geo"":null,
      ""retweet_count"":2,
      ""contributors"":null,
      ""coordinates"":null,
      ""favorited"":false,
      ""in_reply_to_screen_name"":null,
      ""source"":""web"",
      ""id_str"":""115131137906589696"",
      ""in_reply_to_status_id"":null,
      ""id"":115131137906589696,
      ""created_at"":""Sat Sep 17 18:32:56 +0000 2011"",
      ""possibly_sensitive"":false,
      ""place"":null,
      ""retweeted"":false,
      ""in_reply_to_user_id"":null,
      ""text"":""Visual Studio 11 Developer Preview: http:\/\/t.co\/fISuavMD""
   },
   ""profile_sidebar_border_color"":""5ED4DC"",
   ""screen_name"":""JoeMayo"",
   ""default_profile_image"":false,
   ""notifications"":false,
   ""show_all_inline_media"":false,
   ""geo_enabled"":true,
   ""profile_use_background_image"":true,
   ""friends_count"":161,
   ""id_str"":""15411837"",
   ""is_translator"":false,
   ""lang"":""en"",
   ""time_zone"":""Mountain Time (US & Canada)"",
   ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
   ""profile_background_color"":""0099B9"",
   ""id"":15411837,
   ""follow_request_sent"":false,
   ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
   ""profile_background_image_url"":""http:\/\/a1.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
   ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/520626655\/JoeTwitterBW_-_150_x_150_normal.jpg""
}";

//        private string m_testRateLimitStatusQueryResponse = @"<hash>
//  <remaining-hits type=""integer"">99</remaining-hits>
//  <hourly-limit type=""integer"">100</hourly-limit>
//  <reset-time type=""datetime"">2009-04-26T20:57:50+00:00</reset-time>
//  <reset-time-in-seconds type=""integer"">1240779470</reset-time-in-seconds>
//</hash>";

        private string m_testRateLimitStatusQueryResponse = @"{
   ""remaining_hits"":343,
   ""hourly_limit"":350,
   ""reset_time_in_seconds"":1316397996,
   ""reset_time"":""Mon Sep 19 02:06:36 +0000 2011""
}";

        private string m_testEndSessionResponse = @"<hash>
  <request>/account/end_session.xml</request>
  <error>Logged out.</error>
</hash>";

        //private string m_testEndSessionResponse = @"";

//        private string m_testTotalsResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
//<hash>
//  <updates type=""integer"">78</updates>
//  <friends type=""integer"">1</friends>
//  <favorites type=""integer"">2</favorites>
//  <followers type=""integer"">26</followers>
//</hash>";

        private string m_testTotalsResponse = @"{
   ""followers"":875,
   ""favorites"":65,
   ""friends"":161,
   ""updates"":1624
}";

        private string m_testSettingsResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<settings>
  <trend_location>
    <woeid>23424977</woeid>
    <name>United States</name>
    <placeTypeName code=""12"">Country</placeTypeName>
    <country type=""Country"" code=""US"">United States</country>
    <url>http://where.yahooapis.com/v1/place/23424977</url>
  </trend_location>
  <geo_enabled>true</geo_enabled>
  <sleep_time>
    <start_time></start_time>
    <enabled>false</enabled>
    <end_time></end_time>
  </sleep_time>
</settings>";

                private string m_testSettingsResponseTimesEnabled = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<settings>
  <trend_location>
    <woeid>23424977</woeid>
    <name>United States</name>
    <placeTypeName code=""12"">Country</placeTypeName>
    <country type=""Country"" code=""US"">United States</country>
    <url>http://where.yahooapis.com/v1/place/23424977</url>
  </trend_location>
  <geo_enabled>true</geo_enabled>
  <sleep_time>
    <start_time>21</start_time>
    <enabled>true</enabled>
    <end_time>8</end_time>
  </sleep_time>
</settings>";

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
        public void ProcessResultsForVerifyCredentialsTest()
        {
            AccountRequestProcessor<Account> target = new AccountRequestProcessor<Account>();

            IList actual = target.ProcessResults(m_testVerifyCredentialsQueryResponse);

            var acct = actual.Cast<Account>().ToList().FirstOrDefault();
            Assert.AreEqual("Joe Mayo", acct.User.Name);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsForRateLimitStatusTest()
        {
            var acctReqProc = new AccountRequestProcessor_Accessor<Account> { Type = AccountType.RateLimitStatus };

            IList actual = acctReqProc.ProcessResults(m_testRateLimitStatusQueryResponse);

            var acct = actual.Cast<Account>().ToList().FirstOrDefault();
            Assert.AreEqual(350, acct.RateLimitStatus.HourlyLimit);
        }

        [TestMethod()]
        public void ProcessResults_Converts_Totals_To_Account()
        {
            var acctReqProc = new AccountRequestProcessor_Accessor<Account> { Type = AccountType.Totals };
            
            List<Account> actual = acctReqProc.ProcessResults(m_testTotalsResponse);

            var acct = actual.FirstOrDefault();

            Assert.AreEqual(1624, acct.Totals.Updates);
            Assert.AreEqual(161, acct.Totals.Friends);
            Assert.AreEqual(65, acct.Totals.Favorites);
            Assert.AreEqual(875, acct.Totals.Followers);
        }

        [Ignore]
        [TestMethod()]
        public void ProcessResults_Converts_Settings_To_Account()
        {
            var acctReqProc = new AccountRequestProcessor<Account>();

            List<Account> actual = acctReqProc.ProcessResults(m_testSettingsResponse);

            var acct = actual.FirstOrDefault();

            Assert.AreEqual("23424977", acct.Settings.TrendLocation.WoeID);
            Assert.AreEqual("United States", acct.Settings.TrendLocation.Name);
            Assert.AreEqual(12, acct.Settings.TrendLocation.PlaceTypeNameCode);
            Assert.AreEqual("Country", acct.Settings.TrendLocation.PlaceTypeName);
            Assert.AreEqual("US", acct.Settings.TrendLocation.CountryCode);
            Assert.AreEqual("United States", acct.Settings.TrendLocation.Country);
            Assert.AreEqual("http://where.yahooapis.com/v1/place/23424977", acct.Settings.TrendLocation.Url);
            Assert.AreEqual(true, acct.Settings.GeoEnabled);
            Assert.AreEqual(false, acct.Settings.SleepTime.Enabled);
            Assert.AreEqual(null, acct.Settings.SleepTime.StartHour);
            Assert.AreEqual(null, acct.Settings.SleepTime.EndHour);
        }

        [Ignore]
        [TestMethod()]
        public void ProcessResults_Converts_Settings_When_Times_Are_Enabled()
        {
            var acctReqProc = new AccountRequestProcessor<Account>();

            List<Account> actual = acctReqProc.ProcessResults(m_testSettingsResponseTimesEnabled);

            var acct = actual.FirstOrDefault();

            Assert.AreEqual(true, acct.Settings.SleepTime.Enabled);
            Assert.AreEqual(21, acct.Settings.SleepTime.StartHour);
            Assert.AreEqual(8, acct.Settings.SleepTime.EndHour);
        }

        [TestMethod]
        public void ProcessResults_Handles_EndSession()
        {
            var acctReqProc = new AccountRequestProcessor<Account>();
            
            IList actual = acctReqProc.ProcessResults(m_testEndSessionResponse);

            var acct = actual.Cast<Account>().ToList().FirstOrDefault();
            Assert.AreEqual("Logged out.", acct.EndSessionStatus.Error);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new AccountRequestProcessor<Account>() { BaseUrl = "http://api.twitter.com/1/" };

            var accts = reqProc.ProcessResults(string.Empty);

            Assert.AreEqual(0, accts.Count);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            AccountRequestProcessor<Account> target = new AccountRequestProcessor<Account>();
            Expression<Func<Account, bool>> expression = acct => acct.Type == AccountType.RateLimitStatus;
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)AccountType.RateLimitStatus).ToString())));
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildVerifyCredentialsStatusURLTest()
        {
            AccountRequestProcessor<Account> target = new AccountRequestProcessor<Account>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.VerifyCredentials).ToString() }
                };
            string expected = "https://api.twitter.com/1/account/verify_credentials.json";
            
            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildRateLimitStatusURLTest()
        {
            AccountRequestProcessor<Account> target = new AccountRequestProcessor<Account>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.RateLimitStatus).ToString() }
                };
            string expected = "https://api.twitter.com/1/account/rate_limit_status.json";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod()]
        public void BuildUrl_Returns_Totals_Url()
        {
            var acctReqProc = new AccountRequestProcessor<Account>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.Totals).ToString() }
                };
            string expected = "https://api.twitter.com/1/account/totals.json";

            Request req = acctReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod()]
        public void BuildUrl_Returns_Settings_Url()
        {
            var acctReqProc = new AccountRequestProcessor<Account>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.Settings).ToString() }
                };
            string expected = "https://api.twitter.com/1/account/settings.json";

            Request req = acctReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            AccountRequestProcessor<Account> target = new AccountRequestProcessor<Account>() { BaseUrl = "http://twitter.com/" };
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
            AccountRequestProcessor<Account> target = new AccountRequestProcessor<Account>() { BaseUrl = "http://twitter.com/" };
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
