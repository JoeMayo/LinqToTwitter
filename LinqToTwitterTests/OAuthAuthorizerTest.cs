using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using LinqToTwitterTests.Common;
using Moq;

namespace LinqToTwitterTests
{
    [TestClass]
    public class OAuthAuthorizerTest
    {
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
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

        [TestMethod]
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

            Assert.IsNotNull(httpReq);
            Assert.AreEqual(UserAgent, httpReq.UserAgent);
            Assert.AreEqual(1000, httpReq.ReadWriteTimeout);
            Assert.AreEqual(2000, httpReq.Timeout);
            Assert.AreEqual("gzip, deflate", httpReq.Headers[HttpRequestHeader.AcceptEncoding]);
            Assert.AreEqual(DecompressionMethods.Deflate | DecompressionMethods.GZip, httpReq.AutomaticDecompression);
        }

        [TestMethod]
        public void IsAuthorized_Throws_ArgumentNullException_For_Null_Credentials()
        {
            try
            {
                bool isAuth = new PinAuthorizer().IsAuthorized;
                Assert.AreEqual(isAuth, false);

                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException ane)
            {
                Assert.AreEqual("Credentials", ane.ParamName);
            }
        }

        [TestMethod]
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

            Assert.IsTrue(isAuth);
        }

        [TestMethod]
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

            Assert.IsFalse(isAuth);
        }

        [TestMethod]
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

            Assert.IsFalse(isAuth);
        }

        [TestMethod]
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

            Assert.IsFalse(isAuth);
        }

        [TestMethod]
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

            Assert.IsFalse(isAuth);
        }

        [TestMethod]
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

            Assert.IsFalse(isAuth);
        }

        [TestMethod]
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

            Assert.IsFalse(isAuth);
        }

        [TestMethod]
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

            Assert.IsFalse(isAuth);
        }

        [TestMethod]
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

            Assert.IsFalse(isAuth);
        }
    }
}
