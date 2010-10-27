using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using LinqToTwitterTests.Common;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for PinAuthorizerTest and is intended
    ///to contain all PinAuthorizerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PinAuthorizerTest
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
        public void Authorize_Gets_Request_Token()
        {
            string authLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            pinAuth.GetPin = () => { return "1234567"; };
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
        
            pinAuth.Authorize();

            oAuthMock.Verify(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false), Times.Once());
        }

        [TestMethod]
        public void Authorize_Launches_Browser()
        {
            string authLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
            pinAuth.GetPin = () => { return "1234567"; };

            pinAuth.Authorize();

            helperMock.Verify(helper => helper.LaunchBrowser(authLink), Times.Once());
        }

        [TestMethod]
        public void Authorize_Gets_Pin()
        {
            string authLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
            bool pinSet = false;
            pinAuth.GetPin = () => { pinSet = true; return "1234567"; };

            pinAuth.Authorize();

            Assert.IsTrue(pinSet);
        }

        [TestMethod]
        public void Authorize_Requires_GetPin_Handler()
        {
            string authLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
            //bool pinSet = false;
            //pinAuth.GetPin = () => { pinSet = true; return "1234567"; };

            try
            {
                pinAuth.Authorize();

                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException ioe)
            {
                Assert.IsTrue(ioe.Message.Contains("GetPin"));
            }
        }

        [TestMethod]
        public void Authorize_Gets_Access_Token()
        {
            string screenName = "JoeMayo";
            string userID = "123";
            string pinCode = "1234567";
            string authToken = "token";
            string authLink = "https://authorizationlink?oauth_token=" + authToken;
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            oAuthMock.Setup(oAuth => oAuth.AccessTokenGet(authToken, pinCode, It.IsAny<string>(), string.Empty, out screenName, out userID));
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
            pinAuth.GetPin = () => { return pinCode; };

            pinAuth.Authorize();

            oAuthMock.Verify(oauth => oauth.AccessTokenGet(authToken, pinCode, It.IsAny<string>(), string.Empty, out screenName, out userID), Times.Once());
            Assert.AreEqual(screenName, pinAuth.ScreenName);
            Assert.AreEqual(userID, pinAuth.UserId);
        }
    }
}
