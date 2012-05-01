using System.Globalization;
using System.Linq.Expressions;
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
        const string TestUserQueryResponse = @"<user>
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

        const string TestStatusQueryResponse = @"<statuses type=""array"">
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

        const string TestDirectMessageQueryResponse = @"
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

        const string TestEndSessionResponse = @"{
  ""request"": ""/1/account/end_session.json"",
  ""error"": ""Logged out.""
}";

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void TwitterContext_Single_Param_Constructor_Sets_Defaults()
        {
            const string baseUrl = "https://api.twitter.com/1/";
            const string searchUrl = "https://search.twitter.com/";
            ITwitterAuthorizer authorizedClient = new PinAuthorizer();
            var ctx = new TwitterContext(authorizedClient);

            Assert.AreSame(authorizedClient, ctx.AuthorizedClient);
            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        [TestMethod]
        public void TwitterContext_Three_Param_Constructor_Sets_Defaults()
        {
            ITwitterExecute execute = new TwitterExecute(new PinAuthorizer());
            const string baseUrl = "http://api.twitter.com/1/";
            const string searchUrl = "http://search.twitter.com/";
            var ctx = new TwitterContext(execute, baseUrl, searchUrl);

            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        [TestMethod]
        public void TwitterContext_No_Param_Works_With_Object_Initialization()
        {
            const string baseUrl = "http://api.twitter.com/1/";
            const string searchUrl = "http://search.twitter.com/";
            var ctx =
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
                new TwitterContext((PinAuthorizer)null);

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

        [TestMethod]
        public void CreateRequestProcessor_Returns_ProperRequestProcessor()
        {
            var ctx = new TwitterContext();

            var showQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Show
                select tweet;

            var statusProc = ctx.CreateRequestProcessor<Status>(showQuery.Expression);
            Assert.IsInstanceOfType(statusProc, typeof(StatusRequestProcessor<Status>));
        }

        [TestMethod]
        public void CreateStatusRequestProcessorTest()
        {
            var ctx = new TwitterContext();

            var queryResult = from tweet in ctx.Status select tweet;

            IRequestProcessor<Status> actual = ctx.CreateRequestProcessor<Status>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(StatusRequestProcessor<Status>));
        }

        [TestMethod]
        public void CreateAccountRequestProcessorTest()
        {
            var ctx = new TwitterContext();

            var queryResult = from tweet in ctx.Account select tweet;

            IRequestProcessor<Account> actual = ctx.CreateRequestProcessor<Account>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(AccountRequestProcessor<Account>));
        }

        [TestMethod]
        public void CreateBlocksRequestProcessorTest()
        {
            var ctx = new TwitterContext();

            var queryResult = from tweet in ctx.Blocks select tweet;

            IRequestProcessor<Blocks> actual = ctx.CreateRequestProcessor<Blocks>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(BlocksRequestProcessor<Blocks>));
        }

        [TestMethod]
        public void CreateDirectMessageRequestProcessorTest()
        {
            var ctx = new TwitterContext();

            var queryResult = from tweet in ctx.DirectMessage select tweet;

            IRequestProcessor<DirectMessage> actual = ctx.CreateRequestProcessor<DirectMessage>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(DirectMessageRequestProcessor<DirectMessage>));
        }

        [TestMethod]
        public void CreateFavoritesRequestProcessorTest()
        {
            var ctx = new TwitterContext();

            var queryResult = from tweet in ctx.Favorites select tweet;

            IRequestProcessor<Favorites> actual = ctx.CreateRequestProcessor<Favorites>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(FavoritesRequestProcessor<Favorites>));
        }

        [TestMethod]
        public void CreateFriendshipRequestProcessorTest()
        {
            var ctx = new TwitterContext();

            var queryResult = from tweet in ctx.Friendship select tweet;

            IRequestProcessor<Friendship> actual = ctx.CreateRequestProcessor<Friendship>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(FriendshipRequestProcessor<Friendship>));
        }

        [TestMethod]
        public void CreateSearchRequestProcessor_Returns_RawRequestProcessor()
        {
            var ctx = new TwitterContext();

            var queryResult = from raw in ctx.RawQuery select raw;

            IRequestProcessor<Raw> actual = ctx.CreateRequestProcessor<Raw>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(RawRequestProcessor<Raw>));
        }

        [TestMethod]
        public void CreateSearchRequestProcessorTest()
        {
            var ctx = new TwitterContext();

            var queryResult = from tweet in ctx.Search select tweet;

            IRequestProcessor<Search> actual = ctx.CreateRequestProcessor<Search>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(SearchRequestProcessor<Search>));
        }

        [TestMethod]
        public void CreateSocialGraphRequestProcessorTest()
        {
            var ctx = new TwitterContext();

            var queryResult = from tweet in ctx.SocialGraph select tweet;

            IRequestProcessor<SocialGraph> actual = ctx.CreateRequestProcessor<SocialGraph>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(SocialGraphRequestProcessor<SocialGraph>));
        }

        [TestMethod]
        public void CreateTrendRequestProcessorTest()
        {
            var ctx = new TwitterContext();

            var queryResult = from tweet in ctx.Trends select tweet;

            IRequestProcessor<Trend> actual = ctx.CreateRequestProcessor<Trend>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(TrendRequestProcessor<Trend>));
        }

        [TestMethod]
        public void CreateUserRequestProcessorTest()
        {
            var ctx = new TwitterContext();

            var queryResult = from tweet in ctx.User select tweet;

            IRequestProcessor<User> actual = ctx.CreateRequestProcessor<User>(queryResult.Expression);
            Assert.IsInstanceOfType(actual, typeof(UserRequestProcessor<User>));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateRequestProcessorNullExpressionTest1()
        {
            var ctx = new TwitterContext();

            ctx.CreateRequestProcessor<Status>((Expression)null);
        }

        [TestMethod]
        public void CreateRequestProcessor_Returns_LegalRequestProcessor()
        {
            var ctx = new TwitterContext {BaseUrl = "https://stream.twitter.com/1/"};
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var legalQuery =
                from tweet in ctx.Legal
                where tweet.Type == LegalType.Privacy
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<Legal>(legalQuery.Expression);

            Assert.IsInstanceOfType(reqProc, typeof(LegalRequestProcessor<Legal>));
            Assert.AreEqual("https://stream.twitter.com/1/", reqProc.BaseUrl);
        }

        [TestMethod]
        public void CreateRequestProcessor_Returns_RelatedResultsRequestProcessor()
        {
            var ctx = new TwitterContext {BaseUrl = "https://api.twitter.com/1/"};
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var resultsQuery =
                from tweet in ctx.RelatedResults
                where tweet.Type == RelatedResultsType.Show
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<RelatedResults>(resultsQuery.Expression);

            Assert.IsInstanceOfType(reqProc, typeof(RelatedResultsRequestProcessor<RelatedResults>));
            Assert.AreEqual("https://api.twitter.com/1/", reqProc.BaseUrl);
        }

        void InitializeTwitterContextForExecuteTest(out TwitterContext ctx, out Expression expression)
        {
            var exec = new Mock<ITwitterExecute>();
            exec.Setup(exc => exc.QueryTwitter(It.IsAny<Request>(), It.IsAny<IRequestProcessor<Status>>()))
                .Returns(SingleStatusResponse);

            ctx = new TwitterContext(exec.Object);
            var publicQuery = 
                from tweet in ctx.Status
                where tweet.Type == StatusType.Show
                select tweet;

            expression = publicQuery.Expression;
        }

        [TestMethod]
        public void Execute_Returns_List_Of_Status()
        {
            TwitterContext ctx;
            Expression expression;
            InitializeTwitterContextForExecuteTest(out ctx, out expression);
            ctx.Log = new DebuggerWriter();

            var actual = ctx.Execute<Status>(expression, true);

            var tweets = actual as IEnumerable<Status>;
            Assert.IsNotNull(tweets);
            Assert.IsTrue(tweets.Any());
        }

        [TestMethod]
        public void Execute_Logs_Results()
        {
            TwitterContext ctx;
            Expression expression;
            InitializeTwitterContextForExecuteTest(out ctx, out expression);

            var actual = ctx.Execute<Status>(expression, true);

            var tweets = actual as IEnumerable<Status>;
            Assert.IsNotNull(tweets);
            Assert.IsTrue(tweets.Any());
        }
  
        [TestMethod]
        public void Execute_Sets_RawResults_Property()
        {
            TwitterContext ctx;
            Expression expression;
            InitializeTwitterContextForExecuteTest(out ctx, out expression);

            ctx.Execute<Status>(expression, true);

            Assert.AreEqual(SingleStatusResponse, ctx.RawResult);
        }

        [TestMethod]
        public void UpdateAccountProfileTest()
        {
            const string name = "Joe";
            const string url = "http://www.csharp-station.com";
            const string location = "Denver, CO";
            const string description = "Open source developer for LINQ to Twitter";
            XElement expected = XElement.Parse(TestUserQueryResponse);
            string expectedName = null;
            var xElement = expected.Element("name");
            if (xElement != null) expectedName = xElement.Value;
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(
                exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expectedName, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileNullInputTest()
        {
            string name = string.Empty;
            string url = string.Empty;
            string location = string.Empty;
            string description = string.Empty;
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileNameOver20Test()
        {
            var name = new string(Enumerable.Repeat('x', 21).ToArray());
            const string url = "http://www.csharp-station.com";
            const string location = "Denver, CO";
            const string description = "Open source developer for LINQ to Twitter";
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileUrlOver100Test()
        {
            const string name = "Joe";
            var url = new string(Enumerable.Repeat('x', 101).ToArray());
            const string location = "Denver, CO";
            const string description = "Open source developer for LINQ to Twitter";
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileLocationOver30Test()
        {
            const string name = "Joe";
            const string url = "http://www.csharp-station.com";
            var location = new string(Enumerable.Repeat('x', 31).ToArray());
            const string description = "Open source developer for LINQ to Twitter";
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountProfileDescriptionOver160Test()
        {
            const string name = "Joe";
            const string url = "http://www.csharp-station.com";
            const string location = "Denver, CO";
            var description = new string(Enumerable.Repeat('x', 161).ToArray());
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountProfile(name, url, location, description);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdateAccountImageTest()
        {
            const string imageFilePath = "c:\\image.jpg";
            XElement expected = XElement.Parse(TestUserQueryResponse);
            string expectedName = null;
            var xElement = expected.Element("name");
            if (xElement != null) expectedName = xElement.Value;
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostTwitterFile(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountImage(imageFilePath);

            Assert.AreEqual(expectedName, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountImageNullPathTest()
        {
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountImage(null);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdateAccountColorsTest()
        {
            const string background = "9ae4e8";
            const string text = "#000000";
            const string link = "#0000ff";
            const string sidebarFill = "#e0ff92";
            const string sidebarBorder = "#87bc44";
            XElement expected = XElement.Parse(TestUserQueryResponse);
            string expectedName = null;
            var xElement = expected.Element("name");
            if (xElement != null) expectedName = xElement.Value;
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountColors(background, text, link, sidebarFill, sidebarBorder);

            Assert.AreEqual(expectedName, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountColorsNoInputTest()
        {
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountColors(null, null, null, null, null);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void UpdateAccountBackgroundImageTest()
        {
            const string imageFilePath = "C:\\image.png";
            const bool tile = false;
            const bool use = false;
            XElement expected = XElement.Parse(TestUserQueryResponse);
            string expectedName = null;
            var xElement = expected.Element("name");
            if (xElement != null) expectedName = xElement.Value;
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostTwitterFile(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountBackgroundImage(imageFilePath, tile, use);

            Assert.AreEqual(expectedName, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UpdateAccountBackgroundImageNullPathTest()
        {
            string imageFilePath = string.Empty;
            const bool tile = false;
            const bool use = false;
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.PostTwitterImage(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, string>>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.UpdateAccountBackgroundImage(imageFilePath, tile, use);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EndAccountSessionTest()
        {
            const string expectedErrorResponse = "Logged out.";
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<Account>>()))
                .Returns(TestEndSessionResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            TwitterHashResponse actual = ctx.EndAccountSession();

            Assert.AreEqual(expectedErrorResponse, actual.Error);
        }

        [TestMethod]
        public void EnableNotificationsTest()
        {
            const string id = "1";
            const string userID = "2";
            const string screenName = "JoeMayo";
            XElement expected = XElement.Parse(TestUserQueryResponse);
            string expectedName = null;
            var xElement = expected.Element("name");
            if (xElement != null) expectedName = xElement.Value;
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.EnableNotifications(id, userID, screenName);

            Assert.AreEqual(expectedName, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EnableNotificationsNoInputTest()
        {
            string screenName = string.Empty;
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.EnableNotifications(null, null, screenName);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DisableNotificationsTest()
        {
            const string id = "1";
            const string userID = "2";
            const string screenName = "JoeMayo";
            XElement expected = XElement.Parse(TestUserQueryResponse);
            string expectedName = null;
            var xElement = expected.Element("name");
            if (xElement != null) expectedName = xElement.Value;
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.DisableNotifications(id, userID, screenName);

            Assert.AreEqual(expectedName, actual.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DisableNotificationsNoInputTest()
        {
            string id = string.Empty;
            string screenName = string.Empty;
            var expected = new User();
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec =>
                exec.ExecuteTwitter(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>>(),
                    It.IsAny<IRequestProcessor<User>>()))
                .Returns(TestUserQueryResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");

            User actual = ctx.DisableNotifications(id, null, screenName);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ExecuteRawRequest_Invokes_Executor_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            const string queryString = "statuses/update.xml";
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
            const string queryString = "statuses/update.xml";
            var parameters = new Dictionary<string, string>
            {
                { "status", "Testing" }
            };
            const string expectedResult = "<status>xxx</status>";
            const string fullUrl = "https://api.twitter.com/1/statuses/update.xml";
            execMock.Setup(exec => exec.ExecuteTwitter(fullUrl, parameters, It.IsAny<IRequestProcessor<Raw>>())).Returns(expectedResult);

            string actualResult = ctx.ExecuteRaw(queryString, parameters);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        public void ExecuteRawRequest_Resolves_Too_Many_Url_Slashes()
        {
            const string baseUrlWithTrailingSlash = "https://api.twitter.com/1/";
            const string queryStringWithBeginningSlash = "/statuses/update.xml";
            const string fullUrl = "https://api.twitter.com/1/statuses/update.xml";
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
            const string baseUrlWithoutTrailingSlash = "https://api.twitter.com/1";
            const string queryStringWithoutBeginningSlash = "statuses/update.xml";
            const string fullUrl = "https://api.twitter.com/1/statuses/update.xml";
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

        [TestMethod]
        public void CreateRequestProcessor_Returns_StreamingRequestProcessor()
        {
            var ctx = new TwitterContext {StreamingUrl = "https://stream.twitter.com/1/"};
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var streamingQuery =
                from tweet in ctx.Streaming
                where tweet.Type == StreamingType.Sample
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<Streaming>(streamingQuery.Expression);

            Assert.IsInstanceOfType(reqProc, typeof(StreamingRequestProcessor<Streaming>));
            Assert.AreEqual("https://stream.twitter.com/1/", reqProc.BaseUrl);
            var streamingRequestProcessor = reqProc as StreamingRequestProcessor<Streaming>;
            if (streamingRequestProcessor != null)
                Assert.AreEqual(execMock.Object, streamingRequestProcessor.TwitterExecutor);
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
            execMock.Setup(exec => exec.QueryTwitter(It.IsAny<Request>(), It.IsAny<StatusRequestProcessor<Status>>())).Returns(SingleStatusResponse);
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "", "");
            var statusQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Show
                select tweet;

            ctx.Execute<Status>(statusQuery.Expression, isEnumerable: true);

            execMock.Verify(exec => exec.QueryTwitterStream(It.IsAny<Request>()), Times.Never());
            execMock.Verify(exec => exec.QueryTwitter(It.IsAny<Request>(), It.IsAny<StatusRequestProcessor<Status>>()), Times.Once());
        }

        [TestMethod]
        public void CreateRequestProcessor_Returns_UserStreamRequestProcessor()
        {
            var ctx = new TwitterContext {StreamingUrl = "https://userstream.twitter.com/2/"};
            var execMock = new Mock<ITwitterExecute>();
            ctx.TwitterExecutor = execMock.Object;
            var streamingQuery =
                from tweet in ctx.UserStream
                where tweet.Type == UserStreamType.User
                select tweet;

            var reqProc = ctx.CreateRequestProcessor<UserStream>(streamingQuery.Expression);

            Assert.IsInstanceOfType(reqProc, typeof(UserStreamRequestProcessor<UserStream>));
            var userStreamRequestProcessor = reqProc as UserStreamRequestProcessor<UserStream>;
            if (userStreamRequestProcessor != null)
                Assert.AreEqual("https://userstream.twitter.com/2/", userStreamRequestProcessor.UserStreamUrl);
            var streamRequestProcessor = reqProc as UserStreamRequestProcessor<UserStream>;
            if (streamRequestProcessor != null)
                Assert.AreEqual(execMock.Object, streamRequestProcessor.TwitterExecutor);
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

        const string SingleStatusResponse = @"{
      ""retweeted"":false,
      ""in_reply_to_screen_name"":null,
      ""possibly_sensitive"":false,
      ""retweeted_status"":{
         ""retweeted"":false,
         ""in_reply_to_screen_name"":null,
         ""possibly_sensitive"":false,
         ""contributors"":null,
         ""coordinates"":null,
         ""place"":null,
         ""user"":{
            ""id"":41754227,
            ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/565139568\/redshirt_normal.jpg"",
            ""url"":""http:\/\/weblogs.asp.net\/scottgu"",
            ""created_at"":""Fri May 22 04:39:35 +0000 2009"",
            ""followers_count"":57222,
            ""default_profile"":true,
            ""profile_background_color"":""C0DEED"",
            ""lang"":""en"",
            ""utc_offset"":-28800,
            ""name"":""Scott Guthrie"",
            ""profile_background_image_url"":""http:\/\/a0.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""location"":""Redmond, WA"",
            ""profile_link_color"":""0084B4"",
            ""listed_count"":4390,
            ""verified"":false,
            ""protected"":false,
            ""profile_use_background_image"":true,
            ""is_translator"":false,
            ""following"":false,
            ""description"":""I live in Seattle and build a few products for Microsoft"",
            ""profile_text_color"":""333333"",
            ""statuses_count"":3054,
            ""screen_name"":""scottgu"",
            ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/565139568\/redshirt_normal.jpg"",
            ""time_zone"":""Pacific Time (US & Canada)"",
            ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/images\/themes\/theme1\/bg.png"",
            ""friends_count"":86,
            ""default_profile_image"":false,
            ""contributors_enabled"":false,
            ""profile_sidebar_border_color"":""C0DEED"",
            ""id_str"":""41754227"",
            ""geo_enabled"":false,
            ""favourites_count"":44,
            ""profile_background_tile"":false,
            ""notifications"":false,
            ""show_all_inline_media"":false,
            ""profile_sidebar_fill_color"":""DDEEF6"",
            ""follow_request_sent"":false
         },
         ""retweet_count"":393,
         ""id_str"":""184793217231880192"",
         ""in_reply_to_user_id"":null,
         ""favorited"":false,
         ""in_reply_to_status_id_str"":null,
         ""in_reply_to_status_id"":null,
         ""source"":""web"",
         ""created_at"":""Wed Mar 28 00:05:10 +0000 2012"",
         ""in_reply_to_user_id_str"":null,
         ""truncated"":false,
         ""id"":184793217231880192,
         ""geo"":null,
         ""text"":""I just blogged about http:\/\/t.co\/YWHGwOq6 MVC, Web API, Razor and Open Source - Now with Contributions: http:\/\/t.co\/qpevLMZd""
      },
      ""contributors"":null,
      ""coordinates"":null,
      ""place"":null,
      ""user"":{
         ""id"":15411837,
         ""profile_image_url"":""http:\/\/a0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""url"":""http:\/\/www.mayosoftware.com"",
         ""created_at"":""Sun Jul 13 04:35:50 +0000 2008"",
         ""followers_count"":1102,
         ""default_profile"":false,
         ""profile_background_color"":""0099B9"",
         ""lang"":""en"",
         ""utc_offset"":-25200,
         ""name"":""Joe Mayo"",
         ""profile_background_image_url"":""http:\/\/a0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""location"":""Denver, CO"",
         ""profile_link_color"":""0099B9"",
         ""listed_count"":112,
         ""verified"":false,
         ""protected"":false,
         ""profile_use_background_image"":true,
         ""is_translator"":false,
         ""following"":true,
         ""description"":""Independent .NET Consultant; author of 6 books; Microsoft Visual C# MVP"",
         ""profile_text_color"":""3C3940"",
         ""statuses_count"":1906,
         ""screen_name"":""JoeMayo"",
         ""profile_image_url_https"":""https:\/\/si0.twimg.com\/profile_images\/1728197892\/n536783050_1693444_2739826_normal.jpg"",
         ""time_zone"":""Mountain Time (US & Canada)"",
         ""profile_background_image_url_https"":""https:\/\/si0.twimg.com\/profile_background_images\/13330711\/200xColor_2.png"",
         ""friends_count"":211,
         ""default_profile_image"":false,
         ""contributors_enabled"":false,
         ""profile_sidebar_border_color"":""5ED4DC"",
         ""id_str"":""15411837"",
         ""geo_enabled"":true,
         ""favourites_count"":44,
         ""profile_background_tile"":false,
         ""notifications"":true,
         ""show_all_inline_media"":false,
         ""profile_sidebar_fill_color"":""95E8EC"",
         ""follow_request_sent"":false
      },
      ""retweet_count"":393,
      ""id_str"":""184835136037191681"",
      ""in_reply_to_user_id"":null,
      ""favorited"":false,
      ""in_reply_to_status_id_str"":null,
      ""in_reply_to_status_id"":null,
      ""source"":""web"",
      ""created_at"":""Wed Mar 28 02:51:45 +0000 2012"",
      ""in_reply_to_user_id_str"":null,
      ""truncated"":false,
      ""id"":184835136037191681,
      ""geo"":null,
      ""text"":""RT @scottgu: I just blogged about http:\/\/t.co\/YWHGwOq6 MVC, Web API, Razor and Open Source - Now with Contributions: http:\/\/t.co\/qpevLMZd""
   }";
    }
}
