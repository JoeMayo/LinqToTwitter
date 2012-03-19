using System;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LinqToTwitterTests
{
    [TestClass]
    public class WebAuthorizerTest
    {
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void BeginAuthorization_Gets_Request_Token()
        {
            const string requestUrl = "https://api.twitter.com/";
            var webAuth = new WebAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            webAuth.OAuthTwitter = oAuthMock.Object;
            oAuthMock.Setup(oauth => oauth.FilterRequestParameters(It.IsAny<Uri>())).Returns(requestUrl);
            string authUrl = string.Empty;
            webAuth.PerformRedirect = url => authUrl = url;

            webAuth.BeginAuthorization(new Uri(requestUrl));

            oAuthMock.Verify(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), requestUrl, false, AuthAccessType.NoChange), Times.Once());
        }

        [TestMethod]
        public void BeginAuthorize_Requires_Credentials()
        {
            const string requestUrl = "https://api.twitter.com/";
            var webAuth = new WebAuthorizer();

            try
            {
                webAuth.BeginAuthorization(new Uri(requestUrl));

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Credentials", ane.ParamName);
            }
        }

        [TestMethod]
        public void BeginAuthorize_Does_Not_Require_A_Uri()
        {
            var webAuth = new WebAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            webAuth.OAuthTwitter = oAuthMock.Object;
            string authUrl = string.Empty;
            webAuth.PerformRedirect = url => authUrl = url;

            webAuth.BeginAuthorization(null);
        }

        [TestMethod]
        public void BeginAuthorization_Calls_PerformRedirect()
        {
            const string requestUrl = "https://api.twitter.com/";
            var webAuth = new WebAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oauth => oauth.FilterRequestParameters(It.IsAny<Uri>())).Returns(requestUrl);
            webAuth.OAuthTwitter = oAuthMock.Object;
            string authUrl = string.Empty;
            webAuth.PerformRedirect = url => authUrl = url;

            webAuth.BeginAuthorization(new Uri(requestUrl));

            Assert.IsNull(authUrl);
        }

        [TestMethod]
        public void CompleteAuthorization_Gets_Access_Token()
        {
            string screenName = "JoeMayo";
            string userID = "123";
            const string verifier = "1234567";
            const string authToken = "token";
            const string authLink = "https://authorizationlink?oauth_token=" + authToken + "&oauth_verifier=" + verifier;
            var webAuth = new WebAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oauth => oauth.GetUrlParamValue(It.IsAny<string>(), "oauth_verifier")).Returns(verifier);
            oAuthMock.Setup(oauth => oauth.GetUrlParamValue(It.IsAny<string>(), "oauth_token")).Returns(authToken);
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "https://authorizationlink", false, AuthAccessType.NoChange))
                     .Returns(authLink);
            oAuthMock.Setup(oAuth => oAuth.AccessTokenGet(authToken, verifier, It.IsAny<string>(), string.Empty, out screenName, out userID));
            webAuth.OAuthTwitter = oAuthMock.Object;

            webAuth.CompleteAuthorization(new Uri(authLink));

            oAuthMock.Verify(oauth => oauth.AccessTokenGet(authToken, verifier, It.IsAny<string>(), string.Empty, out screenName, out userID), Times.Once());
            Assert.AreEqual(screenName, webAuth.ScreenName);
            Assert.AreEqual(userID, webAuth.UserId);
        }

        [TestMethod]
        public void CompleteAuthorization_Requires_A_Uri()
        {
            var webAuth = new WebAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            webAuth.OAuthTwitter = oAuthMock.Object;

            try
            {
                webAuth.CompleteAuthorization(null);

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("callback", ane.ParamName);
            }
        }

        [TestMethod]
        public void CompleteAuthorization_Requires_Credentials()
        {
            const string authLink = "https://authorizationlink";
            var webAuth = new WebAuthorizer();

            try
            {
                webAuth.CompleteAuthorization(new Uri(authLink));

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Credentials", ane.ParamName);
            }
        }
    }
}
