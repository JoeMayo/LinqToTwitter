using System;
namespace LinqToTwitter
{
    public interface IOAuthTwitter
    {
        void AccessTokenGet(string authToken, string accessTokenUrl);
        string AuthorizationLinkGet(string requestToken, string authorizeUrl);
        string GetOAuthAuthorizationHeader(string url, System.Collections.Generic.Dictionary<string, string> parameters);
        void GetOAuthQueryString(HttpMethod method, string url, out string outUrl, out string queryString);
        void GetOAuthQueryStringForPost(string url, System.Collections.Generic.Dictionary<string, string> parameters, out string outUrl, out string postData);
        string OAuthConsumerKey { get; set; }
        string OAuthConsumerSecret { get; set; }
        string OAuthParameterUrlEncode(string value);
        string OAuthToken { get; set; }
        string OAuthTokenSecret { get; set; }
        string OAuthUserAgent { get; set; }
        string oAuthWebRequest(HttpMethod method, string url, string postData);
        string WebRequest(HttpMethod method, string url, string postData);
        string WebResponseGet(System.Net.HttpWebRequest webRequest);
    }
}
