using System;
using System.Collections.Generic;
using System.Linq;
using LinqToTwitter.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqToTwitterPcl.Tests.SecurityTests
{
    [TestClass]
    public class OAuthTests
    {
        const string Method = "POST";
        const string Url = "https://api.twitter.com/1/statuses/update.json";
        const string StatusKey = "status";
        const string EntitiesKey = "include_entities";
        const string ConsumerSecret = "kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw";
        const string OAuthTokenSecret = "LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE";

        IDictionary<string, string> parameters;
        OAuth oAuth;

        [TestInitialize]
        public void InitTests()
        {
            oAuth = new OAuth();

            // No security vulnerability - credentials are from Twitter's Creating a signature documentation: https://dev.twitter.com/docs/auth/creating-signature
            parameters =
                new Dictionary<string, string>
                {
                    {StatusKey, "Hello Ladies + Gentlemen, a signed OAuth request!"},
                    {EntitiesKey, "true"},
                    {"oauth_consumer_key", "xvz1evFS4wEEPTGEFPHBog" },
                    {"oauth_nonce", "kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg" },
                    {"oauth_signature_method", "HMAC-SHA1" },
                    {"oauth_timestamp", "1318622958" },
                    {"oauth_token", "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb" }, 
                    {"oauth_version", "1.0" }
                };
        }

        [TestMethod]
        public void GetAuthorizationStringReturnsValidString()
        {
            const string ExpectedAuthorizationString = "OAuth oauth_consumer_key=\"xvz1evFS4wEEPTGEFPHBog\", oauth_nonce=\"kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg\", oauth_signature=\"tnnArxj06cWHq44gCs1OSKk%2FjLY%3D\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"1318622958\", oauth_token=\"370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb\", oauth_version=\"1.0\"";

            string authString = oAuth.GetAuthorizationString(Method, Url, parameters, ConsumerSecret, OAuthTokenSecret);

            Assert.AreEqual(ExpectedAuthorizationString, authString);
        }

        [TestMethod]
        public void GetAuthorizationStringAddsMissingParameters()
        {
            parameters =
                new Dictionary<string, string>
                {
                    {StatusKey, "Hello Ladies + Gentlemen, a signed OAuth request!"},
                    {EntitiesKey, "true"},
                    {"oauth_consumer_key", "xvz1evFS4wEEPTGEFPHBog" },
                    {"oauth_token", "370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb" }, 
                };

            oAuth.GetAuthorizationString(Method, Url, parameters, ConsumerSecret, OAuthTokenSecret);

            Assert.IsTrue(parameters.ContainsKey("oauth_nonce"));
            Assert.IsTrue(parameters.ContainsKey("oauth_timestamp"));
            Assert.IsTrue(parameters.ContainsKey("oauth_signature_method"));
            Assert.IsTrue(parameters.ContainsKey("oauth_version"));
        }

        [TestMethod]
        public void BuildEncodedSortedStringTransformsParametersIntoEncodedSortedString()
        {
            const string ExpectedString = "include_entities=true&oauth_consumer_key=xvz1evFS4wEEPTGEFPHBog&oauth_nonce=kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1318622958&oauth_token=370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb&oauth_version=1.0&status=Hello%20Ladies%20%2B%20Gentlemen%2C%20a%20signed%20OAuth%20request%21";
            
            string encodedSortedResponse = oAuth.BuildEncodedSortedString(parameters);

            Assert.AreEqual(ExpectedString, encodedSortedResponse);
        }

        [TestMethod]
        public void CreateSignatureBaseStringReturnsProperString()
        {
            const string EncodedStringParameters = "include_entities=true&oauth_consumer_key=xvz1evFS4wEEPTGEFPHBog&oauth_nonce=kYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg&oauth_signature_method=HMAC-SHA1&oauth_timestamp=1318622958&oauth_token=370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb&oauth_version=1.0&status=Hello%20Ladies%20%2B%20Gentlemen%2C%20a%20signed%20OAuth%20request%21";
            const string ExpectedSignatureBaseString = "POST&https%3A%2F%2Fapi.twitter.com%2F1%2Fstatuses%2Fupdate.json&include_entities%3Dtrue%26oauth_consumer_key%3Dxvz1evFS4wEEPTGEFPHBog%26oauth_nonce%3DkYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1318622958%26oauth_token%3D370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb%26oauth_version%3D1.0%26status%3DHello%2520Ladies%2520%252B%2520Gentlemen%252C%2520a%2520signed%2520OAuth%2520request%2521";

            string signatureBaseString = oAuth.BuildSignatureBaseString(Method, Url, EncodedStringParameters);

            Assert.AreEqual(ExpectedSignatureBaseString, signatureBaseString);
        }

        [TestMethod]
        public void CreateSigningKeyReturnsProcessedConsumerSecretAndOAuthTokenSecret()
        {
            const string ExpectedSigningKey = "kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw&LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE";

            string signingKey = oAuth.BuildSigningKey(ConsumerSecret, OAuthTokenSecret);

            Assert.AreEqual(ExpectedSigningKey, signingKey);
        }

        [TestMethod]
        public void CalculateSignatureReturnsSignatureString()
        {
            const string ExpectedSignature = "tnnArxj06cWHq44gCs1OSKk/jLY=";
            const string SignatureBaseString = "POST&https%3A%2F%2Fapi.twitter.com%2F1%2Fstatuses%2Fupdate.json&include_entities%3Dtrue%26oauth_consumer_key%3Dxvz1evFS4wEEPTGEFPHBog%26oauth_nonce%3DkYjzVBB8Y0ZFabxSWbWovY3uYSQ2pTgmZeNu2VS4cg%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1318622958%26oauth_token%3D370773112-GmHxMAgYyLbNEtIKZeRNFsMKPR9EyMZeS9weJAEb%26oauth_version%3D1.0%26status%3DHello%2520Ladies%2520%252B%2520Gentlemen%252C%2520a%2520signed%2520OAuth%2520request%2521";
            const string SigningKey = "kAcSOqF21Fu85e7zjz7ZN2U4ZRhfV3WpwPAoE3Z7kBw&LswwdoUaIvS8ltyTt5jkRh4J50vUPVVHtR2YPi5kE";

            string signature = oAuth.CalculateSignature(SigningKey, SignatureBaseString);

            Assert.AreEqual(ExpectedSignature, signature);
        }
    }
}
