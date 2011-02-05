using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;

namespace LinqToTwitterTests.Common
{
    class OAuthTwitterMock : IOAuthTwitter
    {
        public void GetRequestTokenAsync(Uri oauthRequestTokenUrl, Uri oauthAuthorizeUrl, string twitterCallbackUrl, bool forceLogin, Action<string> authorizationCallback, Action<TwitterAsyncResponse<object>> authenticationCompleteCallback)
        {
            authenticationCompleteCallback(new TwitterAsyncResponse<object>());
        }

        public void AccessTokenGet(string authToken, string verifier, string accessTokenUrl, string callback, out string screenName, out string userID)
        {
            throw new NotImplementedException();
        }

        public string AuthorizationLinkGet(string requestToken, string authorizeUrl, string callback, bool forceLogin)
        {
            throw new NotImplementedException();
        }

        public string TwitterParameterUrlEncode(string value)
        {
            throw new NotImplementedException();
        }

        public void GetOAuthQueryString(HttpMethod method, string url, string callback, out string outUrl, out string queryString)
        {
            throw new NotImplementedException();
        }

        public string GetOAuthQueryStringForPost(string url)
        {
            throw new NotImplementedException();
        }

        public string OAuthConsumerKey
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string OAuthConsumerSecret
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string OAuthToken
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string OAuthTokenSecret
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string OAuthUserAgent
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string OAuthVerifier
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string oAuthWebRequest(HttpMethod method, string url, string postData, string callback)
        {
            throw new NotImplementedException();
        }

        public string WebRequest(HttpMethod method, string url, string authHeader, string postData)
        {
            throw new NotImplementedException();
        }

        public string WebResponseGet(System.Net.HttpWebRequest webRequest)
        {
            throw new NotImplementedException();
        }

        public string GetOAuthHeader(Uri requestUrl, Uri callbackUrl)
        {
            throw new NotImplementedException();
        }

        public void GetAccessTokenAsync(string verifier, Uri oauthAccessTokenUrl, Uri twitterCallbackUrl, Action<TwitterAsyncResponse<UserIdentifier>> authenticationCompleteCallback)
        {
            authenticationCompleteCallback(new TwitterAsyncResponse<UserIdentifier>());
        }

        public string ProxyUrl
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void GetAccessTokenAsync(string verifier, Uri oauthAccessTokenUrl, string twitterCallbackUrl, Action<TwitterAsyncResponse<UserIdentifier>> authenticationCompleteCallback)
        {
            authenticationCompleteCallback(new TwitterAsyncResponse<UserIdentifier>());
        }

        public string FilterRequestParameters(Uri fullUrl)
        {
            throw new NotImplementedException();
        }

        public string GetUrlParamValue(string queryString, string paramKey)
        {
            throw new NotImplementedException();
        }

        public void PostAccessToken(string accessTokenUrl, string postData, out string screenName, out string userID)
        {
            throw new NotImplementedException();
        }

        public string OAuthWebRequest(HttpMethod method, string url, string postData, string callback)
        {
            throw new NotImplementedException();
        }

        public void PostAccessTokenAsync(Uri uri, string postData, Action<TwitterAsyncResponse<UserIdentifier>> authorizationCompleteCallback)
        {
            throw new NotImplementedException();
        }
    }
}
