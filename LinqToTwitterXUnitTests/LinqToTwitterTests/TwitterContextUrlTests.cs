using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Moq;
using Xunit;

namespace LinqToTwitterXUnitTests.LinqToTwitterTests
{
    public class TwitterContextUrlTests
    {
        ITwitterAuthorizer auth;

        public TwitterContextUrlTests()
        {
            TestCulture.SetCulture();
            auth = new Mock<ITwitterAuthorizer>().Object;
        }

        [Fact]
        public void StreamingUrl_Returns_Default()
        {
            const string DefaultStreamingUrl = "https://stream.twitter.com/1.1/";

            string streamingUrl = new TwitterContext(auth).StreamingUrl;

            Assert.Equal(DefaultStreamingUrl, streamingUrl);
        }

        [Fact]
        public void UserStreamUrl_Returns_Default()
        {
            const string DefaultUserStreamUrl = "https://userstream.twitter.com/1.1/";

            string userStreamUrl = new TwitterContext(auth).UserStreamUrl;

            Assert.Equal(DefaultUserStreamUrl, userStreamUrl);
        }

        [Fact]
        public void SiteStreamUrl_Returns_Default()
        {
            const string DefaultSiteStreamUrl = "https://sitestream.twitter.com/1.1/";

            string siteStreamUrl = new TwitterContext(auth).SiteStreamUrl;

            Assert.Equal(DefaultSiteStreamUrl, siteStreamUrl);
        }
    }
}
