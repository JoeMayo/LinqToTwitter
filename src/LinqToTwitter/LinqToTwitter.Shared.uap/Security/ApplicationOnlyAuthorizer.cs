using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter.Common;
using LinqToTwitter.Net;
using LitJson;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace LinqToTwitter
{
    public class ApplicationOnlyAuthorizer : AuthorizerBase, IAuthorizer
    {
        public string BasicToken { get; set; }
        public string BearerToken { get; set; }
        public string OAuth2Token { get; set; }
        public string OAuth2InvalidateToken { get; set; }

        public ApplicationOnlyAuthorizer()
        {
            OAuth2Token = "https://api.twitter.com/oauth2/token";
            OAuth2InvalidateToken = "https://api.twitter.com/oauth2/invalidate_token";
        }

        public async Task AuthorizeAsync()
        {
            EncodeCredentials();
            await GetBearerTokenAsync().ConfigureAwait(false);
        }

        public async Task InvalidateAsync()
        {
            EncodeCredentials();

            var req = new HttpRequestMessage(HttpMethod.Post, new Uri(OAuth2InvalidateToken));
            req.Headers.Add("Authorization", "Basic " + BasicToken);
            req.Headers.Add("User-Agent", UserAgent);
            req.Headers.Add("Expect", "100-continue");
            req.Content = new HttpStringContent("access_token=" + BearerToken, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");

            var baseFilter = new HttpBaseProtocolFilter
            {
                AutomaticDecompression = true
            };

            //var handler = new HttpClientHandler();
            //if (handler.SupportsAutomaticDecompression)
            //    handler.AutomaticDecompression = DecompressionMethods.GZip;
            //if (Proxy != null && handler.SupportsProxy)
            //    handler.Proxy = Proxy;

            using (var client = new HttpClient(baseFilter))
            {
                var msg = await client.SendRequestAsync(req);

                await TwitterErrorHandler.ThrowIfErrorAsync(msg);

                string response = await msg.Content.ReadAsStringAsync();

                var responseJson = JsonMapper.ToObject(response);
                BearerToken = responseJson.GetValue<string>("access_token"); 
            }
        }
  
        async Task GetBearerTokenAsync()
        {
            var req = new HttpRequestMessage(HttpMethod.Post, new Uri(OAuth2Token));
            req.Headers.Add("Authorization", "Basic " + BasicToken);
            req.Headers.Add("User-Agent", UserAgent);
            req.Headers.Add("Expect", "100-continue");
            req.Content = new HttpStringContent("grant_type=client_credentials", Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");

            var baseFilter = new HttpBaseProtocolFilter
            {
                AutomaticDecompression = true
            };

            //var handler = new HttpClientHandler();
            //if (handler.SupportsAutomaticDecompression)
            //    handler.AutomaticDecompression = DecompressionMethods.GZip;
            //if (Proxy != null && handler.SupportsProxy)
            //    handler.Proxy = Proxy;

            using (var client = new HttpClient(baseFilter))
            {
                var msg = await client.SendRequestAsync(req);

                await TwitterErrorHandler.ThrowIfErrorAsync(msg);

                string response = await msg.Content.ReadAsStringAsync();

                var responseJson = JsonMapper.ToObject(response);
                BearerToken = responseJson.GetValue<string>("access_token"); 
            }
        }

        internal void EncodeCredentials()
        {
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
