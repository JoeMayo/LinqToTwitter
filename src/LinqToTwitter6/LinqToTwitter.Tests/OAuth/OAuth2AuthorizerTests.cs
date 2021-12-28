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
        public void GenerateCodeChallenge_ReturnsValuesGreaterThanOrEqualTo16()
        {
            string codeChallenge = auth.GenerateCodeChallenge();
            Assert.IsTrue(codeChallenge.Length >= 16);
        }

        [TestMethod]
        public void GenerateCodeChallenge_ReturnsValuesLessThanOrEqualTo32()
        {
            string codeChallenge = auth.GenerateCodeChallenge();
            Assert.IsTrue(codeChallenge.Length <= 32);
        }

        [TestMethod]
        public void GenerateCodeChallenge_ReturnsAlphabeticCharacters()
        {
            string codeChallenge = auth.GenerateCodeChallenge();

            foreach (var ch in codeChallenge)
                Assert.IsTrue(char.IsLetter(ch));
        }

        [TestMethod]
        public void PrepareAuthorizeUrl_WithAllInputs_BuildsUrl()
        {
            auth.Callback = "https://www.example.com";
            auth.CredentialStore =
                new OAuth2CredentialStore
                {
                    ClientID = "MyClientID",
                    ClientSecret = "MyClientSecret",
                    Scopes = new List<string>
                    {
                        "tweet.read", "users.read", "account.follows.read", "account.follows.write"
                    }
                };

            string authUrl = (auth as OAuth2Authorizer).PrepareAuthorizeUrl();

            string[] parts = authUrl.Split('?');

            Dictionary<string, string> queryParms = null;
            if (parts.Length == 2)
                queryParms =
                    (from parm in parts[1].Split('&')
                     select parm.Split('='))
                    .ToDictionary(key => key[0], val => val[1]);

            Assert.IsNotNull(queryParms);
            Assert.AreEqual(queryParms["response_type"], "code");
            Assert.AreEqual(queryParms["client_id"], "MyClientID");
            Assert.AreEqual(queryParms["redirect_uri"], "https://www.example.com");
            Assert.AreEqual(queryParms["scope"], "tweet.read%20users.read%20account.follows.read%20account.follows.write");
            Assert.AreEqual(queryParms["state"], "state");
            Assert.AreEqual(queryParms["code_challenge_method"], "plain");
            Assert.IsTrue(queryParms["code_challenge"].Length > 16);
        }

        //[TestMethod]
        //public void PrepareTokenUrl_WithAllInputs_BuildsUrl()
        //{
        //    const string CodeFromTwitterAuthorize = "MyCode";

        //    auth.Callback = "https://www.example.com";
        //    auth.CredentialStore =
        //        new OAuth2CredentialStore
        //        {
        //            ClientID = "MyClientID",
        //            ClientSecret = "MyClientSecret",
        //            Scopes = new List<string>
        //            {
        //                "tweet.read", "users.read", "account.follows.read", "account.follows.write"
        //            }
        //        };

        //    string tokenUrl = (auth as OAuth2Authorizer).PrepareTokenUrl(CodeFromTwitterAuthorize);

        //    string[] parts = tokenUrl.Split('?');

        //    Dictionary<string, string> queryParms = null;
        //    if (parts.Length == 2)
        //        queryParms =
        //            (from parm in parts[1].Split('&')
        //             select parm.Split('='))
        //            .ToDictionary(key => key[0], val => val[1]);

        //    Assert.IsNotNull(queryParms);
        //    Assert.AreEqual(queryParms["code"], CodeFromTwitterAuthorize);
        //    Assert.AreEqual(queryParms["grant_type"], "authorization_code");
        //    Assert.AreEqual(queryParms["client_id"], "MyClientID");
        //    Assert.AreEqual(queryParms["redirect_uri"], "https://www.example.com");
        //    Assert.AreEqual(queryParms["code_verifier"], "challenge");
        //}

        [TestMethod]
        public void PrepareTokenParams_WithAllInputs_ReturnsDictionary()
        {
            const string CodeFromTwitterAuthorize = "MyCode";

            auth.Callback = "https://www.example.com";
            auth.CredentialStore =
                new OAuth2CredentialStore
                {
                    ClientID = "My Client ID",
                    ClientSecret = "MyClientSecret",
                    Scopes = new List<string>
                    {
                        "tweet.read", "users.read", "account.follows.read", "account.follows.write"
                    }
                };

            Dictionary<string, string> queryParms = (auth as OAuth2Authorizer).PrepareTokenParams(CodeFromTwitterAuthorize);

            Assert.IsNotNull(queryParms);
            Assert.AreEqual(queryParms["code"], CodeFromTwitterAuthorize);
            Assert.AreEqual(queryParms["grant_type"], "authorization_code");
            Assert.AreEqual(queryParms["client_id"], "My%20Client%20ID");
            Assert.AreEqual(queryParms["redirect_uri"], "https%3A%2F%2Fwww.example.com");
            Assert.AreEqual(queryParms["code_verifier"], "challenge");
        }
    }
}
