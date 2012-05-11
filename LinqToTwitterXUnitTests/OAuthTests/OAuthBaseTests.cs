using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using LinqToTwitter;
using LinqToTwitterTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests.OAuthTests
{
    public class OAuthBaseTests
    {
        public OAuthBaseTests()
        {
            TestCulture.SetCulture();
        }

#if NETFX_CORE
        [Fact]
        public void ComputeHash_Calculates_Hash()
        {
            const string ExpectedHashValue = "6/oHmxtB0MVw/bedxThQcBK7AOc=";
            const string SignatureData = "GET&https%3A%2F%2Fapi.twitter.com%2Foauth%2Frequest_token&oauth_callback%3Doob%26oauth_consumer_key%3DyK7Unbaiw1ksbiela8brQb%26oauth_nonce%3D4785638%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1336062691%26oauth_version%3D1.0";
            const string HashKey = "Yhqksk39nP627dyna6vcUIEha2jbaplcmaHdGFVjV9&";
            var hmacsha1 = new HMACSHA1
            {
                Key = Encoding.UTF8.GetBytes(HashKey)
            };
            var oauthBase = new OAuthBase();

            string hash = oauthBase.ComputeHash(hmacsha1, SignatureData);

            Assert.Equal(ExpectedHashValue, hash);
        }
#endif
    }
}
