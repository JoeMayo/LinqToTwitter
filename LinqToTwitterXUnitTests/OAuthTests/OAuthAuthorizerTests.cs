using System;
using System.Net;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class OAuthAuthorizerTests
    {
        public OAuthAuthorizerTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void Get_Calls_GetOAuthQueryString()
        {
            var req = new Request("https://api.twitter.com/statuses/public.xml");
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            string outUrl;
            string queryString = "oauth_token=token";
            oAuthMock.Setup(oAuth => oAuth.GetOAuthQueryString(HttpMethod.GET, req, string.Empty, out outUrl, out queryString));
            pinAuth.OAuthTwitter = oAuthMock.Object;

            pinAuth.Get(req);

            oAuthMock.Verify(oAuth => oAuth.GetOAuthQueryString(HttpMethod.GET, req, string.Empty, out outUrl, out queryString), Times.Once());
        }

        [Fact]
        public void InitializeRequest_Sets_Request_Headers()
        {
            var req = new Request("https://api.twitter.com/statuses/public.xml");
            const string UserAgent = "LINQ to Twitter v2.0";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            string outUrl;
            string queryString = "oauth_token=token";
            oAuthMock.Setup(oAuth => oAuth.GetOAuthQueryString(HttpMethod.GET, req, string.Empty, out outUrl, out queryString));
            pinAuth.OAuthTwitter = oAuthMock.Object;

            pinAuth.UserAgent = UserAgent;
            pinAuth.ReadWriteTimeout = new TimeSpan(0, 0, 1);
            pinAuth.Timeout = new TimeSpan(0, 0, 2);
            pinAuth.UseCompression = true;

            var httpReq = pinAuth.Get(req) as HttpWebRequest;

            Assert.NotNull(httpReq);
            Assert.Equal(UserAgent, httpReq.UserAgent);
            Assert.Equal(1000, httpReq.ReadWriteTimeout);
            Assert.Equal(2000, httpReq.Timeout);
            Assert.Equal("gzip, deflate", httpReq.Headers[HttpRequestHeader.AcceptEncoding]);
            Assert.Equal(DecompressionMethods.Deflate | DecompressionMethods.GZip, httpReq.AutomaticDecompression);
        }

        [Fact]
        public void IsAuthorized_Throws_ArgumentNullException_For_Null_Credentials()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new PinAuthorizer().IsAuthorized);

            Assert.Equal("Credentials", ex.ParamName);
        }

        [Fact]
        public void IsAuthorized_Returns_True_When_Credentials_Are_Present()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    AccessToken = "A",
                    ConsumerKey = "C",
                    ConsumerSecret = "S",
                    OAuthToken = "O"
                }
            };

            bool isAuth = pinAuth.IsAuthorized;

            Assert.True(isAuth);
        }

        [Fact]
        public void IsAuthorized_Returns_False_When_4_Credentials_Are_Empty()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    AccessToken = "",
                    ConsumerKey = "",
                    ConsumerSecret = "",
                    OAuthToken = ""
                }
            };

            bool isAuth = pinAuth.IsAuthorized;

            Assert.False(isAuth);
        }

        [Fact]
        public void IsAuthorized_Returns_False_When_3_Credentials_Are_Empty()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    AccessToken = "A",
                    ConsumerKey = "",
                    ConsumerSecret = "",
                    OAuthToken = ""
                }
            };

            bool isAuth = pinAuth.IsAuthorized;

            Assert.False(isAuth);
        }

        [Fact]
        public void IsAuthorized_Returns_False_When_2_Credentials_Are_Empty()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    AccessToken = "A",
                    ConsumerKey = "C",
                    ConsumerSecret = "",
                    OAuthToken = ""
                }
            };

            bool isAuth = pinAuth.IsAuthorized;

            Assert.False(isAuth);
        }

        [Fact]
        public void IsAuthorized_Returns_False_When_1_Credential_Is_Empty()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    AccessToken = "A",
                    ConsumerKey = "C",
                    ConsumerSecret = "S",
                    OAuthToken = ""
                }
            };

            bool isAuth = pinAuth.IsAuthorized;

            Assert.False(isAuth);
        }

        [Fact]
        public void IsAuthorized_Returns_False_When_4_Credentials_Are_Null()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    AccessToken = null,
                    ConsumerKey = null,
                    ConsumerSecret = null,
                    OAuthToken = null
                }
            };

            bool isAuth = pinAuth.IsAuthorized;

            Assert.False(isAuth);
        }

        [Fact]
        public void IsAuthorized_Returns_False_When_3_Credentials_Are_Null()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    AccessToken = "A",
                    ConsumerKey = null,
                    ConsumerSecret = null,
                    OAuthToken = null
                }
            };

            bool isAuth = pinAuth.IsAuthorized;

            Assert.False(isAuth);
        }

        [Fact]
        public void IsAuthorized_Returns_False_When_2_Credentials_Are_Null()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    AccessToken = "A",
                    ConsumerKey = "C",
                    ConsumerSecret = null,
                    OAuthToken = null
                }
            };

            bool isAuth = pinAuth.IsAuthorized;

            Assert.False(isAuth);
        }

        [Fact]
        public void IsAuthorized_Returns_False_When_1_Credential_Is_Null()
        {
            var pinAuth = new PinAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    AccessToken = "A",
                    ConsumerKey = "C",
                    ConsumerSecret = "S",
                    OAuthToken = null
                }
            };

            bool isAuth = pinAuth.IsAuthorized;

            Assert.False(isAuth);
        }
    }
}
