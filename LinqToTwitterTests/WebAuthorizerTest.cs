using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using LinqToTwitterTests.Common;
using LinqToTwitter;
using Moq;

namespace LinqToTwitterTests
{
    
    
    /// <summary>
    ///This is a test class for WebAuthorizerTest and is intended
    ///to contain all WebAuthorizerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WebAuthorizerTest
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

        [TestMethod]
        public void BeginAuthorization_Gets_Request_Token()
        {
            string requestUrl = "https://api.twitter.com/";
            var webAuth = new WebAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            webAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            helperMock.Setup(helper => helper.GetRequestUrl()).Returns(requestUrl);
            webAuth.OAuthHelper = helperMock.Object;

            webAuth.BeginAuthorization();

            oAuthMock.Verify(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), requestUrl, false, false), Times.Once());
        }

        [TestMethod]
        public void CompleteAuthorization_Gets_Access_Token()
        {
            string screenName = "JoeMayo";
            string userID = "123";
            string verifier = "1234567";
            string authToken = "token";
            string authLink = "https://authorizationlink?oauth_token=" + authToken;
            var webAuth = new WebAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "https://authorizationlink", false, false))
                     .Returns(authLink);
            oAuthMock.Setup(oAuth => oAuth.AccessTokenGet(authToken, verifier, It.IsAny<string>(), string.Empty, out screenName, out userID));
            webAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            helperMock.Setup(helper => helper.GetRequestParam("oauth_token")).Returns(authToken);
            helperMock.Setup(helper => helper.GetRequestParam("oauth_verifier")).Returns(verifier);
            webAuth.OAuthHelper = helperMock.Object;

            webAuth.Authorize();

            oAuthMock.Verify(oauth => oauth.AccessTokenGet(authToken, verifier, It.IsAny<string>(), string.Empty, out screenName, out userID), Times.Once());
            Assert.AreEqual(screenName, webAuth.ScreenName);
            Assert.AreEqual(userID, webAuth.UserId);
        }
    }
}
