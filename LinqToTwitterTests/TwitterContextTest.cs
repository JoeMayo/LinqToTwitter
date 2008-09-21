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
        ///BuildUrl with null parameters
        ///</summary>
        public void BuildUrlNullParamsHelper<T>()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();
            Dictionary<string, string> parameters = null;
            string expected = "http://twitter.com/statuses/public_timeline.xml";
            string actual;
            actual = ctx.BuildUrl<Status>(parameters);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildUrlNullParams()
        {
            BuildUrlNullParamsHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///BuildUrl with Public parameters
        ///</summary>
        public void BuildUrlPublicParamsHelper<T>()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", "Public" }
                };
            string expected = "http://twitter.com/statuses/public_timeline.xml";
            string actual;
            actual = ctx.BuildUrl<Status>(parameters);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildUrlPublicParams()
        {
            BuildUrlPublicParamsHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///BuildUrl with Friends parameters
        ///</summary>
        public void BuildUrlFriendsParamsHelper<T>()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", "Friends" }
                };
            string expected = "http://twitter.com/statuses/friends_timeline.xml";
            string actual;
            actual = ctx.BuildUrl<Status>(parameters);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildUrlFriendsParams()
        {
            BuildUrlFriendsParamsHelper<GenericParameterHelper>();
        }

        /// <summary>
        ///BuildUrl with unknown type
        ///</summary>
        public void BuildUrlUnknownTypeParamsHelper<T>()
        {
            TwitterContext_Accessor ctx = new TwitterContext_Accessor();
            Dictionary<string, string> parameters =
                new Dictionary<string, string>
                {
                    { "Type", "Friends" }
                };
            string expected = "http://twitter.com/statuses/public_timeline.xml";
            string actual;
            actual = ctx.BuildUrl<T>(parameters);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        [DeploymentItem("LinqToTwitter.dll")]
        public void BuildUnknownTypeParams()
        {
            BuildUrlUnknownTypeParamsHelper<GenericParameterHelper>();
        }
    }
}
