using LinqToTwitter.Common;
using LinqToTwitter.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    public class OAuth2Authorizer : OAuth2AuthorizerBase, IOAuth2Authorizer
    {
        public string AuthorizeUrl { get; set; }
        public string TokenUrl { get; set; }

        public string? BasicToken { get; set; }

        public string? BearerToken { get; set; }

        public string? CodeChallenge { get; set; }

        public OAuth2Authorizer()
        {
            AuthorizeUrl = "https://twitter.com/i/oauth2/authorize";
            TokenUrl = "https://api.twitter.com/2/oauth2/token";

            SupportsCompression = true;

            if (string.IsNullOrWhiteSpace(UserAgent))
                UserAgent = L2TKeys.DefaultUserAgent;
        }

        internal async Task<string> GetAuthorizationCodeAsync(string authUrl, bool isConfidentialClient, string? authHeader = null)
        {
            
            var req = new HttpRequestMessage(HttpMethod.Get, authUrl);
            if (isConfidentialClient)
                req.Headers.Add("Authorization", authHeader);
            req.Headers.Add("User-Agent", UserAgent);
            req.Headers.ExpectContinue = false;

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip;
            if (Proxy != null && handler.SupportsProxy)
                handler.Proxy = Proxy;

            var msg = await new HttpClient(handler).SendAsync(req).ConfigureAwait(false);

            await TwitterErrorHandler.ThrowIfErrorAsync(msg).ConfigureAwait(false);

            return await msg.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public async Task<string> GetAccessTokenAsync(Dictionary<string, string> criteria, bool isConfidentialClient, string? authHeader = null)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, TokenUrl);
            if (isConfidentialClient)
                req.Headers.Add("Authorization", authHeader);
            req.Headers.Add("User-Agent", UserAgent);
            req.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            req.Headers.ExpectContinue = false;
            string postParams =
                (from pair in criteria
                 select $"{pair.Key}={pair.Value}")
                .Aggregate((full, curr) => $"{full}&{curr}");

            req.Content = new StringContent(postParams, Encoding.UTF8, "application/x-www-form-urlencoded");

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip;
            if (Proxy != null && handler.SupportsProxy)
                handler.Proxy = Proxy;

            var client = new HttpClient(handler);

            var msg = await client.SendAsync(req).ConfigureAwait(false);

            await TwitterErrorHandler.ThrowIfErrorAsync(msg).ConfigureAwait(false);

            return await msg.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        public virtual string GenerateCodeChallenge()
        {
            var rand = new Random((int)DateTime.UtcNow.Ticks);
            int size = rand.Next(16, 32);
            var chars = new char[size];

            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)(rand.Next(0, 25) + 65);
            }

            return new string(chars);
        }

        public virtual string PrepareAuthorizeUrl()
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(
                    "The authorization process for confidential clients requires both a Client ID and Client Secret. " +
                    "You must assign the CredentialStore property (with required values) before calling AuthorizeAsync().");

            string scopes = string.Join(",", credStore.Scopes ?? new string[0]);

            CodeChallenge = GenerateCodeChallenge();
            string state = GenerateCodeChallenge();

            return 
                $"{AuthorizeUrl}?" +
                $"response_type=code&" +
                $"client_id={credStore.ClientID}&" +
                $"redirect_uri={Callback}&" +
                $"scope={Url.PercentEncode(scopes)}&" +
                $"state={Url.PercentEncode(state)}&" +
                $"code_challenge={Url.PercentEncode(CodeChallenge)}&" +
                $"code_challenge_method=plain";
        }

        //public virtual string PrepareTokenUrl(string code)
        //{
        //    if (CredentialStore is not IOAuth2CredentialStore credStore)
        //        throw new NullReferenceException(
        //            "The authorization process for confidential clients requires both a Client ID and Client Secret. " +
        //            "You must assign the CredentialStore property (with required values) before calling AuthorizeAsync().");

        //    return
        //        $"{TokenUrl}?" +
        //        $"code={code}&" +
        //        $"grant_type=authorization_code&" +
        //        $"client_id={credStore.ClientID}&" +
        //        $"redirect_uri={Callback}&" +
        //        $"code_verifier=challenge";
        //}

        public virtual Dictionary<string, string> PrepareTokenParams(string? code)
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(
                    "The authorization process for confidential clients requires both a Client ID and Client Secret. " +
                    "You must assign the CredentialStore property (with required values) before calling AuthorizeAsync().");

            return new Dictionary<string, string>
            {
                {"code", Url.PercentEncode(code)},
                {"grant_type", "authorization_code" },
                {"client_id",  Url.PercentEncode(credStore.ClientID) },
                {"redirect_uri", Url.PercentEncode(Callback)},
                {"code_verifier", "challenge" }
            };
        }

        /// <summary>
        /// Performs Basic Authorization
        /// </summary>
        /// <exception cref="NullReferenceException">You must assign an IOAuth2CredentialStore to the CredentialStore.</exception>
        /// <exception cref="ArgumentException">The CredentialStore must have both ClientID and ClientSecret.</exception>
        public async Task AuthorizeAsync()
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(
                    "The authorization process for confidential clients requires both a Client ID and Client Secret. " +
                    "You must assign the CredentialStore property (with required values) before calling AuthorizeAsync().");

            if (string.IsNullOrWhiteSpace(credStore.ClientID))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.ClientID)} before calling AuthorizeAsync.", nameof(credStore.ClientID));
            if (string.IsNullOrWhiteSpace(credStore.ClientSecret))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.ClientSecret)} before calling AuthorizeAsync.", nameof(credStore.ClientSecret));

            string authHeader = 
                "Basic " +
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        $"{credStore.ClientID}:{credStore.ClientSecret}"));

            string authUrl = PrepareAuthorizeUrl();

            string codeResponse = await GetAuthorizationCodeAsync(authUrl, false, authHeader);

            var codeResponseJson = JsonDocument.Parse(codeResponse);
            BearerToken = codeResponseJson.RootElement.GetProperty("code").GetString();

            Dictionary<string, string> headers = PrepareTokenParams(BearerToken);

            string tokenResponse = await GetAccessTokenAsync(headers, true, authHeader);

            var tokenResponseJson = JsonDocument.Parse(tokenResponse);
            BearerToken = tokenResponseJson.RootElement.GetProperty("code").GetString();

            await Task.CompletedTask;
        }

        public async Task BeginAuthorizeAsync()
        {
            var credStore = CredentialStore as IOAuth2CredentialStore;

            if (credStore == null)
                throw new NullReferenceException(
                    "The authorization process requires both a Client ID and a set of scopes (permissions). " +
                    "You must assign the CredentialStore property (with required values) before calling BeginAuthorizeAsync().");

            if (credStore.HasAllCredentials()) return;

            if (string.IsNullOrWhiteSpace(credStore.ClientID))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.ClientID)} before calling AuthorizeAsync.", nameof(credStore.ClientID));
            if (credStore.Scopes?.Any() ?? false)
                throw new ArgumentException($"You must populate CredentialStore with {nameof(credStore.Scopes)} (permissions) before calling AuthorizeAsync.", nameof(credStore.Scopes));

            string authUrl = PrepareAuthorizeUrl();
        }

        public async Task CompleteAuthorizeAsync(string code)
        {
            Dictionary<string, string> headers = PrepareTokenParams(code);
            
        }

        public async Task LogoutAsync()
        {
        }

        public async Task RefreshAsync()
        {
        }

        public override string? GetAuthorizationString(string method, string oauthUrl, IDictionary<string, string> parameters)
        {
            return $"Bearer {BearerToken}";
        }
    }
}
