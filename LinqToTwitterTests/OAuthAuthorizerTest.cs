using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using LinqToTwitterTests.Common;
using Moq;
using System.Collections.Generic;

namespace LinqToTwitterTests
{
    
    
    /// <summary>
    ///This is a test class for OAuthAuthorizerTest and is intended
    ///to contain all OAuthAuthorizerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OAuthAuthorizerTest
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
        public void Get_Calls_GetOAuthQueryString()
        {
            string url = "https://api.twitter.com/statuses/public.xml";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            string outUrl = url;
            string queryString = "oauth_token=token";
            oAuthMock.Setup(oAuth => oAuth.GetOAuthQueryString(HttpMethod.GET, url, string.Empty, out outUrl, out queryString));
            pinAuth.OAuthTwitter = oAuthMock.Object;

            pinAuth.Get(url);

            oAuthMock.Verify(oAuth => oAuth.GetOAuthQueryString(HttpMethod.GET, url, string.Empty, out outUrl, out queryString), Times.Once());
        }

        [TestMethod]
        public void InitializeRequest_Sets_Request_Headers()
        {
            string url = "https://api.twitter.com/statuses/public.xml";
            string userAgent = "LINQ to Twitter v2.0";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            string outUrl = url;
            string queryString = "oauth_token=token";
            oAuthMock.Setup(oAuth => oAuth.GetOAuthQueryString(HttpMethod.GET, url, string.Empty, out outUrl, out queryString));
            pinAuth.OAuthTwitter = oAuthMock.Object;

            pinAuth.UserAgent = userAgent;
            pinAuth.ReadWriteTimeout = new TimeSpan(0, 0, 1);
            pinAuth.Timeout = new TimeSpan(0, 0, 2);
            pinAuth.UseCompression = true;

            HttpWebRequest req = pinAuth.Get(url) as HttpWebRequest;

            Assert.AreEqual(userAgent, req.UserAgent);
            Assert.AreEqual(1000, req.ReadWriteTimeout);
            Assert.AreEqual(2000, req.Timeout);
            Assert.AreEqual("gzip, deflate", req.Headers[HttpRequestHeader.AcceptEncoding]);
            Assert.AreEqual(DecompressionMethods.Deflate | DecompressionMethods.GZip, req.AutomaticDecompression);
        }

        [TestMethod]
        public void Post_With_Params_Calls_GetOAuthQueryStringForPost()
        {
            string url = "https://api.twitter.com/statuses/public.xml";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;

            pinAuth.Post(url,
                new Dictionary<string, string>
                {
                    {"param1", "val1" }
                });

            oAuthMock.Verify(oAuth => oAuth.GetOAuthQueryStringForPost(It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void Post_With_Params_Gets_HttpWebResponse()
        {
            string url = "https://api.twitter.com/statuses/public.xml";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;

            pinAuth.Post(url,
                new Dictionary<string, string>
                {
                    {"param1", "val1" }
                });

            helperMock.Verify(helper => helper.GetResponse(It.IsAny<HttpWebRequest>()), Times.Once());
        }

        [TestMethod]
        public void Post_Without_Params_Calls_GetOAuthQueryStringForPost()
        {
            string url = "https://api.twitter.com/statuses/public.xml";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;

            pinAuth.Post(url);

            oAuthMock.Verify(oAuth => oAuth.GetOAuthQueryStringForPost(It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public void Post_Without_Params_Sets_Method_To_POST()
        {
            string url = "https://api.twitter.com/statuses/public.xml";
            var pinAuth = new PinAuthorizer();
            var oAuthMock = new Mock<IOAuthTwitter>();
            pinAuth.OAuthTwitter = oAuthMock.Object;
            var helperMock = new Mock<IOAuthHelper>();
            pinAuth.OAuthHelper = helperMock.Object;

            HttpWebRequest req = pinAuth.Post(url);

            Assert.AreEqual("POST", req.Method);
        }

        [TestMethod]
        public void IsAuthorized_Throws_ArgumentNullException_For_Null_Credentials()
        {
            var pinAuth = new PinAuthorizer();

            try
            {
                bool isAuth = pinAuth.IsAuthorized;

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
