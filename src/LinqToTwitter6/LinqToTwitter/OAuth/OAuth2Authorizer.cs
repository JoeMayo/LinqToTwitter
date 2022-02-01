using LinqToTwitter.Common;
using LinqToTwitter.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    public class OAuth2Authorizer : OAuth2AuthorizerBase, IOAuth2Authorizer
    {
        public const string CredentialStoreMessage = "You must assign the CredentialStore property (with required values).";

        public const string ValidCharacterString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
        public static readonly char[] ValidCharacters = ValidCharacterString.ToCharArray();

        public string AuthorizeUrl { get; set; }
        public string AccessTokenUrl { get; set; }
        public string RefreshTokenUrl { get; set; }
        public string RevokeTokenUrl { get; set; }

        public string? HtmlResponseString { get; set; }

        /// <summary>
        /// This is a hook where you can assign
        /// an <see cref="Action"/> to perform the technology
        /// specific redirection action.
        /// 
        /// The string passed as the <see cref="Action"/> paramter
        /// is the Twitter authorization URL.
        /// </summary>
        public Action<string>? GoToTwitterAuthorization { get; set; }

        public OAuth2Authorizer()
        {
            AuthorizeUrl = "https://twitter.com/i/oauth2/authorize";
            AccessTokenUrl = "https://api.twitter.com/2/oauth2/token";
            RefreshTokenUrl = "https://api.twitter.com/2/oauth2/token";
            RevokeTokenUrl = "https://api.twitter.com/2/oauth2/revoke";

            SupportsCompression = true;

            if (string.IsNullOrWhiteSpace(UserAgent))
                UserAgent = L2TKeys.DefaultUserAgent;
        }

        void HandleAuthorizationHeader(HttpRequestMessage req, string url)
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);

            bool isAuthorizationRequest(string url) => url.StartsWith(AuthorizeUrl);
            bool isPublicClient() => string.IsNullOrWhiteSpace(credStore.ClientSecret);

            if (isAuthorizationRequest(url) || isPublicClient())
                return;

            string authHeader =
                "Basic " +
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes(
                        $"{credStore.ClientID}:{credStore.ClientSecret}"));
            req.Headers.Add("Authorization", authHeader);
        }

        HttpRequestMessage GetHttpRequestMessage(HttpMethod method, string url)
        {
            var req = new HttpRequestMessage(method, url);
            req.Headers.Add("User-Agent", UserAgent);
            HandleAuthorizationHeader(req, url);
            return req;
        }

        public async Task<string> SendHttpAsync(HttpRequestMessage req)
        {
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

        public virtual async Task<string> GetAuthorizationCodeAsync(string authUrl)
        {
            var req = GetHttpRequestMessage(HttpMethod.Get, authUrl);
            return await SendHttpAsync(req).ConfigureAwait(false);
        }

        public virtual async Task<string> GetAccessTokenAsync(string code)
        {
            HttpRequestMessage req = GetHttpRequestMessage(HttpMethod.Post, AccessTokenUrl);

            string postParams = PrepareAccessTokenParams(code);

            req.Content = new StringContent(postParams, Encoding.UTF8, "application/x-www-form-urlencoded");

            return await SendHttpAsync(req).ConfigureAwait(false);
        }

        public async Task<string> RevokeTokenAsync()
        {
            HttpRequestMessage req = GetHttpRequestMessage(HttpMethod.Post, RevokeTokenUrl);

            string postParams = PrepareRevokeTokenParams();

            req.Content = new StringContent(postParams, Encoding.UTF8, "application/x-www-form-urlencoded");

            return await SendHttpAsync(req).ConfigureAwait(false);
        }

        public async Task<string> RefreshTokenAsync()
        {
            HttpRequestMessage req = GetHttpRequestMessage(HttpMethod.Post, RefreshTokenUrl);

            string postParams = PrepareRefreshTokenParams();

            req.Content = new StringContent(postParams, Encoding.UTF8, "application/x-www-form-urlencoded");

            string tokenResponse = await SendHttpAsync(req).ConfigureAwait(false);

            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);

            var tokenResponseJson = JsonDocument.Parse(tokenResponse).RootElement;
            credStore.AccessToken = tokenResponseJson.GetString("access_token");
            credStore.RefreshToken = tokenResponseJson.GetString("refresh_token");

            return tokenResponse;
        }

        string ToBase64Url(byte[] bytes)
        {
            return Convert
                .ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        public virtual string GenerateCodeChallenge()
        {
            var rand = new Random((int)DateTime.UtcNow.Ticks);
            int size = rand.Next(44, 127);
            var chars = new char[size];

            for (int i = 0; i < chars.Length; i++)
            {
                int charIndex = rand.Next(0, ValidCharacters.Length);
                chars[i] = ValidCharacters[charIndex];
            }

            return new string(chars);
        }

        public virtual string? HashCodeChallenge(string challenge)
        {
            byte[] challengeBytes = Encoding.ASCII.GetBytes(challenge);

            using var sha256 = SHA256.Create();

            byte[] challengeHashed = sha256.ComputeHash(challengeBytes);

            return ToBase64Url(challengeHashed);
        }

        public virtual string PrepareAuthorizeUrl(string? state = null)
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);

            string scopes = string.Join(' ', credStore.Scopes ?? new string[0]);

            credStore.CodeChallenge = GenerateCodeChallenge();
            string? hashedCodeChallenge = HashCodeChallenge(credStore.CodeChallenge);

            Func<string?, string> getState = (state) => string.IsNullOrWhiteSpace(state) ? "" : $"state={state}&";

            return 
                $"{AuthorizeUrl}?" +
                $"response_type=code&" +
                $"client_id={credStore.ClientID}&" +
                $"redirect_uri={credStore.RedirectUri}&" +
                $"scope={scopes}&" +
                $"{getState(state)}" +
                $"code_challenge={hashedCodeChallenge}&" +
                $"code_challenge_method=S256";
        }

        public virtual string PrepareAccessTokenParams(string? code)
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);
            if (string.IsNullOrWhiteSpace(credStore.ClientID))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.ClientID)} before calling AuthorizeAsync.", nameof(credStore.ClientID));
            if (string.IsNullOrWhiteSpace(credStore.RedirectUri))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.RedirectUri)} before calling AuthorizeAsync.", nameof(credStore.RedirectUri));
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(code)} before calling AuthorizeAsync.", nameof(code));
            if (string.IsNullOrWhiteSpace(credStore?.CodeChallenge))
                throw new InvalidOperationException("Internal LINQ to Twitter error - Missing Code Challenge!");

            return 
                $"code={Url.PercentEncode(code)}&" +
                $"grant_type=authorization_code&" +
                $"client_id={credStore.ClientID}&" +
                $"redirect_uri={credStore.RedirectUri}&" +
                $"code_verifier={credStore.CodeChallenge}";
        }

        public virtual string PrepareRefreshTokenParams()
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);
            if (string.IsNullOrWhiteSpace(credStore.ClientID))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.ClientID)} before calling AuthorizeAsync.", nameof(credStore.ClientID));
            if (string.IsNullOrWhiteSpace(credStore.RefreshToken))
                throw new ArgumentException(
                    "Your CredentialStore doesn't have a refresh token. " +
                    "Therefore, you are unable to update your token. " +
                    "To get a refresh token, add the 'offline.access' scope when initially authorizing.");

            return
                $"refresh_token={credStore.RefreshToken}&" +
                $"grant_type=refresh_token&" +
                $"client_id={credStore.ClientID}";
        }

        public virtual string PrepareRevokeTokenParams()
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);
            if (string.IsNullOrWhiteSpace(credStore.ClientID))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.ClientID)} before calling AuthorizeAsync.", nameof(credStore.ClientID));
            if (string.IsNullOrWhiteSpace(credStore.AccessToken))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.AccessToken)} before calling AuthorizeAsync.", nameof(credStore.AccessToken));

            return
                $"token={credStore.AccessToken}&" +
                $"client_id={credStore.ClientID}&" +
                $"token_type_hint=access_token";
        }

        public (string code, string state) ParseRedirectResponse(string response)
        {
            const string stateParam = "state=";
            const string codeParam = "code=";

            int stateStart = response.IndexOf(stateParam) + stateParam.Length;
            int stateEnd = response.IndexOf(codeParam, stateStart) - 1;
            int codeStart = response.IndexOf(codeParam, stateEnd) + codeParam.Length;
            int codeEnd = response.IndexOf(" ", codeStart);

            string state = response.Substring(stateStart, stateEnd-stateStart);
            string code = response.Substring(codeStart, codeEnd-codeStart);

            return (code, state);
        }

        /// <summary>
        /// Performs Basic Authorization (not implemented yet)
        /// </summary>
        public async Task AuthorizeAsync()
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);

            credStore.State = GenerateCodeChallenge();

            if (string.IsNullOrWhiteSpace(credStore.ClientID))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.ClientID)} before calling AuthorizeAsync.", nameof(credStore.ClientID));
            if (string.IsNullOrWhiteSpace(credStore.RedirectUri))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.RedirectUri)} before calling AuthorizeAsync.", nameof(credStore.RedirectUri));
            if (!credStore.Scopes?.Any() ?? false)
                throw new ArgumentException($"You must populate CredentialStore with {nameof(credStore.Scopes)} (permissions) before calling AuthorizeAsync.", nameof(credStore.Scopes));

            string authUrl = PrepareAuthorizeUrl(credStore.State);

            if (GoToTwitterAuthorization != null)
                GoToTwitterAuthorization(authUrl);

            Uri uri = new Uri(credStore.RedirectUri);
            string response = new OAuthListener(HtmlResponseString).Listen(uri.Host, uri.Port);

            (string code, string state) = ParseRedirectResponse(response);

            await CompleteAuthorizeAsync(code, state);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Starts the authorization process
        /// </summary>
        /// <param name="state">Any state of your choice for security checks durng <see cref="CompleteAuthorizeAsync(string)"/>.</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">For missing Credential Store</exception>
        /// <exception cref="ArgumentException">For missing ClientID or Scopes</exception>
        public async Task BeginAuthorizeAsync(string? state = null)
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);

            credStore.State = state;

            if (credStore.HasAllCredentials()) return;

            if (string.IsNullOrWhiteSpace(credStore.ClientID))
                throw new ArgumentException($"You must populate CredentialStore with a {nameof(credStore.ClientID)} before calling AuthorizeAsync.", nameof(credStore.ClientID));
            if (!credStore.Scopes?.Any() ?? false)
                throw new ArgumentException($"You must populate CredentialStore with {nameof(credStore.Scopes)} (permissions) before calling AuthorizeAsync.", nameof(credStore.Scopes));

            string authUrl = PrepareAuthorizeUrl(state);

            if (GoToTwitterAuthorization != null)
                GoToTwitterAuthorization(authUrl);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Completes the authorization process, which gets access token 
        /// (and optionally refresh token if you added the "offline.access" scope)
        /// </summary>
        /// <param name="code">Authorization code result from the <see cref="BeginAuthorizeAsync(string?)"/> process.</param>
        /// <exception cref="NullReferenceException">If missing CredentialStore</exception>
        /// <exception cref="TwitterQueryException">When state passed here doesn't equal state passed to <see cref="BeginAuthorizeAsync(string?)"/>, then possible CSRF attack.</exception>
        public async Task CompleteAuthorizeAsync(string code, string? state = null)
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);
            
            string expectedState = credStore.State ?? string.Empty;
            string returnedState = Uri.UnescapeDataString(state ?? string.Empty);

            if (expectedState != returnedState)
                throw new TwitterQueryException("Possible CSRF attack - State doesn't match. Check that the state sent in the begin request matches the state in the received request.");

            string tokenResponse = await GetAccessTokenAsync(code);

            var tokenResponseJson = JsonDocument.Parse(tokenResponse).RootElement;
            credStore.AccessToken = tokenResponseJson.GetString("access_token");
            credStore.RefreshToken = tokenResponseJson.GetString("refresh_token");
        }

        /// <summary>
        /// Called by LINQ to Twitter provider to build the Authorization header for each request.
        /// </summary>
        /// <exception cref="NullReferenceException">For missing CredentialStore</exception>
        public override string? GetAuthorizationString(string method, string oauthUrl, IDictionary<string, string> parameters)
        {
            if (CredentialStore is not IOAuth2CredentialStore credStore)
                throw new NullReferenceException(CredentialStoreMessage);

            return $"Bearer {credStore.AccessToken}";
        }
    }
}
