using System;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class WebAuthorizerTests
    {
        public WebAuthorizerTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
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
            Assert.Null(authUrl);
        }

        [Fact]
        public void BeginAuthorize_Requires_Credentials()
        {
            const string RequestUrl = "https://api.twitter.com/";
            var webAuth = new WebAuthorizer();

            var ex = Assert.Throws<ArgumentNullException>(() => webAuth.BeginAuthorization(new Uri(RequestUrl)));

            Assert.Equal("Credentials", ex.ParamName);
        }

        [Fact]
        public void BeginAuthorize_Does_Not_Require_A_Uri()
        {
            var webAuth = new WebAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            webAuth.OAuthTwitter = oAuthMock.Object;
            string authUrl = string.Empty;
            webAuth.PerformRedirect = url => authUrl = url;

            webAuth.BeginAuthorization(null);
            Assert.Null(authUrl);
        }

        [Fact]
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

            Assert.Null(authUrl);
        }

        [Fact]
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
            Assert.Equal(screenName, webAuth.ScreenName);
            Assert.Equal(userID, webAuth.UserId);
        }

        [Fact]
        public void CompleteAuthorization_Requires_A_Uri()
        {
            var webAuth = new WebAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            webAuth.OAuthTwitter = oAuthMock.Object;

            var ex = Assert.Throws<ArgumentNullException>(() => webAuth.CompleteAuthorization(null));

            Assert.Equal("callback", ex.ParamName);
        }

        [Fact]
        public void CompleteAuthorization_Requires_Credentials()
        {
            const string AuthLink = "https://authorizationlink";
            var webAuth = new WebAuthorizer();

            var ex = Assert.Throws<ArgumentNullException>(() => webAuth.CompleteAuthorization(new Uri(AuthLink)));

            Assert.Equal("Credentials", ex.ParamName);
        }
    }
}
