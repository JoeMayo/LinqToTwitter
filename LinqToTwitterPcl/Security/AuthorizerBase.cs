using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LinqToTwitter.Security;

namespace LinqToTwitter
{
    public class AuthorizerBase
    {
        /// <summary>
        /// URL for OAuth Request Tokens
        /// </summary>
        public string OAuthRequestTokenUrl { get; set; }

        /// <summary>
        /// URL for OAuth authorization
        /// </summary>
        public string OAuthAuthorizeUrl { get; set; }

        /// <summary>
        /// URL for OAuth Access Tokens
        /// </summary>
        public string OAuthAccessTokenUrl { get; set; }

        /// <summary>
        /// Get/Set Credentials
        /// </summary>
        public ICredentialStore CredentialStore { get; set; }

        /// <summary>
        /// Force the user to enter their name/password when authorizing
        /// </summary>
        public bool ForceLogin { get; set; }

        /// <summary>
        /// Overrides read/write settings for application registered with Twitter
        /// </summary>
        public AuthAccessType AccessType { get; set; }

        /// <summary>
        /// Optional name to prefill when user visits the Twitter authorization screen
        /// </summary>
        public string PreFillScreenName { get; set; }

        /// <summary>
        /// User-Agent header string sent to Twitter to represent your application. Defaults to LINQ to Twitter.
        /// </summary>
        public string UserAgent { get; set; }

        IDictionary<string, string> parameters;
        public IDictionary<string, string> Parameters
        {
            get 
            {
                if (parameters == null)
                {
                    parameters = new Dictionary<string, string>();
                    parameters.Add("oauth_consumer_key", CredentialStore.ConsumerKey);
                    parameters.Add("oauth_token", CredentialStore.OAuthToken);
                }

                return parameters; 
            }
        }

        public AuthorizerBase(ICredentialStore store, bool forceLogin, AuthAccessType accessType, string prefillScreenName)
        {
            CredentialStore = store;
            ForceLogin = forceLogin;
            AccessType = accessType;

            if (string.IsNullOrWhiteSpace(UserAgent))
                UserAgent = TwitterContext.DefaultUserAgent;

            OAuthRequestTokenUrl = "https://api.twitter.com/oauth/request_token";
            OAuthAuthorizeUrl = "https://api.twitter.com/oauth/authorize";
            OAuthAccessTokenUrl = "https://api.twitter.com/oauth/access_token";
        }

        public async Task GetRequestTokenAsync(string callback)
        {
            if (string.IsNullOrWhiteSpace(callback))
                throw new ArgumentNullException("callback", "callback is required.");

            Parameters.Add("oauth_callback", callback);
            Parameters.Remove("oauth_token");

            if (AccessType != AuthAccessType.NoChange)
                Parameters.Add("x_auth_access_type", AccessType.ToString().ToLower());

            string response = await HttpGetAsync(OAuthRequestTokenUrl, Parameters);

            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentNullException("Empty response to request token response from Twitter.");

            UpdateCredentialsFromRequestTokenResponse(response);
        }

        public string PrepareAuthorizeUrl(bool forceLogin)
        {
            if (CredentialStore.OAuthToken == null)
                throw new InvalidOperationException("OAuthToken not set. Call GetRequestTokenAsync first and verify that OAuthToken is set.");

            string forceLoginParamString = forceLogin ? "&force_login=true" : "";

            string preFillScreenNameParamString = 
                !string.IsNullOrWhiteSpace(PreFillScreenName) ? "&screen_name=" + PreFillScreenName : "";

            return string.Format(
                "{0}?oauth_token={1}{2}{3}", 
                OAuthAuthorizeUrl, CredentialStore.OAuthToken, 
                forceLoginParamString, preFillScreenNameParamString);
        }

        public async Task GetAccessTokenAsync(IDictionary<string, string> accessTokenParams)
        {
            if (!accessTokenParams.ContainsKey("x_auth_mode") && !accessTokenParams.ContainsKey("oauth_verifier"))
                throw new ArgumentException("oauth_verifier is required, unless you're using xAuth.");

            foreach (var key in accessTokenParams.Keys)
                Parameters.Add(key, accessTokenParams[key]);

            Parameters.Remove("oauth_callback");

            string response = await HttpGetAsync(OAuthAccessTokenUrl, Parameters);

            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentNullException("Empty response to access token response from Twitter.");
            
            UpdateCredentialsWithAccessTokenResponse(response);
        }

        void UpdateCredentialsFromRequestTokenResponse(string response)
        {
            CredentialStore.OAuthToken =
                (from nameValPair in response.Split('&')
                 let pair = nameValPair.Split('=')
                 where pair[0] == "oauth_token"
                 select pair[1])
                .SingleOrDefault();

            Parameters.Add("oauth_token", CredentialStore.OAuthToken);
        }

        void UpdateCredentialsWithAccessTokenResponse(string response)
        {
            var responseParams =
                (from nameValPair in response.Split('&')
                 let pair = nameValPair.Split('=')
                 select new
                 {
                     Key = pair[0],
                     Value = pair[1]
                 })
                .ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value);

            if (responseParams["oauth_token"] != null)
                CredentialStore.OAuthToken = responseParams["oauth_token"];

            if (responseParams["oauth_token_secret"] != null)
                CredentialStore.OAuthTokenSecret = responseParams["oauth_token_secret"];

            if (responseParams["screen_name"] != null)
                CredentialStore.ScreenName = responseParams["screen_name"];

            if (responseParams["user_id"] != null)
                CredentialStore.UserID = responseParams["user_id"];
        }

        async Task<string> HttpGetAsync(string oauthUrl, IDictionary<string, string> parameters)
        {
            string consumerSecret = CredentialStore.ConsumerSecret ?? "";
            string oAuthTokenSecret = CredentialStore.OAuthTokenSecret ?? "";
            string authorizationString = 
                new OAuth().GetAuthorizationString(
                    HttpMethod.Get.ToString(), oauthUrl, parameters, consumerSecret, oAuthTokenSecret);

            var req = new HttpRequestMessage(HttpMethod.Get, oauthUrl);
            req.Headers.Add("Authorization", authorizationString);
            req.Headers.Add("User-Agent", UserAgent);
            req.Headers.ExpectContinue = false;

            var msg = await new HttpClient().SendAsync(req);

            return await msg.Content.ReadAsStringAsync();
        }
    }
}
