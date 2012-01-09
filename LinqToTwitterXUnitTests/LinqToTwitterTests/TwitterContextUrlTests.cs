using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.LinqToTwitterTests
{
    public class TwitterContextUrlTests
    {
        public TwitterContextUrlTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void UploadUrl_Returns_Default()
        {
            const string defaultUploadUrl = "https://upload.twitter.com/1/";

            string uploadUrl = new TwitterContext().UploadUrl;

            Assert.Equal(defaultUploadUrl, uploadUrl);
        }

        [Fact]
        public void StreamingUrl_Returns_Default()
        {
            const string defaultStreamingUrl = "https://stream.twitter.com/1/";

            string streamingUrl = new TwitterContext().StreamingUrl;

            Assert.Equal(defaultStreamingUrl, streamingUrl);
        }

        [Fact]
        public void UserStreamUrl_Returns_Default()
        {
            const string defaultUserStreamUrl = "https://userstream.twitter.com/2/";

            string userStreamUrl = new TwitterContext().UserStreamUrl;

            Assert.Equal(defaultUserStreamUrl, userStreamUrl);
        }

        [Fact]
        public void SiteStreamUrl_Returns_Default()
        {
            const string defaultSiteStreamUrl = "https://sitestream.twitter.com/2b/";

            string siteStreamUrl = new TwitterContext().SiteStreamUrl;

            Assert.Equal(defaultSiteStreamUrl, siteStreamUrl);
        }
    }
}
