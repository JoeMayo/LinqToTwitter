using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;

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
            TwitterContext ctx = new TwitterContext();

            Assert.AreEqual(userName, ctx.UserName);
            Assert.AreEqual(password, ctx.Password);
            Assert.AreEqual(baseUrl, ctx.BaseUrl);
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
            TwitterContext ctx = new TwitterContext(userName, password);

            Assert.AreEqual(userName, ctx.UserName);
            Assert.AreEqual(password, ctx.Password);
            Assert.AreEqual(baseUrl, ctx.BaseUrl);
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
            TwitterContext ctx = new TwitterContext(userName, password, baseUrl);

            Assert.AreEqual(userName, ctx.UserName);
            Assert.AreEqual(password, ctx.Password);
            Assert.AreEqual(baseUrl, ctx.BaseUrl);
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
        }

        /// <summary>
        ///A test for CreateRequestProcessor
        ///</summary>
        public void CreateRequestProcessorTestHelper<T>()
        {
            TwitterContext ctx = new TwitterContext();
            IRequestProcessor statusProc;
            statusProc = ctx.CreateRequestProcessor<Status>();
            Assert.IsInstanceOfType(statusProc, typeof(StatusRequestProcessor));
        }

        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void CreateRequestProcessorTest()
        {
            CreateRequestProcessorTestHelper<GenericParameterHelper>();
        }
    }
}
