using System;
using System.Collections.Generic;

namespace LinqToTwitter
{
    public interface IOAuthTwitter
    {
        void AccessTokenGet(string authToken, string verifier, string accessTokenUrl, string callback, out string screenName, out string userID);

        /// <summary>
        /// Requests an access token from Twitter
        /// </summary>
        /// <param name="accessTokenUrl">Base url for request</param>
        /// <param name="postData">POST body params</param>
        /// <param name="screenName">Returns user's Twitter screen name</param>
        /// <param name="userID">Returns user's Twitter ID</param>
        void PostAccessToken(string accessTokenUrl, string postData, out string screenName, out string userID);

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

        /// <summary>
        /// URL for Silverlight Proxy
        /// </summary>
        string ProxyUrl { get; set; }

        string OAuthWebRequest(HttpMethod method, string url, string postData, string callback);
        string WebRequest(HttpMethod method, string url, string authHeader, string postData);
        string WebResponseGet(System.Net.HttpWebRequest webRequest);

        void GetRequestTokenAsync(
            Uri oauthRequestTokenUrl, 
            Uri oauthAuthorizeUrl, 
            string twitterCallbackUrl, 
            bool readOnly, 
            bool forceLogin, 
            Action<string> authorizationCallback, 
            Action<TwitterAsyncResponse<object>> authenticationCompleteCallback);

        void GetAccessTokenAsync(
            string verifier,
            Uri oauthAccessTokenUrl,
            string twitterCallbackUrl,
            Action<TwitterAsyncResponse<UserIdentifier>> authenticationCompleteCallback);

        string GetOAuthHeader(Uri requestUrl, Uri callbackUrl);

        /// <summary>
        /// Removes OAuth parameters from URL
        /// </summary>
        /// <param name="fullUrl">Raw url with OAuth parameters</param>
        /// <returns>Filtered url without OAuth parameters</returns>
        string FilterRequestParameters(Uri fullUrl);

        /// <summary>
        /// Extracts a value from a query string matching a key
        /// </summary>
        /// <param name="queryString">query string</param>
        /// <param name="paramKey">key to match val</param>
        /// <returns>value matching key</returns>
        string GetUrlParamValue(string queryString, string paramKey);

        void PostAccessTokenAsync(Uri uri, string postData, Action<TwitterAsyncResponse<UserIdentifier>> authorizationCompleteCallback);
    }
}