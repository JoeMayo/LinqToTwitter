using System;
namespace LinqToTwitter
{
    public interface IOAuthTwitter
    {
        void AccessTokenGet(string authToken, string verifier, string accessTokenUrl, string callback, out string screenName, out string userID);
        string AuthorizationLinkGet(string requestToken, string authorizeUrl, string callback, bool readOnly, bool forceLogin);
        string GetOAuthAuthorizationHeader(string url, System.Collections.Generic.Dictionary<string, string> parameters, string callback);
        void GetOAuthQueryString(HttpMethod method, string url, string callback, out string outUrl, out string queryString);
        void GetOAuthQueryStringForPost(string url, System.Collections.Generic.Dictionary<string, string> parameters, string callback, out string outUrl, out string postData);
        string OAuthConsumerKey { get; set; }
        string OAuthConsumerSecret { get; set; }
        string OAuthToken { get; set; }
        string OAuthTokenSecret { get; set; }
        string OAuthUserAgent { get; set; }
        string oAuthWebRequest(HttpMethod method, string url, string postData, string callback);
        string WebRequest(HttpMethod method, string url, string authHeader, string postData);
        string WebResponseGet(System.Net.HttpWebRequest webRequest);
    }
}