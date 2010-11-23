using System;
using System.Collections.Generic;

namespace LinqToTwitter
{
    public interface IOAuthTwitter
    {
        void AccessTokenGet(string authToken, string verifier, string accessTokenUrl, string callback, out string screenName, out string userID);
        string AuthorizationLinkGet(string requestToken, string authorizeUrl, string callback, bool readOnly, bool forceLogin);
        string TwitterParameterUrlEncode(string value);       
        void GetOAuthQueryString(HttpMethod method, string url, string callback, out string outUrl, out string queryString);
        string GetOAuthQueryStringForPost(string url);
        string OAuthConsumerKey { get; set; }
        string OAuthConsumerSecret { get; set; }
        string OAuthToken { get; set; }
        string OAuthTokenSecret { get; set; }
        string OAuthUserAgent { get; set; }
        string OAuthVerifier { get; set; }
        string oAuthWebRequest(HttpMethod method, string url, string postData, string callback);
        string WebRequest(HttpMethod method, string url, string authHeader, string postData);
        string WebResponseGet(System.Net.HttpWebRequest webRequest);
    }
}