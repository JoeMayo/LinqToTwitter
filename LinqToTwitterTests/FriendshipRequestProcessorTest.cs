using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for FriendshipRequestProcessorTest and is intended
    ///to contain all FriendshipRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FriendshipRequestProcessorTest
    {
        private string m_testQueryResponse = "<friends>true</friends>";

        private string m_testLookupResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<relationships>
    <relationship id=""783214"">
        <id>783214</id>
        <name>Twitter</name>
        <screen_name>twitter</screen_name>
        <connections>
            <connection>none</connection>
        </connections>
    </relationship>
        <relationship id=""15411837"">
        <id>15411837</id>
        <name>Joe Mayo</name>
        <screen_name>JoeMayo</screen_name>
        <connections>
            <connection>following</connection>
        </connections>
    </relationship>
</relationships>";

        private string m_testUpdateResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<relationship>
  <target>
    <followed_by type=""boolean"">true</followed_by>
    <screen_name>JoeMayo</screen_name>
    <following type=""boolean"">false</following>
    <id type=""integer"">15411837</id>
  </target>
  <source>
    <marked_spam type=""boolean"">false</marked_spam>
    <followed_by type=""boolean"">false</followed_by>
    <notifications_enabled type=""boolean"">true</notifications_enabled>
    <screen_name>LinqToTweeter</screen_name>
    <following type=""boolean"">true</following>
    <all_replies type=""boolean"">false</all_replies>
    <want_retweets type=""boolean"">true</want_retweets>
    <blocking type=""boolean"">false</blocking>
    <id type=""integer"">16761255</id>
  </source>
</relationship>";

        private string m_testNoRetweetIDsResponse = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<ids>
<id>15411837</id>
<id>241594327</id>
</ids>";

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
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>();
            bool expected = true;

            IList actual = target.ProcessResults(m_testQueryResponse);

            var isFriend = actual.Cast<Friendship>().First().IsFriend;
            Assert.AreEqual(expected, actual.Cast<Friendship>().First().IsFriend);
        }

        [TestMethod]
        public void ProcessResults_Translates_Relationships_From_LookupQuery()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>();

            List<Friendship> friends = friendReqProc.ProcessResults(m_testLookupResponse);

            var relations = friends.First().Relationships;
            Assert.AreEqual(2, relations.Count);
            Assert.AreEqual("none", relations[0].Connections[0]);
            Assert.AreEqual("following", relations[1].Connections[0]);
        }

        [TestMethod()]
        public void ProcessResults_Translates_Relationships_From_UpdateSettings()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>();

            List<Friendship> friends = friendReqProc.ProcessResults(m_testUpdateResponse);

            var srcRel = friends.First().SourceRelationship;
            Assert.AreEqual(true, srcRel.RetweetsWanted);
            Assert.AreEqual(true, srcRel.NotificationsEnabled);
        }

        [TestMethod()]
        public void ProcessResults_Translates_IDs_From_Response()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>();

            List<Friendship> friends = friendReqProc.ProcessResults(m_testNoRetweetIDsResponse);

            var ids = friends.First().IDInfo.IDs;
            Assert.AreEqual(2, ids.Count);
        }

        [TestMethod]
        public void ProcessResults_Returns_Empty_Collection_When_Empty_Results()
        {
            var reqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://api.twitter.com/1/" };

            var friendships = reqProc.ProcessResults(string.Empty);

            Assert.AreEqual(1, friendships.Count);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParameters_Returns_Parameters()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Expression<Func<Friendship, bool>> expression = 
                friend => 
                    friend.Type == FriendshipType.Exists &&
                    friend.SubjectUser == "123" && 
                    friend.FollowingUser == "456" &&
                    friend.SourceUserID == "1" &&
                    friend.SourceScreenName == "Name" &&
                    friend.TargetUserID == "2" &&
                    friend.TargetScreenName == "Name" &&
                    friend.Cursor == "-1" &&
                    friend.ScreenName == "twitter,joemayo" ;
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = target.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)FriendshipType.Exists).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SubjectUser", "123")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("FollowingUser", "456")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SourceUserID", "1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("SourceScreenName", "Name")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("TargetUserID", "2")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("TargetScreenName", "Name")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Cursor", "-1")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ScreenName", "twitter,joemayo")));
        }

        /// <summary>
        ///A test for BuildURL for the exists API
        ///</summary>
        [TestMethod()]
        public void BuildExistsURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Exists.ToString() },
                    { "SubjectUser", "123" },
                    { "FollowingUser", "456" }
                };
            string expected = "https://api.twitter.com/1/friendships/exists.xml?user_a=123&user_b=456";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL for the show function
        ///</summary>
        [TestMethod()]
        public void BuildShowURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "SourceUserID", "123" },
                    { "SourceScreenName", "JoeMayo" },
                    { "TargetUserID", "456" },
                    { "TargetScreenName", "LinqToTweeter" }
                };
            string expected = "https://api.twitter.com/1/friendships/show.xml?source_id=123&source_screen_name=JoeMayo&target_id=456&target_screen_name=LinqToTweeter";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL for the show function
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildShowWithoutSourceURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "TargetUserID", "456" },
                    { "TargetScreenName", "LinqToTweeter" }
                };
            
            target.BuildURL(parameters);
        }

        /// <summary>
        ///A test for BuildURL for the show function
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildShowWithoutTargetURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "SourceUserID", "123" },
                    { "SourceScreenName", "JoeMayo" },
                };
            
            target.BuildURL(parameters);
        }

        /// <summary>
        ///A test for BuildURL for the show function
        ///</summary>
        [TestMethod()]
        public void BuildUrl_Creates_No_Retweet_Url()
        {
            var target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://api.twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.NoRetweetIDs.ToString() }
                };
            string expected = "http://api.twitter.com/friendships/no_retweet_ids.xml";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
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
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
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

        /// <summary>
        ///A test for BuildURL for the incoming function
        ///</summary>
        [TestMethod()]
        public void BuildIncomingURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Incoming.ToString() }
                };
            string expected = "https://api.twitter.com/1/friendships/incoming.xml";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL for the incoming with cursor parameter function
        ///</summary>
        [TestMethod()]
        public void BuildIncomingWithCursorURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Incoming.ToString() },
                    { "Cursor", "-1" }
                };
            string expected = "https://api.twitter.com/1/friendships/incoming.xml?cursor=-1";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL for the outgoing function
        ///</summary>
        [TestMethod()]
        public void BuildOutgoingURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Outgoing.ToString() }
                };
            string expected = "https://api.twitter.com/1/friendships/outgoing.xml";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL for the outgoing with cursor parameter function
        ///</summary>
        [TestMethod()]
        public void BuildOutgoingWithCursorURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Outgoing.ToString() },
                    { "Cursor", "-1" }
                };
            string expected = "https://api.twitter.com/1/friendships/outgoing.xml?cursor=-1";

            Request req = target.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        /// <summary>
        ///A test for BuildURL for the Lookup function
        ///</summary>
        [TestMethod()]
        public void BuildLookupUrl_Returns_Url()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Lookup.ToString() },
                    { "ScreenName", "twitter,joemayo" }
                };
            string expected = "https://api.twitter.com/1/friendships/lookup.xml?screen_name=twitter%2Cjoemayo";

            Request req = friendReqProc.BuildURL(parameters);

            Assert.AreEqual(expected, req.FullUrl);
        }

        [TestMethod]
        public void BuildLookupUrl_Requires_ScreenName()
        {
            var friendReqProc = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "https://api.twitter.com/1/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Lookup.ToString() },
                    //{ "ScreenName", "twitter,joemayo" }
                };

            try
            {
                friendReqProc.BuildURL(parameters);

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("ScreenName", ane.ParamName);
            }
        }

        [TestMethod]
        public void UpdateFriendshipSettings_Calls_Execute()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => exec.ExecuteTwitter(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<IRequestProcessor<Friendship>>())).Returns("<friends>true</friends>");
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "https://api.twitter.com/1/", "");
            var reqProc = new Mock<IRequestProcessor<User>>();

            ctx.UpdateFriendshipSettings("joemayo", true, true);

            execMock.Verify(exec => 
                exec.ExecuteTwitter(
                    "https://api.twitter.com/1/friendships/update.xml",
                    It.IsAny<Dictionary<string, string>>(), 
                    It.IsAny<IRequestProcessor<Friendship>>()),
                Times.Once());
        }

        [TestMethod]
        public void UpdateFriendshipSettings_Requires_ScreenName()
        {
            var authMock = new Mock<ITwitterAuthorizer>();
            var execMock = new Mock<ITwitterExecute>();
            execMock.SetupGet(exec => exec.AuthorizedClient).Returns(authMock.Object);
            execMock.Setup(exec => exec.ExecuteTwitter(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<IRequestProcessor<Friendship>>())).Returns("<friends>true</friends>");
            var ctx = new TwitterContext(authMock.Object, execMock.Object, "https://api.twitter.com/1/", "");
            var reqProc = new Mock<IRequestProcessor<User>>();

            try
            {
                ctx.UpdateFriendshipSettings(/*"joemayo"*/ null, true, true);

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("screenName", ane.ParamName);
            }
        }
    }
}
