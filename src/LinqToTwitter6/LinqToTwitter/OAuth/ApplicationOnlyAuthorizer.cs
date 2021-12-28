using LinqToTwitter.Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    public class ApplicationOnlyAuthorizer : AuthorizerBase, IAuthorizer
    {
        public string? BasicToken { get; set; }
        public string? BearerToken { get; set; }
        public string OAuth2TokenUrl { get; set; }
        public string OAuth2InvalidateTokenUrl { get; set; }

        public ApplicationOnlyAuthorizer()
        {
            OAuth2TokenUrl = "https://api.twitter.com/oauth2/token";
            OAuth2InvalidateTokenUrl = "https://api.twitter.com/oauth2/invalidate_token";
        }

        public async Task AuthorizeAsync()
        {
            EncodeCredentials();
            await GetBearerTokenAsync().ConfigureAwait(false);
        }

        public async Task InvalidateAsync()
        {
            EncodeCredentials();

            var req = new HttpRequestMessage(HttpMethod.Post, OAuth2InvalidateTokenUrl);
            req.Headers.Add("Authorization", "Basic " + BasicToken);
            req.Headers.Add("User-Agent", UserAgent);
            req.Headers.ExpectContinue = false;
            req.Content = new StringContent("access_token=" + BearerToken, Encoding.UTF8, "application/x-www-form-urlencoded");

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip;
            if (Proxy != null && handler.SupportsProxy)
                handler.Proxy = Proxy;

            var client = new HttpClient(handler);

            var msg = await client.SendAsync(req).ConfigureAwait(false);

            await TwitterErrorHandler.ThrowIfErrorAsync(msg).ConfigureAwait(false);

            string response = await msg.Content.ReadAsStringAsync().ConfigureAwait(false);

            var responseJson = JsonDocument.Parse(response);
            BearerToken = responseJson.RootElement.GetProperty("access_token").GetString();
        }
  
        async Task GetBearerTokenAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Post, OAuth2TokenUrl);
            req.Headers.Add("Authorization", "Basic " + BasicToken);
            req.Headers.Add("User-Agent", UserAgent);
            req.Headers.ExpectContinue = false;
            req.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.GZip;
            if (Proxy != null && handler.SupportsProxy)
                handler.Proxy = Proxy;

            var client = new HttpClient(handler);

            var msg = await client.SendAsync(req).ConfigureAwait(false);

            await TwitterErrorHandler.ThrowIfErrorAsync(msg).ConfigureAwait(false);

            string response = await msg.Content.ReadAsStringAsync().ConfigureAwait(false);

            var responseJson = JsonDocument.Parse(response);
            BearerToken = responseJson.RootElement.GetProperty("access_token").GetString();
        }

        internal void EncodeCredentials()
        {
            if (CredentialStore == null)
                throw new ArgumentException($"{nameof(CredentialStore)} is required", nameof(CredentialStore));
            if (CredentialStore.ConsumerKey == null)
                throw new ArgumentException($"{nameof(CredentialStore.ConsumerKey)} is required", nameof(CredentialStore.ConsumerKey));
            if (CredentialStore.ConsumerSecret == null)
                throw new ArgumentException($"{nameof(CredentialStore.ConsumerSecret)} is required", nameof(CredentialStore.ConsumerSecret));

            string encodedConsumerKey = Url.PercentEncode(CredentialStore.ConsumerKey);
            string encodedConsumerSecret = Url.PercentEncode(CredentialStore.ConsumerSecret);

            string concatenatedCredentials = encodedConsumerKey + ":" + encodedConsumerSecret;

            byte[] credBytes = Encoding.UTF8.GetBytes(concatenatedCredentials);

            string base64Credentials = Convert.ToBase64String(credBytes);

            BasicToken = base64Credentials;
        }

        public override string GetAuthorizationString(string method, string oauthUrl, IDictionary<string, string> parameters)
        {
            return "Bearer " + BearerToken;
        }
    }
}
