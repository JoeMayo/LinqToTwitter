using System;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterTests
{
    
    
    /// <summary>
    ///This is a test class for WebAuthorizerTest and is intended
    ///to contain all WebAuthorizerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SignInAuthorizerTest
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

        [TestMethod]
        public void BeginAuthorization_Sets_ForceLogin()
        {
            string requestUrl = "https://api.twitter.com/";
            var signInAuth = new SignInAuthorizer();
            signInAuth.Credentials = new InMemoryCredentials();
            var oAuthMock = new Mock<IOAuthTwitter>();
            signInAuth.OAuthTwitter = oAuthMock.Object;
            oAuthMock.Setup(oauth => oauth.FilterRequestParameters(It.IsAny<Uri>())).Returns(requestUrl);
            string authUrl = string.Empty;
            signInAuth.PerformRedirect = url => authUrl = url;

            signInAuth.BeginAuthorization(new Uri(requestUrl), forceLogin: true);

            oAuthMock.Verify(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), requestUrl, true), Times.Once());
        }
    }
}
