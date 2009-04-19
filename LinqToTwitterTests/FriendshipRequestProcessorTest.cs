using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Linq;
using System;

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
        public void ProcessResultsTest()
        {
            FriendshipRequestProcessor target = new FriendshipRequestProcessor();
            XElement twitterResponse = XElement.Parse(m_testQueryResponse);
            bool expected = true;
            IQueryable actual;
            actual = target.ProcessResults(twitterResponse);
            var isFriend = actual.Cast<Friendship>().First().IsFriend;
            Assert.AreEqual(expected, actual.Cast<Friendship>().First().IsFriend);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            FriendshipRequestProcessor target = new FriendshipRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Func<Friendship, bool> func = friend => friend.SubjectUser == "123" && friend.FollowingUser == "456";
            Expression<Func<Friendship, bool>> expression = 
                friend => 
                    friend.Type == FriendshipType.Exists &&
                    friend.SubjectUser == "123" && 
                    friend.FollowingUser == "456";
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
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildURLTest()
        {
            FriendshipRequestProcessor target = new FriendshipRequestProcessor() { BaseUrl = "http://twitter.com/" };
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "SubjectUser", "123" },
                    { "FollowingUser", "456" }
                };
            string expected = "http://twitter.com/friendships/exists.xml?user_a=123&user_b=456";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }
    }
}
