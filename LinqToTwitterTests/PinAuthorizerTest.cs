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
            pinAuth.Credentials = new InMemoryCredentials();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            pinAuth.GetPin = () => { return "1234567"; };
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
            string destinationUrl = string.Empty;
            pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

            pinAuth.Authorize();

            oAuthMock.Verify(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false), Times.Once());
        }

        [TestMethod]
        public void Authorize_Launches_Browser()
        {
            string authLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer();
            pinAuth.Credentials = new InMemoryCredentials();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
            pinAuth.GetPin = () => { return "1234567"; };
            string destinationUrl = string.Empty;
            pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

            pinAuth.Authorize();

            Assert.AreEqual(authLink, destinationUrl);
        }

        [TestMethod]
        public void Authorize_Gets_Pin()
        {
            string authLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer();
            pinAuth.Credentials = new InMemoryCredentials();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
            bool pinSet = false;
            pinAuth.GetPin = () => { pinSet = true; return "1234567"; };
            string destinationUrl = string.Empty;
            pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

            pinAuth.Authorize();

            Assert.IsTrue(pinSet);
        }

        [TestMethod]
        public void Authorize_Requires_GetPin_Handler()
        {
            string authLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer();
            pinAuth.Credentials = new InMemoryCredentials();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
            //bool pinSet = false;
            //pinAuth.GetPin = () => { pinSet = true; return "1234567"; };
            string destinationUrl = string.Empty;
            pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

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
        public void Authorize_Requires_GoToTwitterAuthorization_Handler()
        {
            string authLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer();
            pinAuth.Credentials = new InMemoryCredentials();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
            bool pinSet = false;
            pinAuth.GetPin = () => { pinSet = true; return "1234567"; };
            //string destinationUrl = string.Empty;
            //pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

            try
            {
                pinAuth.Authorize();

                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException ioe)
            {
                Assert.IsTrue(ioe.Message.Contains("GoToTwitterAuthorization"));
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
            pinAuth.Credentials = new InMemoryCredentials();
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, false))
                     .Returns(authLink);
            oAuthMock.Setup(oAuth => oAuth.AccessTokenGet(authToken, pinCode, It.IsAny<string>(), string.Empty, out screenName, out userID));
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;
            pinAuth.GetPin = () => { return pinCode; };
            string destinationUrl = string.Empty;
            pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

            pinAuth.Authorize();

            oAuthMock.Verify(oauth => oauth.AccessTokenGet(authToken, pinCode, It.IsAny<string>(), string.Empty, out screenName, out userID), Times.Once());
            Assert.AreEqual(screenName, pinAuth.ScreenName);
            Assert.AreEqual(userID, pinAuth.UserId);
        }

        [TestMethod]
        public void Authorize_Returns_If_Already_Authorized()
        {
            var oAuthMock = new Mock<IOAuthTwitter>();
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = "consumerkey",
                    ConsumerSecret = "consumersecret",
                    OAuthToken = "oauthtoken",
                    AccessToken = "accesstoken"
                },
                OAuthTwitter = oAuthMock.Object,
                OAuthHelper = new Mock<IOAuthHelper>().Object,
                GetPin = () => { return "1234567"; },
                GoToTwitterAuthorization = link => { }
            };

            pinAuth.Authorize();

            oAuthMock.Verify(oauth =>
                oauth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), false, false),
                Times.Never());
        }

        [TestMethod]
        public void BeginAuthorize_Gets_Request_Token()
        {
            var oauthRequestTokenUrl = new Uri("https://api.twitter.com/oauth/request_token");
            var oauthAuthorizeUrl = new Uri("https://api.twitter.com/oauth/authorize");
            var oAuthMock = new Mock<IOAuthTwitter>();
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials(),
                OAuthTwitter = oAuthMock.Object,
                OAuthHelper = new Mock<IOAuthHelper>().Object,
                GetPin = () => { return "1234567"; },
                GoToTwitterAuthorization = link => { }
            };

            pinAuth.BeginAuthorize(resp => { });

            oAuthMock.Verify(oAuth =>
                oAuth.GetRequestTokenAsync(oauthRequestTokenUrl, oauthAuthorizeUrl, null, false, false, It.IsAny<Action<string>>(), It.IsAny<Action<TwitterAsyncResponse<object>>>()),
                Times.Once());
        }

        [TestMethod]
        public void BeginAuthorize_Requires_GoToTwitterAuthorization_Handler()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials(),
                OAuthTwitter = new OAuthTwitterMock(),
                OAuthHelper = new Mock<IOAuthHelper>().Object,
                GetPin = () => { return "1234567"; },
                //GoToTwitterAuthorization = link => { }
            };

            try
            {
                pinAuth.BeginAuthorize(resp => { });

                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException ioe)
            {
                Assert.IsTrue(ioe.Message.Contains("GoToTwitterAuthorization"));
            }
        }

        [TestMethod]
        public void BeginAuthorize_Returns_If_Already_Authorized()
        {
            var oAuthMock = new Mock<IOAuthTwitter>();
            TwitterAsyncResponse<object> twitterResp = null;
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = "consumerkey",
                    ConsumerSecret = "consumersecret",
                    OAuthToken = "oauthtoken",
                    AccessToken = "accesstoken"
                },
                OAuthTwitter = oAuthMock.Object,
                OAuthHelper = new Mock<IOAuthHelper>().Object,
                GetPin = () => { return "1234567"; },
                GoToTwitterAuthorization = link => { }
            };

            pinAuth.BeginAuthorize(resp => twitterResp = resp);

            oAuthMock.Verify(oauth =>
                oauth.GetRequestTokenAsync(It.IsAny<Uri>(), It.IsAny<Uri>(), null, false, false, It.IsAny<Action<string>>(), It.IsAny<Action<TwitterAsyncResponse<object>>>()), 
                Times.Never());
        }

        [TestMethod]
        public void BeginAuthorize_Invokes_authorizationCompleteCallback()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials(),
                OAuthTwitter = new OAuthTwitterMock(),
                OAuthHelper = new Mock<IOAuthHelper>().Object,
                GetPin = () => { return "1234567"; },
                GoToTwitterAuthorization = link => { }
            };
            TwitterAsyncResponse<object> twitterResp = null;
            pinAuth.BeginAuthorize(resp => twitterResp = resp);

            Assert.IsNotNull(twitterResp);
        }

        [TestMethod]
        public void CompleteAuthorize_Gets_Access_Token()
        {
            string pin = "1234567";
            Action<TwitterAsyncResponse<UserIdentifier>> callback = resp => { };
            var oauthAccessTokenUrl = new Uri("https://api.twitter.com/oauth/access_token");
            var oAuthMock = new Mock<IOAuthTwitter>();
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials(),
                OAuthTwitter = oAuthMock.Object,
                OAuthHelper = new Mock<IOAuthHelper>().Object,
                GoToTwitterAuthorization = link => { }
            };

            pinAuth.CompleteAuthorize(pin, callback);

            oAuthMock.Verify(oAuth =>
                oAuth.GetAccessTokenAsync(pin, oauthAccessTokenUrl, null, callback),
                Times.Once());
        }

        [TestMethod]
        public void CompleteAuthorize_Invokes_authorizationCompleteCallback()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials(),
                OAuthTwitter = new OAuthTwitterMock(),
                OAuthHelper = new Mock<IOAuthHelper>().Object,
                GoToTwitterAuthorization = link => { }
            };
            TwitterAsyncResponse<UserIdentifier> twitterResp = null;

            pinAuth.CompleteAuthorize("1234567", resp => twitterResp = resp);

            Assert.IsNotNull(twitterResp);
        }

        [TestMethod]
        public void CompleteAuthorize_Requires_Pin()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials(),
                OAuthTwitter = new OAuthTwitterMock(),
                OAuthHelper = new Mock<IOAuthHelper>().Object,
                GoToTwitterAuthorization = link => { }
            };

            try
            {
                pinAuth.CompleteAuthorize(null, resp => { });

                Assert.Fail("Expected Exception.");
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("pin", ex.ParamName);
            }
        }

        [TestMethod]
        public void CompleteAuthorize_Returns_If_Already_Authorized()
        {
            string pin = "1234567";
            Action<TwitterAsyncResponse<UserIdentifier>> callback = resp => { };
            var oauthAccessTokenUrl = new Uri("https://api.twitter.com/oauth/access_token");
            var oAuthMock = new Mock<IOAuthTwitter>();
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = "consumerkey",
                    ConsumerSecret = "consumersecret",
                    OAuthToken = "oauthtoken",
                    AccessToken = "accesstoken"
                },
                OAuthTwitter = oAuthMock.Object,
                OAuthHelper = new Mock<IOAuthHelper>().Object,
                GoToTwitterAuthorization = link => { }
            };

            pinAuth.CompleteAuthorize(pin, callback);

            oAuthMock.Verify(oAuth =>
                oAuth.GetAccessTokenAsync(pin, oauthAccessTokenUrl, null, callback),
                Times.Never());
        }
    }
}
