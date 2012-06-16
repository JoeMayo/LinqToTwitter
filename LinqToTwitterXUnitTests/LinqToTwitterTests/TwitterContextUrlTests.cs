using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
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
            const string DefaultUploadUrl = "https://upload.twitter.com/1/";

            string uploadUrl = new TwitterContext().UploadUrl;

            Assert.Equal(DefaultUploadUrl, uploadUrl);
        }

        [Fact]
        public void StreamingUrl_Returns_Default()
        {
            const string DefaultStreamingUrl = "https://stream.twitter.com/1/";

            string streamingUrl = new TwitterContext().StreamingUrl;

            Assert.Equal(DefaultStreamingUrl, streamingUrl);
        }

        [Fact]
        public void UserStreamUrl_Returns_Default()
        {
            const string DefaultUserStreamUrl = "https://userstream.twitter.com/2/";

            string userStreamUrl = new TwitterContext().UserStreamUrl;

            Assert.Equal(DefaultUserStreamUrl, userStreamUrl);
        }

        [Fact]
        public void SiteStreamUrl_Returns_Default()
        {
            const string DefaultSiteStreamUrl = "https://sitestream.twitter.com/2b/";

            string siteStreamUrl = new TwitterContext().SiteStreamUrl;

            Assert.Equal(DefaultSiteStreamUrl, siteStreamUrl);
        }
    }
}
