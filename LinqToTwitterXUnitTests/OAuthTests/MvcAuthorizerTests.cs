using System;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class MvcAuthorizerTests
    {
        public MvcAuthorizerTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void BeginAuthorization_Sets_Callback()
        {
            var auth = new MvcAuthorizer();
            Uri linqToTwitterUri = new Uri("http://linqtotwitter.codeplex.com");

            auth.BeginAuthorization(linqToTwitterUri);

            Assert.Equal(linqToTwitterUri, auth.Callback);
        }
    }
}
