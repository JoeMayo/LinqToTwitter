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
            const string ExpectedHashValue = "";
            const string SignatureData = "";
            const string HashKey = "";

            var oauthBase = new OAuthBase();

            string hash = oauthBase.HashWith(SignatureData, HashKey);
        }
#endif
    }
}
