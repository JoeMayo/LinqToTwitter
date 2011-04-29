using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

        [TestMethod()]
        public void FilterRequestParameters_Splits_Url_Properly()
        {
            var oaTwit = new OAuthTwitter();
            Uri fullUrl = new Uri("http://www.mySite.com?oauth_token=123&p1=v1");
            
            string filteredUrl = oaTwit.FilterRequestParameters(fullUrl);

            Assert.AreEqual("http://www.mysite.com/?p1=v1", filteredUrl);
        }
    }
}
