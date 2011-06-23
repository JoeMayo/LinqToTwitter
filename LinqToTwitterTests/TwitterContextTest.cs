using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LinqToTwitterTests
{
    [TestClass]
    public class TwitterContextTest
    {
        private TestContext m_testContextInstance;

        #region Test Data

        private string m_testUserQueryResponse =
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

        private string m_testStatusQueryResponse = @"<statuses type=""array"">
  <status>
    <created_at>Fri Nov 27 18:28:57 +0000 2009</created_at>
    <id>6118906745</id>
    <text>ah,vou lá comer</text>
    <source>web</source>
    <truncated>false</truncated>
    <in_reply_to_status_id></in_reply_to_status_id>
    <in_reply_to_user_id></in_reply_to_user_id>
    <favorited>false</favorited>
    <in_reply_to_screen_name></in_reply_to_screen_name>
    <user>
      <id>77880019</id>
      <name>caah </name>
      <screen_name>caahbuss</screen_name>
      <location></location>
      <description></description>
      <profile_image_url>http://a1.twimg.com/profile_images/440024240/d_normal.JPG</profile_image_url>
      <url></url>
      <protected>false</protected>
      <followers_count>48</followers_count>
      <profile_background_color>131516</profile_background_color>
      <profile_text_color>333333</profile_text_color>
      <profile_link_color>009999</profile_link_color>
      <profile_sidebar_fill_color>efefef</profile_sidebar_fill_color>
      <profile_sidebar_border_color>eeeeee</profile_sidebar_border_color>
      <friends_count>47</friends_count>
      <created_at>Mon Sep 28 00:47:48 +0000 2009</created_at>
      <favourites_count>0</favourites_count>
      <utc_offset></utc_offset>
      <time_zone></time_zone>
      <profile_background_image_url>http://s.twimg.com/a/1259091217/images/themes/theme14/bg.gif</profile_background_image_url>
      <profile_background_tile>true</profile_background_tile>
      <statuses_count>211</statuses_count>
      <notifications>false</notifications>
      <geo_enabled>false</geo_enabled>
      <verified>false</verified>
      <following>false</following>
    </user>
    <geo />
  </status>
</statuses>";

        private string m_testDirectMessageQueryResponse = @"
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

        private string m_testEndSessionResponse = @"<hash>
  <request>/account/end_session.xml</request>
  <error>Logged out.</error>
</hash>";

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return m_testContextInstance;
            }
            set
            {
                m_testContextInstance = value;
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }

        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region Constructor Tests

        /// <summary>
        ///1 param constructor defaults
        ///</summary>
        [TestMethod()]
        public void OneParamCtorDefaults()
        {
            string baseUrl = "https://api.twitter.com/1/";
            string searchUrl = "http://search.twitter.com/";
            ITwitterAuthorizer authorizedClient = new PinAuthorizer();
            TwitterContext ctx = new TwitterContext(authorizedClient);

            Assert.AreSame(authorizedClient, ctx.AuthorizedClient);
            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        /// <summary>
        ///3 param constructor defaults
        ///</summary>
        [TestMethod()]
        public void ThreeParamCtorDefaults()
        {
            ITwitterExecute execute = new TwitterExecute(new PinAuthorizer());
            string baseUrl = "http://api.twitter.com/1/";
            string searchUrl = "http://search.twitter.com/";
            TwitterContext ctx = new TwitterContext(execute, baseUrl, searchUrl);

            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        /// <summary>
        ///3 param constructor defaults
        ///</summary>
        [TestMethod()]
        public void ObjectInitializerTest()
        {
            string baseUrl = "http://api.twitter.com/1/";
            string searchUrl = "http://search.twitter.com/";
            TwitterContext ctx =
                new TwitterContext
                {
                    BaseUrl = baseUrl,
                    SearchUrl = searchUrl,
                };

            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        [TestMethod]
        public void TwitterContext_1_Param_Requres_NonNull_Authorization()
        {
            try
            {
                PinAuthorizer auth = null;

                new TwitterContext(auth);

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("authorizedClient", ane.ParamName);
            }
        }

        [TestMethod]
        public void TwitterContext_4_Params_Requres_NonNull_Authorization()
        {
            try
            {
                var execMock = new Mock<ITwitterExecute>();
                new TwitterContext(null, execMock.Object, "", "");

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("authorization", ane.ParamName);
            }
        }

        [TestMethod]
        public void TwitterContext_Requres_NonNull_Executor()
        {
            try
            {
                var authMock = new Mock<ITwitterAuthorizer>();
                new TwitterContext(authMock.Object, null, "", "");

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("execute", ane.ParamName);
            }
        }

        #endregion

        #region CreateRequestProcessor Tests

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        public void CreateRequestProcessorTestHelper<T>()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var publicQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            var statusProc = ctx.CreateRequestProcessor<Status>(publicQuery.Expression);
            Assert.IsInstanceOfType(statusProc, typeof(StatusRequestProcessor<Status>));
        }

        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void CreateRequestProcessorTest()
        {
            CreateRequestProcessorTestHelper<GenericParameterHelper>();
        }

        [TestMethod]
        public void CreateStatusRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Status select tweet;

            IRequestProcessor<Status> actual = ctx.CreateRequestProcessor<Status>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(StatusRequestProcessor<Status>));
        }

        [TestMethod]
        public void CreateAccountRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Account select tweet;

            IRequestProcessor<Account> actual = ctx.CreateRequestProcessor<Account>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(AccountRequestProcessor<Account>));
        }

        [TestMethod]
        public void CreateBlocksRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Blocks select tweet;

            IRequestProcessor<Blocks> actual = ctx.CreateRequestProcessor<Blocks>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(BlocksRequestProcessor<Blocks>));
        }

        [TestMethod]
        public void CreateDirectMessageRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.DirectMessage select tweet;

            IRequestProcessor<DirectMessage> actual = ctx.CreateRequestProcessor<DirectMessage>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(DirectMessageRequestProcessor<DirectMessage>));
        }

        [TestMethod]
        public void CreateFavoritesRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Favorites select tweet;

            IRequestProcessor<Favorites> actual = ctx.CreateRequestProcessor<Favorites>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(FavoritesRequestProcessor<Favorites>));
        }

        [TestMethod]
        public void CreateFriendshipRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Friendship select tweet;

            IRequestProcessor<Friendship> actual = ctx.CreateRequestProcessor<Friendship>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(FriendshipRequestProcessor<Friendship>));
        }

        [TestMethod]
        public void CreateSearchRequestProcessor_Returns_RawRequestProcessor()
        {
            var ctx = new TwitterContext_Accessor();

            var queryResult = from raw in ctx.RawQuery select raw;

            IRequestProcessor<Raw> actual = ctx.CreateRequestProcessor<Raw>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(RawRequestProcessor<Raw>));
        }

        [TestMethod]
        public void CreateSearchRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Search select tweet;

            IRequestProcessor<Search> actual = ctx.CreateRequestProcessor<Search>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(SearchRequestProcessor<Search>));
        }

        [TestMethod]
        public void CreateSocialGraphRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.SocialGraph select tweet;

            IRequestProcessor<SocialGraph> actual = ctx.CreateRequestProcessor<SocialGraph>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(SocialGraphRequestProcessor<SocialGraph>));
        }

        [TestMethod]
        public void CreateTrendRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.Trends select tweet;

            IRequestProcessor<Trend> actual = ctx.CreateRequestProcessor<Trend>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(TrendRequestProcessor<Trend>));
        }

        [TestMethod]
        public void CreateUserRequestProcessorTest()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            var queryResult = from tweet in ctx.User select tweet;

            IRequestProcessor<User> actual = ctx.CreateRequestProcessor<User>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(UserRequestProcessor<User>));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateRequestProcessorNullExpressionTest1()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();

            ctx.CreateRequestProcessor<Status>(null);
        }

        [TestMethod]
        public void CreateRequestProcessor_Returns_LegalRequestProcessor()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();
            ctx.BaseUrl = "http://stream.twitter.com/1/";
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var legalQuery =
                from tweet in ctx.Legal
                where tweet.Type == LegalType.Privacy
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<Legal>(legalQuery.Expression);

            Assert.IsInstanceOfType(reqProc, typeof(LegalRequestProcessor<Legal>));
            Assert.AreEqual("http://stream.twitter.com/1/", reqProc.BaseUrl);
        }

        [TestMethod]
        public void CreateRequestProcessor_Returns_RelatedResultsRequestProcessor()
        {
            var ctx = new TwitterContext_Accessor();
            ctx.BaseUrl = "http://api.twitter.com/1/";
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var resultsQuery =
                from tweet in ctx.RelatedResults
                where tweet.Type == RelatedResultsType.Show
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<RelatedResults>(resultsQuery.Expression);

            Assert.IsInstanceOfType(reqProc, typeof(RelatedResultsRequestProcessor<RelatedResults>));
            Assert.AreEqual("http://api.twitter.com/1/", reqProc.BaseUrl);
        }

        #endregion

        #region Execute Tests

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            var ctx = new TwitterContext();

            //Log
            ctx.Log = new LinqToTwitterTests.Common.DebuggerWriter();

            var publicQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            var actual = ctx.Execute<Status>(publicQuery.Expression, true);
            var tweets = actual as IEnumerable<Status>;
            Assert.IsNotNull(tweets);
            Assert.IsTrue(tweets.ToList().Count > 0);
        }

        #endregion

        #region API Tests

        #region Status Tests

        [TestMethod]
        public void UpdateStatus_With_Reply_Sets_StatusID()
        {
            string status = "Hello";
            string inReplyToStatusID = "1";
            var expected = XElement.Parse(m_testStatusQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            execMock.Setup(exec => exec.ExecuteTwitter(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<IRequestProcessor<Status>>()))
                    .Returns(m_testStatusQueryResponse);

            Status actual = ctx.UpdateStatus(status, inReplyToStatusID);

            Assert.AreEqual(expected.Element("status").Element("id").Value, actual.StatusID);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateStatusNullStatusTest1()
        {
            string status = null;
            string inReplyToStatusID = "1";
            Status expected = new Status();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Status>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            Status actual = ctx.UpdateStatus(status, inReplyToStatusID);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateStatusOver140Test1()
        {
            string status = new string(Enumerable.Repeat<char>('x', 141).ToArray());
            string inReplyToStatusID = "1";
            Status expected = new Status();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Status>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            Status actual = ctx.UpdateStatus(status, inReplyToStatusID);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdateStatus_Sets_StatusID()
        {
            string status = "Hello";
            XElement expected = XElement.Parse(m_testStatusQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            execMock.Setup(exec => exec.ExecuteTwitter(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<IRequestProcessor<Status>>()))
                    .Returns(m_testStatusQueryResponse);

            Status actual = ctx.UpdateStatus(status);

            Assert.AreEqual(expected.Element("status").Element("id").Value, actual.StatusID);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateStatusNullStatusTest()
        {
            string status = null;
            XElement expected = XElement.Parse(m_testStatusQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Status>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            Status actual = ctx.UpdateStatus(status);

            Assert.AreEqual(expected.Element("status").Element("id").Value, actual.StatusID);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateStatusOver140Test()
        {
            string status = new string(Enumerable.Repeat<char>('x', 141).ToArray());
            XElement expected = XElement.Parse(m_testStatusQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Status>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            Status actual = ctx.UpdateStatus(status);

            Assert.AreEqual(expected.Element("status").Element("id").Value, actual.StatusID);
        }

        [TestMethod]
        public void DestroyStatusTest()
        {
            string id = "1";
            XElement expected = XElement.Parse(m_testStatusQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Status>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            Status actual = ctx.DestroyStatus(id);

            Assert.AreEqual(expected.Element("status").Element("id").Value, actual.StatusID);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DestroyStatusNullStatusTest()
        {
            string id = string.Empty;
            Status expected = new Status();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Status>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            Status actual = ctx.DestroyStatus(id);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Account Tests

        [TestMethod]
        public void UpdateAccountProfileTest()
        {
            string name = "Joe";
            string url = "http://www.csharp-station.com";
            string location = "Denver, CO";
            string description = "Open source developer for LINQ to Twitter";
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(actual.Name, expected.Element("name").Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileNullInputTest()
        {
            string name = string.Empty;
            string url = string.Empty;
            string location = string.Empty;
            string description = string.Empty;
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileNameOver20Test()
        {
            string name = new string(Enumerable.Repeat<char>('x', 21).ToArray());
            string url = "http://www.csharp-station.com";
            string location = "Denver, CO";
            string description = "Open source developer for LINQ to Twitter";
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileUrlOver100Test()
        {
            string name = "Joe";
            string url = new string(Enumerable.Repeat<char>('x', 101).ToArray());
            string location = "Denver, CO";
            string description = "Open source developer for LINQ to Twitter";
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileLocationOver30Test()
        {
            string name = "Joe";
            string url = "http://www.csharp-station.com";
            string location = new string(Enumerable.Repeat<char>('x', 31).ToArray());
            string description = "Open source developer for LINQ to Twitter";
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileDescriptionOver160Test()
        {
            string name = "Joe";
            string url = "http://www.csharp-station.com";
            string location = "Denver, CO";
            string description = new string(Enumerable.Repeat<char>('x', 161).ToArray());
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdateAccountImageTest()
        {
            string imageFilePath = "c:\\image.jpg";
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostTwitterFile<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountImage(imageFilePath);

            Assert.AreEqual(expected.Element("name").Value, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountImageNullPathTest()
        {
            string imageFilePath = null;
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountImage(imageFilePath);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdateAccountDeliveryDeviceTest()
        {
            DeviceType device = new DeviceType();
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountDeliveryDevice(device);

            Assert.AreEqual(expected.Element("name").Value, actual.Name);
        }

        [TestMethod]
        public void UpdateAccountColorsTest()
        {
            string background = "9ae4e8";
            string text = "#000000";
            string link = "#0000ff";
            string sidebarFill = "#e0ff92";
            string sidebarBorder = "#87bc44";
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountColors(background, text, link, sidebarFill, sidebarBorder);

            Assert.AreEqual(expected.Element("name").Value, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountColorsNoInputTest()
        {
            string background = null;
            string text = null;
            string link = null;
            string sidebarFill = null;
            string sidebarBorder = null;
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountColors(background, text, link, sidebarFill, sidebarBorder);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdateAccountBackgroundImageTest()
        {
            string imageFilePath = "C:\\image.png";
            bool tile = false;
            bool use = false;
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostTwitterFile<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountBackgroundImage(imageFilePath, tile, use);

            Assert.AreEqual(expected.Element("name").Value, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountBackgroundImageNullPathTest()
        {
            string imageFilePath = string.Empty;
            bool tile = false;
            bool use = false;
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostTwitterImage<User>(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountBackgroundImage(imageFilePath, tile, use);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EndAccountSessionTest()
        {
            XElement expected = XElement.Parse(m_testEndSessionResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Account>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Account>>()))
                .Returns(m_testEndSessionResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            TwitterHashResponse actual = ctx.EndAccountSession();

            Assert.AreEqual(expected.Element("error").Value, actual.Error);
        }

        [TestMethod]
        public void EnableNotificationsTest()
        {
            string id = "1";
            string userID = "2";
            string screenName = "JoeMayo";
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.EnableNotifications(id, userID, screenName);

            Assert.AreEqual(expected.Element("name").Value, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnableNotificationsNoInputTest()
        {
            string id = null;
            string userID = null;
            string screenName = string.Empty;
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.EnableNotifications(id, userID, screenName);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DisableNotificationsTest()
        {
            string id = "1";
            string userID = "2";
            string screenName = "JoeMayo";
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.DisableNotifications(id, userID, screenName);

            Assert.AreEqual(expected.Element("name").Value, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DisableNotificationsNoInputTest()
        {
            string id = string.Empty;
            string userID = null;
            string screenName = string.Empty;
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.DisableNotifications(id, userID, screenName);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Direct Message Tests

        [TestMethod]
        public void NewDirectMessageTest()
        {
            string userID = "1";
            string text = "Hi";
            XElement expected = XElement.Parse(m_testDirectMessageQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<DirectMessage>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(m_testDirectMessageQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            DirectMessage actual = ctx.NewDirectMessage(userID, text);

            Assert.AreEqual(expected.Element("id").Value, actual.ID.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewDirectMessageNullMessageTest()
        {
            string userID = "1";
            string text = null;
            DirectMessage expected = new DirectMessage();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<DirectMessage>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(m_testDirectMessageQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            DirectMessage actual = ctx.NewDirectMessage(userID, text);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewDirectMessageNullUserIDTest()
        {
            string userID = string.Empty;
            string text = "Test Text";
            DirectMessage expected = new DirectMessage();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<DirectMessage>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(m_testDirectMessageQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            DirectMessage actual = ctx.NewDirectMessage(userID, text);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DestroyDirectMessageTest()
        {
            string id = "1";
            XElement expected = XElement.Parse(m_testDirectMessageQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<DirectMessage>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(m_testDirectMessageQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            DirectMessage actual = ctx.DestroyDirectMessage(id);

            Assert.AreEqual(expected.Element("id").Value, actual.ID.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DestroyDirectMessageNullIDTest()
        {
            string id = null;
            DirectMessage expected = new DirectMessage();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<DirectMessage>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<DirectMessage>>()))
                .Returns(m_testDirectMessageQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            DirectMessage actual = ctx.DestroyDirectMessage(id);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Help Tests

        [TestMethod]
        public void HelpTestTest()
        {
            string input = "<ok>False</ok>";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<bool>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<bool>>()))
                .Returns(input);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            bool actual = ctx.HelpTest();
            Assert.AreEqual(false, actual);
        }

        #endregion

        #region Friendship Tests

        [TestMethod]
        public void CreateFriendshipTest()
        {
            string id = "1";
            string userID = "2";
            string screenName = "JoeMayo";
            bool follow = false;
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.CreateFriendship(id, userID, screenName, follow);

            Assert.AreEqual(expected.Element("name").Value, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateFriendshipNoInputTest()
        {
            string id = string.Empty;
            string userID = string.Empty;
            string screenName = null;
            bool follow = false;
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.CreateFriendship(id, userID, screenName, follow);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DestroyFriendshipTest()
        {
            string id = "1";
            string userID = "2";
            string screenName = "JoeMayo";
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.DestroyFriendship(id, userID, screenName);

            Assert.AreEqual(expected.Element("name").Value, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DestroyFriendshipNoInputTest()
        {
            string id = null;
            string userID = string.Empty;
            string screenName = null;
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.DestroyFriendship(id, userID, screenName);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Favorites Tests

        [TestMethod]
        public void CreateFavoriteTest()
        {
            string id = "1";
            XElement expected = XElement.Parse(m_testStatusQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Status>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            Status actual = ctx.CreateFavorite(id);

            Assert.AreEqual(expected.Element("status").Element("id").Value, actual.StatusID);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateFavoriteNoIDTest()
        {
            string id = null;
            Status expected = new Status();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Status>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            Status actual = ctx.CreateFavorite(id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DestroyFavoriteTest()
        {
            string id = "1";
            XElement expected = XElement.Parse(m_testStatusQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Status>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            Status actual = ctx.DestroyFavorite(id);

            Assert.AreEqual(expected.Element("status").Element("id").Value, actual.StatusID);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DestroyFavoriteNullIDTest()
        {
            string id = string.Empty;
            Status expected = new Status();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<Status>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Status>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            Status actual = ctx.DestroyFavorite(id);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Block Tests

        [TestMethod]
        public void CreateBlockTest()
        {
            string id = "1";
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => 
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny <Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.CreateBlock(id);

            Assert.AreEqual(expected.Element("name").Value, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateBlockNoIDTest()
        {
            string id = string.Empty;
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.CreateBlock(id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DestroyBlockTest()
        {
            string id = "1";
            XElement expected = XElement.Parse(m_testUserQueryResponse);
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.DestroyBlock(id);

            Assert.AreEqual(expected.Element("name").Value, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DestroyBlockNullIDTest()
        {
            string id = string.Empty;
            User expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<User>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(m_testUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.DestroyBlock(id);

            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Raw Request Tests

        [TestMethod]
        public void ExecuteRawRequest_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            string queryString = "statuses/update.xml";
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            ctx.ExecuteRaw(queryString, parameters);

            execMock.Verify(exec => 
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/statuses/update.xml",
                    parameters, 
                    It.IsAny<IRequestProcessor<Raw>>()), 
                Times.Once());
        }

        [TestMethod]
        public void ExecuteRawRequest_Returns_Raw_Result()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            string queryString = "statuses/update.xml";
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };
            var expectedResult = "<status>xxx</status>";
            var fullUrl = "https://api.twitter.com/1/statuses/update.xml";
            execMock.Setup(exec => exec.ExecuteTwitter(fullUrl, parameters, It.IsAny<IRequestProcessor<Raw>>())).Returns(expectedResult);

            string actualResult = ctx.ExecuteRaw(queryString, parameters);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void ExecuteRawRequest_Resolves_Too_Many_Url_Slashes()
        {
            string baseUrlWithTrailingSlash = "https://api.twitter.com/1/";
            string queryStringWithBeginningSlash = "/statuses/update.xml";
            var fullUrl = "https://api.twitter.com/1/statuses/update.xml";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, baseUrlWithTrailingSlash, "");
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            ctx.ExecuteRaw(queryStringWithBeginningSlash, parameters);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    fullUrl,
                    parameters, 
                    It.IsAny<IRequestProcessor<Raw>>()), Times.Once());
        }

        [TestMethod]
        public void ExecuteRawRequest_Resolves_Too_Few_Url_Slashes()
        {
            string baseUrlWithoutTrailingSlash = "https://api.twitter.com/1";
            string queryStringWithoutBeginningSlash = "statuses/update.xml";
            var fullUrl = "https://api.twitter.com/1/statuses/update.xml";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, baseUrlWithoutTrailingSlash, "");
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };

            ctx.ExecuteRaw(queryStringWithoutBeginningSlash, parameters);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    fullUrl,
                    parameters, It.IsAny<IRequestProcessor<Raw>>()), Times.Once());
        }

        #endregion

        #region Streaming Tests

        [TestMethod]
        public void CreateRequestProcessor_Returns_StreamingRequestProcessor()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();
            ctx.StreamingUrl = "https://stream.twitter.com/1/";
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var streamingQuery =
                from tweet in ctx.Streaming
                where tweet.Type == StreamingType.Sample
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<Streaming>(streamingQuery.Expression);

            Assert.IsInstanceOfType(reqProc, typeof(StreamingRequestProcessor<Streaming>));
            Assert.AreEqual("https://stream.twitter.com/1/", reqProc.BaseUrl);
            Assert.AreEqual(execMock.Object, (reqProc as StreamingRequestProcessor<Streaming>).TwitterExecutor);
        }

        [TestMethod]
        public void Execute_Calls_QueryTwitterStream_For_Streaming_Queries()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var streamingQuery =
                from tweet in ctx.Streaming
                where tweet.Type == StreamingType.Sample
                select tweet;

            ctx.Execute<Streaming>(streamingQuery.Expression, isEnumerable: true);

            execMock.Verify(exec => exec.QueryTwitterStream(It.IsAny<Request>()), Times.Once());
        }

        [TestMethod]
        public void Execute_Calls_QueryTwitter_InsteadOf_QueryTwitterStream_For_NonStreaming_Queries()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => exec.QueryTwitter(It.IsAny<Request>(), It.IsAny<StatusRequestProcessor<Status>>())).Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var statusQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            ctx.Execute<Status>(statusQuery.Expression, isEnumerable: true);

            execMock.Verify(exec => exec.QueryTwitterStream(It.IsAny<Request>()), Times.Never());
            execMock.Verify(exec => exec.QueryTwitter(It.IsAny<Request>(), It.IsAny<StatusRequestProcessor<Status>>()), Times.Once());
        }

        [TestMethod]
        public void CreateRequestProcessor_Returns_UserStreamRequestProcessor()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();
            ctx.StreamingUrl = "https://userstream.twitter.com/2/";
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var streamingQuery =
                from tweet in ctx.UserStream
                where tweet.Type == UserStreamType.User
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<UserStream>(streamingQuery.Expression);

            Assert.IsInstanceOfType(reqProc, typeof(UserStreamRequestProcessor<UserStream>));
            Assert.AreEqual("https://userstream.twitter.com/2/", (reqProc as UserStreamRequestProcessor<UserStream>).UserStreamUrl);
            Assert.AreEqual(execMock.Object, (reqProc as UserStreamRequestProcessor<UserStream>).TwitterExecutor);
        }

        [TestMethod]
        public void Execute_Calls_QueryTwitterStream_For_UserStream_Queries()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var streamingQuery =
                from tweet in ctx.UserStream
                where tweet.Type == UserStreamType.User
                select tweet;

            ctx.Execute<UserStream>(streamingQuery.Expression, isEnumerable: true);

            execMock.Verify(exec => exec.QueryTwitterStream(It.IsAny<Request>()), Times.Once());
        }

        #endregion

        #region List Tests

        [TestMethod]
        public void CreateList_Requires_ListName()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.CreateList(null, "public", "desc");

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("listName", ae.ParamName);
            }
        }

        [TestMethod]
        public void CreateList_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var parameters = new Dictionary<string, string>
            {
                { "name", "test" },
                { "mode", "public" },
                { "description", "desc" }
            };

            ctx.CreateList("test", "public", "desc");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/create.xml",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [TestMethod]
        public void UpdateList_Requires_ListID_Or_Slug()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.UpdateList(null, null, null, null, null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void UpdateList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.UpdateList("123", "test", null, null, null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void UpdateList_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" },
                { "mode", "public" },
                { "description", "desc" }
            };

            ctx.UpdateList("123", "test", "456", "JoeMayo", "public", "desc");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/update.xml",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [TestMethod]
        public void DeleteList_Requires_ListID_Or_Slug()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.DeleteList(null, null, null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("listIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void DeleteList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.DeleteList("123", "test", null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void DeleteList_Works_With_Slug_And_OwnerID()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            ctx.DeleteList(null, "test", "456", null);
        }

        [TestMethod]
        public void DeleteList_Works_With_ListID_Only()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            ctx.DeleteList("123", null, null, null);
        }

        [TestMethod]
        public void DeleteList_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.DeleteList("123", "test", "456", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/destroy.xml",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [TestMethod]
        public void AddMemberToList_Requires_UserID_Or_ScreenName()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.AddMemberToList(null, null, null, null, null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("UserIdOrScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberToList_Requires_ListID_Or_Slug()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.AddMemberToList(null, "JoeMayo", null, null, null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberToList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.AddMemberToList(null, "JoeMayo", null, "linq", null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberToList_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var parameters = new Dictionary<string, string>
            {
                { "user_id", "789" },
                { "screen_name", "JoeMayo" },
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.AddMemberToList("789", "JoeMayo", "123", "test", "456", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/create.xml",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [TestMethod]
        public void AddMemberRangeToList_For_ScreenNames_Requires_ListID_Or_Slug()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.AddMemberRangeToList(null, null, null, null, new List<string> { "SomeName" });

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberRangeToList_For_ScreenNames_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.AddMemberRangeToList(null, "test", null, null, new List<string> { "SomeOne" });

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberRangeToList_For_ScreenNames_Requires_ScreenNames()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.AddMemberRangeToList(null, "test", "123", null, (List<string>)null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("screenNames", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberRangeToList_For_ScreenNames_Requires_ScreenNames_With_Values()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.AddMemberRangeToList(null, "test", "123", null, new List<string>());

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("screenNames", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberRangeToList_For_ScreenNames_Requires_ScreenNames_Count_LessThanOrEqualTo_100()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var screenNames = Enumerable.Range(1, 101).Select(item => item.ToString()).ToList();

            try
            {
                ctx.AddMemberRangeToList(null, "test", "123", null, screenNames);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("screenNames", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberRangeToList_For_ScreenNames_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" },
                { "user_id", null },
                { "screen_name", "JoeMayo,Linq2Tweeter,SomeOneElse" },
            };
            var screenNames = new List<string> { "JoeMayo", "Linq2Tweeter", "SomeOneElse" };

            ctx.AddMemberRangeToList("123", "test", "456", "JoeMayo", screenNames);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/create_all.xml",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [TestMethod]
        public void AddMemberRangeToList_For_UserIDs_Requires_UserIDs()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.AddMemberRangeToList(null, "test", "123", null, (List<ulong>)null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("userIDs", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberRangeToList_For_UserIDs_Requires_UserIDs_With_Values()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.AddMemberRangeToList(null, "test", "123", null, new List<ulong>());

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("userIDs", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberRangeToList_For_UserIDs_Requires_UserIDs_Count_LessThanOrEqualTo_100()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var userIDs = Enumerable.Range(1, 101).Select(item => (ulong)item).ToList();

            try
            {
                ctx.AddMemberRangeToList(null, "test", "123", null, userIDs);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("userIDs", ae.ParamName);
            }
        }

        [TestMethod]
        public void AddMemberRangeToList_For_UserIDs_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" },
                { "user_id", "123,234,345" },
                { "screen_name", null },
            };
            var userIDs = new List<ulong> { 123ul, 234ul, 345ul };

            ctx.AddMemberRangeToList("123", "test", "456", "JoeMayo", userIDs);

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/create_all.xml",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [TestMethod]
        public void DeleteMemberFromList_Requires_UserID_Or_ScreenName()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.DeleteMemberFromList(null, null, null, null, null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("UserIdOrScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void DeleteMemberFromList_Requires_ListID_Or_Slug()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.DeleteMemberFromList(null, "JoeMayo", null, null, null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void DeleteMemberFromList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.DeleteMemberFromList(null, "JoeMayo", null, "linq", null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void DeleteMemberFromList_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var parameters = new Dictionary<string, string>
            {
                { "user_id", "789" },
                { "screen_name", "JoeMayo" },
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.DeleteMemberFromList("789", "JoeMayo", "123", "test", "456", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/members/destroy.xml",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [TestMethod]
        public void SubscribeToList_Requires_ListID_Or_Slug()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.SubscribeToList(null, null, null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void SubscribeToList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.SubscribeToList(null, "linq", null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void SubscribeToList_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.SubscribeToList("123", "test", "456", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/subscribers/create.xml",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        [TestMethod]
        public void UnsubscribeFromList_Requires_ListID_Or_Slug()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.UnsubscribeFromList(null, null, null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("ListIdOrSlug", ae.ParamName);
            }
        }

        [TestMethod]
        public void UnsubscribeFromList_Requires_OwnerID_Or_OwnerScreenName_If_Slug_Used()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            try
            {
                ctx.UnsubscribeFromList(null, "linq", null, null);

                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException ae)
            {
                Assert.AreEqual("OwnerIdOrOwnerScreenName", ae.ParamName);
            }
        }

        [TestMethod]
        public void UnsubscribeFromList_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter<List>(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<List>>()))
                .Returns(m_testStatusQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var parameters = new Dictionary<string, string>
            {
                { "list_id", "123" },
                { "slug", "test" },
                { "owner_id", "456" },
                { "owner_screen_name", "JoeMayo" }
            };

            ctx.UnsubscribeFromList("123", "test", "456", "JoeMayo");

            execMock.Verify(exec =>
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/lists/subscribers/destroy.xml",
                    parameters,
                    It.IsAny<IRequestProcessor<List>>()),
                Times.Once());
        }

        #endregion

        #endregion
    }
}
