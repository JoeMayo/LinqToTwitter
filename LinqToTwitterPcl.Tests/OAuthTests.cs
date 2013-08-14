using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests
{
    [TestClass]
    public class OAuthTests
    {
        const string Method = "POST";
        const string Url = "https://api.twitter.com/1/statuses/update.json";
        const string StatusKey = "status";
        const string EntitiesKey = "include_entities";

        IDictionary<string, string> parameters;
        OAuth oAuth;

        [TestInitialize]
        public void InitTests()
        {
            oAuth = new OAuth();

            // No security vulnerability - credentials are from Twitter's Creating a signature documentation: https://dev.twitter.com/docs/auth/creating-signature
            oAuth.ConsumerKey = "xvz1evFS4wEEPTGEFPHBog";
            oAuth.OAuthToken = "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb";
            parameters =
                new Dictionary<string, string>
                {
                    {StatusKey, "Hello Ladies + Gentlemen, a signed OAuth request!"},
                    {EntitiesKey, "true"}
                };
        }

        [Ignore]
        [TestMethod]
        public void GetAuthorizationStringReturnsAString()
        {
            string authString = oAuth.GetAuthorizationString(Method, Url, parameters);

            Assert.IsNotNull(authString);
        }

        [TestMethod]
        public void BuildEncodedSortedStringTransformsParametersIntoEncodedSortedString()
        {
            const string ExpectedString = EntitiesKey + "=true&" + StatusKey + "=Hello%20Ladies%20%2B%20Gentlemen%2C%20a%20signed%20OAuth%20request%21";
            string encodedSortedResponse = oAuth.BuildEncodedSortedString(parameters);

            Assert.AreEqual(ExpectedString, encodedSortedResponse);
        }
    }
}
