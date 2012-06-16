using System;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class SignInAuthorizerTests
    {
        public SignInAuthorizerTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void BeginAuthorization_Sets_ForceLogin()
        {
            const string RequestUrl = "https://api.twitter.com/";
            var signInAuth = new SignInAuthorizer {Credentials = new InMemoryCredentials()};
            var oAuthMock = new Mock<IOAuthTwitter>();
            signInAuth.OAuthTwitter = oAuthMock.Object;
            oAuthMock.Setup(oauth => oauth.FilterRequestParameters(It.IsAny<Uri>())).Returns(RequestUrl);
            string authUrl = string.Empty;
            signInAuth.PerformRedirect = url => authUrl = url;

            signInAuth.BeginAuthorization(new Uri(RequestUrl), forceLogin: true);

            oAuthMock.Verify(oAuth => oAuth.AuthorizationLinkGet(It.IsAny<string>(), It.IsAny<string>(), RequestUrl, true, AuthAccessType.NoChange), Times.Once());
            Assert.Null(authUrl);
        }
    }
}
