using System;
using System.Linq;
using LinqToTwitter;
using LinqToTwitterXUnitTests.Common;
using Xunit;

namespace LinqToTwitterXUnitTests
{
    public class ApplicationOnlyAuthorizerTests
    {
        public ApplicationOnlyAuthorizerTests()
        {
            TestCulture.SetCulture();
        }

        [Fact]
        public void EncodeCredentials_Returns_Valid_Credentials()
        {
            const string ExpectedEncodedCredentials = "eHZ6MWV2RlM0d0VFUFRHRUZQSEJvZzpMOHFxOVBaeVJnNmllS0dFS2hab2xHQzB2SldMdzhpRUo4OERSZHlPZw==";
            var auth = new ApplicationOnlyAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = "xvz1evFS4wEEPTGEFPHBog",
                    ConsumerSecret = "L8qq9PZyRg6ieKGEKhZolGC0vJWLw8iEJ88DRdyOg"
                }
            };

            auth.EncodeCredentials();

            Assert.Equal(ExpectedEncodedCredentials, auth.BasicToken);
        }
    }
}
