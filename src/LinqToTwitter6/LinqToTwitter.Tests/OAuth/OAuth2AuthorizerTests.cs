using LinqToTwitter.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter.Tests.OAuth
{
    [TestClass]
    public class OAuth2AuthorizerTests
    {
        OAuth2Authorizer auth;

        [TestInitialize]
        public void Init()
        {
            auth = new OAuth2Authorizer();
        }

        [TestMethod]
        public void GenerateCodeChallenge_ReturnsValuesGreaterThanOrEqualTo44()
        {
            string codeChallenge = auth.GenerateCodeChallenge();
            Assert.IsTrue(codeChallenge.Length >= 44);
        }

        [TestMethod]
        public void GenerateCodeChallenge_ReturnsValuesLessThanOrEqualTo127()
        {
            string codeChallenge = auth.GenerateCodeChallenge();
            Assert.IsTrue(codeChallenge.Length <= 127);
        }

        [TestMethod]
        public void GenerateCodeChallenge_ReturnsAlphabeticCharacters()
        {
            string codeChallenge = auth.GenerateCodeChallenge();

            foreach (var ch in codeChallenge)
                Assert.IsTrue(OAuth2Authorizer.ValidCharacters.Contains(ch));
        }

        [TestMethod]
        public void PrepareAuthorizeUrl_WithAllInputs_BuildsUrl()
        {
            const string State = "state";

            auth.CredentialStore =
                new OAuth2CredentialStore
                {
                    ClientID = "MyClientID",
                    ClientSecret = "MyClientSecret",
                    Scopes = new List<string>
                    {
                        "tweet.read", "users.read", "account.follows.read", "account.follows.write"
                    },
                    RedirectUri = "https://www.example.com"
                };

            string authUrl = auth.PrepareAuthorizeUrl(State);

            string[] parts = authUrl.Split('?');

            Dictionary<string, string> queryParms = null;
            if (parts.Length == 2)
                queryParms =
                    (from parm in parts[1].Split('&')
                     select parm.Split('='))
                    .ToDictionary(key => key[0], val => val[1]);

            Assert.IsNotNull(queryParms);
            Assert.AreEqual("code", queryParms["response_type"]);
            Assert.AreEqual("MyClientID", queryParms["client_id"]);
            Assert.AreEqual("https://www.example.com", queryParms["redirect_uri"]);
            Assert.AreEqual("tweet.read users.read account.follows.read account.follows.write", queryParms["scope"]);
            Assert.AreEqual(State, queryParms["state"]);
            Assert.AreEqual("S256", queryParms["code_challenge_method"]);
            Assert.IsTrue(queryParms["code_challenge"].Length > 16);
        }

        [TestMethod]
        public void PrepareTokenParams_WithAllInputs_ReturnsParamString()
        {
            const string CodeFromTwitterAuthorize = "MyCode";
            const string ExpectedParams = 
                "code=MyCode&" +
                "grant_type=authorization_code&" +
                "client_id=My Client ID&" +
                "redirect_uri=https://www.example.com&" +
                "code_verifier=challenge";

            auth.CredentialStore =
                new OAuth2CredentialStore
                {
                    ClientID = "My Client ID",
                    ClientSecret = "MyClientSecret",
                    Scopes = new List<string>
                    {
                        "tweet.read", "users.read", "account.follows.read", "account.follows.write"
                    },
                    RedirectUri = "https://www.example.com",
                    CodeChallenge = "challenge"
                };

            string queryParms = auth.PrepareAccessTokenParams(CodeFromTwitterAuthorize);

            Assert.IsNotNull(queryParms);
            Assert.AreEqual(ExpectedParams, queryParms);
        }
    }
}
