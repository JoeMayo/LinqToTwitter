using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for OAuthTwitterTest and is intended
    ///to contain all OAuthTwitterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OAuthTwitterTest
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
        ///A test for GetOAuthAuthorizationHeader
        ///</summary>
        [TestMethod()]
        public void GetOAuthAuthorizationHeaderTest()
        {
            OAuthTwitter target = new OAuthTwitter();
            target.OAuthConsumerKey = "5";
            target.OAuthConsumerSecret = "7";
            target.OAuthToken = "3";
            target.OAuthTokenSecret = "9";
            string url = "http://twitter.com";
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "first", "one" },
                { "second", "two" }
            };
            string expected = "OAuth realm=\"http://twitter.com/\"";
            string actual;
            actual = target.GetOAuthAuthorizationHeader(url, parameters);
            Assert.IsTrue(actual.StartsWith(expected));
        }
    }
}
