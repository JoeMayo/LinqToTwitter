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

        [TestMethod]
        public void ParseRedirectResponse_WithGetParams_ParsesParams()
        {
            const string Response = @"GET /?state=zdso8cyW9aMoWE2Zs.20vx_f2KEcKtPAT2qQyzQmo3GzsrdWrkqfbD1BfNzxXWLQKLViblp7uPM5rM0KvwynCyn&code=OWIwZDZ5UUR1R3VsQ0FLX3Y1YURKZVdQRFJzdG1TcjM5aUd4NkQyQ3ZwbkhIOjE2NDMyNTEwNDQwOTU6MToxOmFjOjE HTTP/1.1
Host: 127.0.0.1:8599
Connection: keep-alive
sec-ch-ua: "" Not; A Brand"";v=""99"", ""Microsoft Edge"";v=""97"", ""Chromium"";v=""97""
sec - ch - ua - mobile: ?0
sec - ch - ua - platform: ""Windows""
Upgrade - Insecure - Requests: 1
DNT: 1
User - Agent: Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 97.0.4692.99 Safari / 537.36 Edg / 97.0.1072.69
Accept: text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,image / apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
Sec-Fetch-Site: cross-site
Sec-Fetch-Mode: navigate
Sec-Fetch-User: ?1
Sec-Fetch-Dest: document
Accept-Encoding: gzip, deflate, br
Accept-Language: en-US,en;q=0.9
Cookie: ai_user=YwWs2|2022-01-17T06:01:24.226Z

";

            (string code, string state) = auth.ParseRedirectResponse(Response);

            Assert.AreEqual("OWIwZDZ5UUR1R3VsQ0FLX3Y1YURKZVdQRFJzdG1TcjM5aUd4NkQyQ3ZwbkhIOjE2NDMyNTEwNDQwOTU6MToxOmFjOjE", code);
            Assert.AreEqual("zdso8cyW9aMoWE2Zs.20vx_f2KEcKtPAT2qQyzQmo3GzsrdWrkqfbD1BfNzxXWLQKLViblp7uPM5rM0KvwynCyn", state);
        }
    }
}
