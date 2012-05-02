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
            const string RequestUrl = "https://api.twitter.com/";
            var webAuth = new WebAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            webAuth.OAuthTwitter = oAuthMock.Object;
            oAuthMock.Setup(oauth => oauth.FilterRequestParameters(It.IsAny<Uri>())).Returns(RequestUrl);
            string authUrl = string.Empty;
            webAuth.PerformRedirect = url => authUrl = url;

            webAuth.BeginAuthorization(new Uri(RequestUrl));

            oAuthMock.Verify(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), RequestUrl, false, AuthAccessType.NoChange), Times.Once());
            Assert.IsNull(authUrl);
        }

        [TestMethod]
        public void BeginAuthorize_Requires_Credentials()
        {
            const string RequestUrl = "https://api.twitter.com/";
            var webAuth = new WebAuthorizer();

            try
            {
                webAuth.BeginAuthorization(new Uri(RequestUrl));

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
            Assert.IsNull(authUrl);
        }

        [TestMethod]
        public void BeginAuthorization_Calls_PerformRedirect()
        {
            const string RequestUrl = "https://api.twitter.com/";
            var webAuth = new WebAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oauth => oauth.FilterRequestParameters(It.IsAny<Uri>())).Returns(RequestUrl);
            webAuth.OAuthTwitter = oAuthMock.Object;
            string authUrl = string.Empty;
            webAuth.PerformRedirect = url => authUrl = url;

            webAuth.BeginAuthorization(new Uri(RequestUrl));

            Assert.IsNull(authUrl);
        }

        [TestMethod]
        public void CompleteAuthorization_Gets_Access_Token()
        {
            string screenName = "JoeMayo";
            string userID = "123";
            const string Verifier = "1234567";
            const string AuthToken = "token";
            const string AuthLink = "https://authorizationlink?oauth_token=" + AuthToken + "&oauth_verifier=" + Verifier;
            var webAuth = new WebAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            oAuthMock.Setup(oauth => oauth.GetUrlParamValue(It.IsAny<string>(), "oauth_verifier")).Returns(Verifier);
            oAuthMock.Setup(oauth => oauth.GetUrlParamValue(It.IsAny<string>(), "oauth_token")).Returns(AuthToken);
            oAuthMock.Setup(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), "https://authorizationlink", false, AuthAccessType.NoChange))
                     .Returns(AuthLink);
            oAuthMock.Setup(oAuth => oAuth.AccessTokenGet(AuthToken, Verifier, It.IsAny<string>(), string.Empty, out screenName, out userID));
            webAuth.OAuthTwitter = oAuthMock.Object;

            webAuth.CompleteAuthorization(new Uri(AuthLink));

            oAuthMock.Verify(oauth => oauth.AccessTokenGet(AuthToken, Verifier, It.IsAny<string>(), string.Empty, out screenName, out userID), Times.Once());
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
            const string AuthLink = "https://authorizationlink";
            var webAuth = new WebAuthorizer();

            try
            {
                webAuth.CompleteAuthorization(new Uri(AuthLink));

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Credentials", ane.ParamName);
            }
        }
    }
}
