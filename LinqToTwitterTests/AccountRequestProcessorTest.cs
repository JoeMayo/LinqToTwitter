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
    ///This is a test class for AccountRequestProcessorTest and is intended
    ///to contain all AccountRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AccountRequestProcessorTest
    {
        private TestContext testContextInstance;

        private string m_testVerifyCredentialsQueryResponse = @"<user>
  <id>15411837</id>
  <name>Joe Mayo</name>
  <screen_name>JoeMayo</screen_name>
  <location>Denver, CO</location>
  <description>Created LINQ to Twitter, author of LINQ Programming/McGraw-Hill, .NET Consulting, and Microsoft MVP</description>
  <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/62569644/JoeTwitter_normal.jpg</profile_image_url>
  <url>http://linqtotwitter.codeplex.com/</url>
  <protected>false</protected>
  <followers_count>61</followers_count>
  <profile_background_color>0099B9</profile_background_color>
  <profile_text_color>3C3940</profile_text_color>
  <profile_link_color>0099B9</profile_link_color>
  <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
  <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
  <friends_count>25</friends_count>
  <created_at>Sun Jul 13 04:35:50 +0000 2008</created_at>
  <favourites_count>2</favourites_count>
  <utc_offset>-25200</utc_offset>
  <time_zone>Mountain Time (US &amp; Canada)</time_zone>
  <profile_background_image_url>http://static.twitter.com/images/themes/theme4/bg.gif</profile_background_image_url>
  <profile_background_tile>false</profile_background_tile>
  <statuses_count>155</statuses_count>
  <notifications>false</notifications>
  <following>false</following>
  <status>
    <created_at>Sun Apr 26 16:06:04 +0000 2009</created_at>
    <id>1621219524</id>
    <text>Flutter might overtake twitter: http://safeurl.to/8U1W; video at bottom of article.</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
  </status>
</user>";

        private string m_testRateLimitStatusQueryResponse = @"<hash>
  <remaining-hits type=""integer"">99</remaining-hits>
  <hourly-limit type=""integer"">100</hourly-limit>
  <reset-time type=""datetime"">2009-04-26T20:57:50+00:00</reset-time>
  <reset-time-in-seconds type=""integer"">1240779470</reset-time-in-seconds>
</hash>";

        private string m_testInvalidValueQueryResponse = @"<invalidValue></invalidValue>";

        private string m_testEndSessionResponse = @"<hash>
  <request>/account/end_session.xml</request>
  <error>Logged out.</error>
</hash>";

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
            AccountRequestProcessor target = new AccountRequestProcessor();
            XElement twitterResponse = XElement.Parse(m_testVerifyCredentialsQueryResponse);
            IList actual = target.ProcessResults(twitterResponse);
            var acct = actual.Cast<Account>().ToList().FirstOrDefault();

            Assert.AreEqual("Joe Mayo", acct.User.Name);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsForRateLimitStatusTest()
        {
            AccountRequestProcessor target = new AccountRequestProcessor();
            XElement twitterResponse = XElement.Parse(m_testRateLimitStatusQueryResponse);
            IList actual = target.ProcessResults(twitterResponse);
            var acct = actual.Cast<Account>().ToList().FirstOrDefault();

            Assert.AreEqual(100, acct.RateLimitStatus.HourlyLimit);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessResultsForInvalidValueTest()
        {
            AccountRequestProcessor target = new AccountRequestProcessor();
            XElement twitterResponse = XElement.Parse(m_testInvalidValueQueryResponse);
            IList actual = target.ProcessResults(twitterResponse);
            var acct = actual.Cast<Account>().ToList().FirstOrDefault();
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsForEndSessionTest()
        {
            AccountRequestProcessor target = new AccountRequestProcessor();
            XElement twitterResponse = XElement.Parse(m_testEndSessionResponse);
            IList actual = target.ProcessResults(twitterResponse);
            var acct = actual.Cast<Account>().ToList().FirstOrDefault();

            Assert.AreEqual("Logged out.", acct.EndSessionStatus.Error);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            AccountRequestProcessor target = new AccountRequestProcessor();
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
            AccountRequestProcessor target = new AccountRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.VerifyCredentials).ToString() }
                };
            string expected = "http://twitter.com/account/verify_credentials.xml";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildRateLimitStatusURLTest()
        {
            AccountRequestProcessor target = new AccountRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)AccountType.RateLimitStatus).ToString() }
                };
            string expected = "http://twitter.com/account/rate_limit_status.xml";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }
    }
}
