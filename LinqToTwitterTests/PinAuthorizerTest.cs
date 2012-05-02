using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using LinqToTwitterTests.Common;

namespace LinqToTwitterTests
{
    [TestClass]
    public class PinAuthorizerTest
    {
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void Authorize_Gets_Request_Token()
        {
            const string AuthLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, AuthAccessType.NoChange))
                     .Returns(AuthLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            pinAuth.GetPin = () => "1234567";
            string destinationUrl = string.Empty;
            pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

            pinAuth.Authorize();

            oAuthMock.Verify(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, AuthAccessType.NoChange), Times.Once());
            Assert.AreEqual(AuthLink, destinationUrl);
        }

        [TestMethod]
        public void Authorize_Launches_Browser()
        {
            const string AuthLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, AuthAccessType.NoChange))
                     .Returns(AuthLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            pinAuth.GetPin = () => "1234567";
            string destinationUrl = string.Empty;
            pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

            pinAuth.Authorize();

            Assert.AreEqual(AuthLink, destinationUrl);
        }

        [TestMethod]
        public void Authorize_Gets_Pin()
        {
            const string AuthLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, AuthAccessType.NoChange))
                     .Returns(AuthLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            bool pinSet = false;
            pinAuth.GetPin = () => { pinSet = true; return "1234567"; };
            string destinationUrl = string.Empty;
            pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

            pinAuth.Authorize();

            Assert.IsTrue(pinSet);
            Assert.AreEqual(AuthLink, destinationUrl);
        }

        [TestMethod]
        public void Authorize_Requires_GetPin_Handler()
        {
            const string AuthLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, AuthAccessType.NoChange))
                     .Returns(AuthLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            string destinationUrl = string.Empty;
            pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

            try
            {
                pinAuth.Authorize();
                Assert.AreEqual(AuthLink, destinationUrl);

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
            const string AuthLink = "https://authorizationlink";
            var pinAuth = new PinAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, AuthAccessType.NoChange))
                     .Returns(AuthLink);
            pinAuth.OAuthTwitter = oAuthMock.Object;
            pinAuth.GetPin = () => { return "1234567"; };

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
        public void Authorize_Requires_Credentials()
        {
            var pinAuth = new PinAuthorizer();

            try
            {
                pinAuth.Authorize();

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Credentials", ane.ParamName);
            }
        }

        [TestMethod]
        public void Authorize_Gets_Access_Token()
        {
            string screenName = "JoeMayo";
            string userID = "123";
            const string PinCode = "1234567";
            const string AuthToken = "token";
            const string AuthLink = "https://authorizationlink?oauth_token=" + AuthToken;
            var pinAuth = new PinAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "oob", false, AuthAccessType.NoChange))
                     .Returns(AuthLink);
            oAuthMock.Setup(oAuth => oAuth.AccessTokenGet(AuthToken, PinCode, It.IsAny<string>(), string.Empty, out screenName, out userID));
            pinAuth.OAuthTwitter = oAuthMock.Object;
            pinAuth.GetPin = () => PinCode;
            string destinationUrl = string.Empty;
            pinAuth.GoToTwitterAuthorization = link => destinationUrl = link;

            pinAuth.Authorize();

            oAuthMock.Verify(oauth => oauth.AccessTokenGet(AuthToken, PinCode, It.IsAny<string>(), string.Empty, out screenName, out userID), Times.Once());
            Assert.AreEqual(screenName, pinAuth.ScreenName);
            Assert.AreEqual(userID, pinAuth.UserId);
            Assert.AreEqual(AuthLink, destinationUrl);
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
                GetPin = () => "1234567",
                GoToTwitterAuthorization = link => { }
            };

            pinAuth.Authorize();

            oAuthMock.Verify(oauth =>
                oauth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), false, AuthAccessType.NoChange),
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
                GetPin = () => "1234567",
                GoToTwitterAuthorization = link => { }
            };

            pinAuth.BeginAuthorize(resp => { });

            oAuthMock.Verify(oAuth =>
                oAuth.GetRequestTokenAsync(oauthRequestTokenUrl, oauthAuthorizeUrl, "oob", AuthAccessType.NoChange, false, It.IsAny<Action<string>>(), It.IsAny<Action<TwitterAsyncResponse<object>>>()),
                Times.Once());
        }

        [TestMethod]
        public void BeginAuthorize_Requires_GoToTwitterAuthorization_Handler()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials(),
                OAuthTwitter = new OAuthTwitterMock(),
                GetPin = () => "1234567",
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
                GetPin = () => "1234567",
                GoToTwitterAuthorization = link => { }
            };

            pinAuth.BeginAuthorize(resp => twitterResp = resp);

            oAuthMock.Verify(oauth =>
                oauth.GetRequestTokenAsync(It.IsAny<Uri>(), It.IsAny<Uri>(), "oob", AuthAccessType.NoChange, false, It.IsAny<Action<string>>(), It.IsAny<Action<TwitterAsyncResponse<object>>>()), 
                Times.Never());
            Assert.IsNull(twitterResp);
        }

        [TestMethod]
        public void BeginAuthorize_Invokes_AuthorizationCompleteCallback()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials(),
                OAuthTwitter = new OAuthTwitterMock(),
                GetPin = () => "1234567",
                GoToTwitterAuthorization = link => { }
            };
            TwitterAsyncResponse<object> twitterResp = null;
            pinAuth.BeginAuthorize(resp => twitterResp = resp);

            Assert.IsNotNull(twitterResp);
        }

        [TestMethod]
        public void CompleteAuthorize_Gets_Access_Token()
        {
            const string Pin = "1234567";
            Action<TwitterAsyncResponse<UserIdentifier>> callback = resp => { };
            var oauthAccessTokenUrl = new Uri("https://api.twitter.com/oauth/access_token");
            var oAuthMock = new Mock<IOAuthTwitter>();
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials(),
                OAuthTwitter = oAuthMock.Object,
                GoToTwitterAuthorization = link => { }
            };

            pinAuth.CompleteAuthorize(Pin, callback);

            oAuthMock.Verify(oAuth =>
                oAuth.GetAccessTokenAsync(Pin, oauthAccessTokenUrl, "oob", AuthAccessType.NoChange, callback),
                Times.Once());
        }

        [TestMethod]
        public void CompleteAuthorize_Invokes_AuthorizationCompleteCallback()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials(),
                OAuthTwitter = new OAuthTwitterMock(),
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
            const string Pin = "1234567";
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
                GoToTwitterAuthorization = link => { }
            };

            pinAuth.CompleteAuthorize(Pin, callback);

            oAuthMock.Verify(oAuth =>
                oAuth.GetAccessTokenAsync(Pin, oauthAccessTokenUrl, "oob", AuthAccessType.NoChange, callback),
                Times.Never());
        }
    }
}
