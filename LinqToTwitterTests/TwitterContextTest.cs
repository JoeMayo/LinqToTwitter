using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for TwitterContextTest and is intended
    ///to contain all TwitterContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TwitterContextTest
    {
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
        ///1 param constructor defaults
        ///</summary>
        [TestMethod()]
        public void OneParamCtorDefaults()
        {
            string userName = string.Empty;
            string password = string.Empty;
            string baseUrl = "http://twitter.com/";
            string searchUrl = "http://search.twitter.com/";
            TwitterContext ctx = new TwitterContext();

            Assert.AreEqual(userName, ctx.UserName);
            Assert.AreEqual(password, ctx.Password);
            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        /// <summary>
        ///2 param constructor defaults
        ///</summary>
        [TestMethod()]
        public void TwoParamCtorDefaults()
        {
            string userName = "TestUser";
            string password = "TestPassword";
            string baseUrl = "http://twitter.com/";
            string searchUrl = "http://search.twitter.com/";
            TwitterContext ctx = new TwitterContext(userName, password);

            Assert.AreEqual(userName, ctx.UserName);
            Assert.AreEqual(password, ctx.Password);
            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        /// <summary>
        ///3 param constructor defaults
        ///</summary>
        [TestMethod()]
        public void ThreeParamCtorDefaults()
        {
            string userName = "TestUser";
            string password = "TestPassword";
            string baseUrl = "http://www.twitter.com/";
            string searchUrl = "http://search.twitter.com/";
            TwitterContext ctx = new TwitterContext(userName, password, baseUrl, searchUrl);

            Assert.AreEqual(userName, ctx.UserName);
            Assert.AreEqual(password, ctx.Password);
            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        /// <summary>
        ///3 param constructor defaults
        ///</summary>
        [TestMethod()]
        public void ObjectInitializerTest()
        {
            string userName = "TestUser";
            string password = "TestPassword";
            string baseUrl = "http://www.twitter.com/";
            string searchUrl = "http://search.twitter.com/";
            TwitterContext ctx =
                new TwitterContext
                {
                    UserName = userName,
                    Password = password,
                    BaseUrl = baseUrl
                };

            Assert.AreEqual(userName, ctx.UserName);
            Assert.AreEqual(password, ctx.Password);
            Assert.AreEqual(baseUrl, ctx.BaseUrl);
            Assert.AreEqual(searchUrl, ctx.SearchUrl);
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        public void CreateRequestProcessorTestHelper<T>()
        {
            TwitterContext ctx = new TwitterContext();

            var publicQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            var statusProc = ctx.CreateRequestProcessor(publicQuery.Expression);
            Assert.IsInstanceOfType(statusProc, typeof(StatusRequestProcessor));
        }

        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void CreateRequestProcessorTest()
        {
            CreateRequestProcessorTestHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void ExecuteTest()
        {
            var ctx = new TwitterContext();

            var publicQuery =
                from tweet in ctx.Status
                where tweet.Type == StatusType.Public
                select tweet;

            var actual = ctx.Execute(publicQuery.Expression);
            var tweets = (actual as IQueryable<Status>).ToList();
            Assert.IsNotNull(tweets);
            Assert.IsTrue(tweets.Count > 0);
        }

        /// <summary>
        ///A test for QueryTwitter
        ///</summary>
        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void QueryTwitterTest()
        {
            TwitterContext_Accessor target = new TwitterContext_Accessor();
            string url = "http://twitter.com/statuses/public_timeline.xml";
            IRequestProcessor requestProcessor = new StatusRequestProcessor();
            var twitterResponse = target.QueryTwitter(url, requestProcessor);

            var tweets = (twitterResponse as IQueryable<Status>).ToList();
            Assert.IsNotNull(tweets);
            Assert.IsTrue(tweets.Count > 0);
        }

        /// <summary>
        ///A test for GetAuthorizationPageLink
        ///</summary>
        [TestMethod()]
        public void GetAuthorizationPageLinkTest()
        {
            TwitterContext target = new TwitterContext();
            Console.Write("Consumer Key: ");
            target.ConsumerKey = "123";
            Console.Write("Consumer Secret: ");
            target.ConsumerSecret = "456";
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual = string.Empty;
            //actual = target.GetAuthorizationPageLink();
            Assert.AreEqual(expected, actual);
        }
    }
}
