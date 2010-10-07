using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System;
using LinqToTwitterTests.Common;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for DirectMessageRequestProcessorTest and is intended
    ///to contain all DirectMessageRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DirectMessageRequestProcessorTest
    {
        #region Test Data

        private string m_testQueryResponse = @"
        <direct_message>
          <id>87864628</id>
          <sender_id>1234567</sender_id>
          <text>;)</text>
          <recipient_id>15411837</recipient_id>
          <created_at>Tue Apr 07 16:47:25 +0000 2009</created_at>
          <sender_screen_name>senderscreenname</sender_screen_name>
          <recipient_screen_name>JoeMayo</recipient_screen_name>
          <sender>
            <id>1234567</id>
            <name>Sender Name</name>
            <screen_name>senderscreenname</screen_name>
            <location>SenderLocation</location>
            <description>Sender Description</description>
            <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/12345678/name_of_image.jpg</profile_image_url>
            <url>http://sendersite.com</url>
            <protected>false</protected>
            <followers_count>10406</followers_count>
            <profile_background_color>9ae4e8</profile_background_color>
            <profile_text_color>696969</profile_text_color>
            <profile_link_color>72412c</profile_link_color>
            <profile_sidebar_fill_color>b8aa9c</profile_sidebar_fill_color>
            <profile_sidebar_border_color>b8aa9c</profile_sidebar_border_color>
            <friends_count>705</friends_count>
            <created_at>Tue May 01 05:55:26 +0000 2007</created_at>
            <favourites_count>56</favourites_count>
            <utc_offset>-28800</utc_offset>
            <time_zone>Pacific Time (US &amp; Canada)</time_zone>
            <profile_background_image_url>http://s3.amazonaws.com/twitter_production/profile_background_images/2036752/background.gif</profile_background_image_url>
            <profile_background_tile>true</profile_background_tile>
            <statuses_count>7607</statuses_count>
            <notifications>false</notifications>
            <following>true</following>
          </sender>
          <recipient>
            <id>15411837</id>
            <name>Joe Mayo</name>
            <screen_name>JoeMayo</screen_name>
            <location>Denver, CO</location>
            <description>Author/entrepreneur, specializing in custom .NET software development</description>
            <profile_image_url>http://s3.amazonaws.com/twitter_production/profile_images/62569644/JoeTwitter_normal.jpg</profile_image_url>
            <url>http://www.csharp-station.com</url>
            <protected>false</protected>
            <followers_count>47</followers_count>
            <profile_background_color>0099B9</profile_background_color>
            <profile_text_color>3C3940</profile_text_color>
            <profile_link_color>0099B9</profile_link_color>
            <profile_sidebar_fill_color>95E8EC</profile_sidebar_fill_color>
            <profile_sidebar_border_color>5ED4DC</profile_sidebar_border_color>
            <friends_count>22</friends_count>
            <created_at>Sun Jul 13 04:35:50 +0000 2008</created_at>
            <favourites_count>0</favourites_count>
            <utc_offset>-25200</utc_offset>
            <time_zone>Mountain Time (US &amp; Canada)</time_zone>
            <profile_background_image_url>http://static.twitter.com/images/themes/theme4/bg.gif</profile_background_image_url>
            <profile_background_tile>false</profile_background_tile>
            <statuses_count>137</statuses_count>
            <notifications>false</notifications>
            <following>false</following>
          </recipient>
        </direct_message>";

        #endregion

        private TestContext testContextInstance;

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
        ///A test for BuildUrl
        ///</summary>
        [TestMethod()]
        public void BuildUrlSentToSinceIDTest()
        {
            var dmProc = new DirectMessageRequestProcessor<DirectMessage>() { BaseUrl = "http://twitter.com/" };
            string expected = "http://twitter.com/direct_messages.xml?since_id=1234567&max_id=357&page=1&count=2";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageType.SentTo).ToString() },
                        { "SinceID", "1234567" },
                        { "MaxID", "357" },
                        { "Page", "1" },
                        { "Count", "2" }
                };
            string actual = dmProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildUrl
        ///</summary>
        [TestMethod()]
        public void BuildUrlSentByTest()
        {
            var dmProc = new DirectMessageRequestProcessor<DirectMessage>() { BaseUrl = "http://twitter.com/" };
            string expected = "http://twitter.com/direct_messages/sent.xml?since_id=1234567&max_id=357&page=1&count=2";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageType.SentBy).ToString() },
                        { "SinceID", "1234567" },
                        { "MaxID", "357" },
                        { "Page", "1" },
                        { "Count", "2" }
                };
            string actual = dmProc.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void BuildUrlShow_Returns_Url()
        {
            var dmProc = new DirectMessageRequestProcessor<DirectMessage>() { BaseUrl = "https://api.twitter.com/1/" };
            string expected = "https://api.twitter.com/1/direct_messages/show/478805447.xml";
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageType.Show).ToString() },
                        { "ID", "478805447" },
                };

            string actual = dmProc.BuildURL(parameters);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void BuildUrlShow_Requires_ID()
        {
            var dmProc = new DirectMessageRequestProcessor<DirectMessage>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                        { "Type", ((int)DirectMessageType.Show).ToString() },
                        //{ "ID", "478805447" },
                };

            try
            {
                string actual = dmProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("ID", ane.ParamName);
            }
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessResultsTest()
        {
            var dmProc = new DirectMessageRequestProcessor<DirectMessage>() { BaseUrl = "http://twitter.com/" };

            var actual = dmProc.ProcessResults(m_testQueryResponse);

            var actualQuery = actual as IList<DirectMessage>;
            Assert.IsNotNull(actualQuery);
            Assert.AreEqual(1 ,actualQuery.Count());
        }

        [TestMethod()]
        public void GetParameters_Returns_Parameters()
        {
            var dmProc = new DirectMessageRequestProcessor<DirectMessage>();
            Expression<Func<DirectMessage, bool>> expression =
                dm =>
                    dm.Type == DirectMessageType.Show &&
                    dm.Count == 1 &&
                    dm.MaxID == 789 &&
                    dm.Page == 1 &&
                    dm.SinceID == 123 &&
                    dm.ID == 456;
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = dmProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)DirectMessageType.Show).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Count", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("MaxID", "789")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Page", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SinceID", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "456")));
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            DirectMessageRequestProcessor<DirectMessage> target = new DirectMessageRequestProcessor<DirectMessage>() { BaseUrl = "http://twitter.com/" };
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
            DirectMessageRequestProcessor<DirectMessage> target = new DirectMessageRequestProcessor<DirectMessage>() { BaseUrl = "http://twitter.com/" };
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
