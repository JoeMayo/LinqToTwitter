using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Linq;
using System;
using System.Collections;
using LinqToTwitterTests.Common;

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

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://twitter.com/" };
            Expression<Func<Friendship, bool>> expression = 
                friend => 
                    friend.Type == FriendshipType.Exists &&
                    friend.SubjectUser == "123" && 
                    friend.FollowingUser == "456" &&
                    friend.SourceUserID == "1" &&
                    friend.SourceScreenName == "Name" &&
                    friend.TargetUserID == "2" &&
                    friend.TargetScreenName == "Name" &&
                    friend.Cursor == "-1";
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
        }

        /// <summary>
        ///A test for BuildURL for the exists API
        ///</summary>
        [TestMethod()]
        public void BuildExistsURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Exists.ToString() },
                    { "SubjectUser", "123" },
                    { "FollowingUser", "456" }
                };
            string expected = "http://twitter.com/friendships/exists.xml?user_a=123&user_b=456";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL for the show function
        ///</summary>
        [TestMethod()]
        public void BuildShowURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "SourceUserID", "123" },
                    { "SourceScreenName", "JoeMayo" },
                    { "TargetUserID", "456" },
                    { "TargetScreenName", "LinqToTweeter" }
                };
            string expected = "http://twitter.com/friendships/show.xml?source_id=123&source_screen_name=JoeMayo&target_id=456&target_screen_name=LinqToTweeter";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL for the show function
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildShowWithoutSourceURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "TargetUserID", "456" },
                    { "TargetScreenName", "LinqToTweeter" }
                };
            string actual = target.BuildURL(parameters);
        }

        /// <summary>
        ///A test for BuildURL for the show function
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildShowWithoutTargetURLTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", FriendshipType.Show.ToString() },
                    { "SourceUserID", "123" },
                    { "SourceScreenName", "JoeMayo" },
                };
            string actual = target.BuildURL(parameters);
        }

        /// <summary>
        ///A test for missing type
        ///</summary>
        [TestMethod()]
        public void MissingTypeTest()
        {
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://twitter.com/" };
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
            FriendshipRequestProcessor<Friendship> target = new FriendshipRequestProcessor<Friendship>() { BaseUrl = "http://twitter.com/" };
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
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
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
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }
    }
}
