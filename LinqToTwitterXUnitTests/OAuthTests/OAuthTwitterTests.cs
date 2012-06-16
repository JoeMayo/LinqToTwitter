using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using System;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class OAuthTwitterTests
    {
        public OAuthTwitterTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void FilterRequestParameters_Splits_Url_Properly()
        {
            var oaTwit = new OAuthTwitter();
            var fullUrl = new Uri("http://www.mySite.com?oauth_token=123&p1=v1");
            
            string filteredUrl = oaTwit.FilterRequestParameters(fullUrl);

            Assert.Equal("http://www.mysite.com/?p1=v1", filteredUrl);
        }
    }
}
