//using System;
//using System.Collections.Generic;
//using LinqToTwitter;

//namespace LinqToTwitterPcl.Tests.Common
//{
//    class OAuthTwitterMock : IOAuthTwitter
//    {
//        public void GetRequestTokenAsync(Uri oauthRequestTokenUrl, Uri oauthAuthorizeUrl, string twitterCallbackUrl, AuthAccessType authAccessType, bool forceLogin, Action<string> authorizationCallback, Action<TwitterAsyncResponse<object>> authenticationCompleteCallback)
//        {
//            authenticationCompleteCallback(new TwitterAsyncResponse<object>());
//        }

//        public void AccessTokenGet(string authToken, string verifier, string accessTokenUrl, string callback, out string screenName, out string userID)
//        {
//            throw new NotImplementedException();
//        }

//        public string AuthorizationLinkGet(string requestToken, string authorizeUrl, string callback, bool forceLogin, AuthAccessType authAccessType)
//        {
//            throw new NotImplementedException();
//        }

//        public void GetOAuthQueryString(HttpMethod method, Request url, string callback, out string outUrl, out string queryString)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetOAuthQueryStringForPost(Request url, IDictionary<string, string> postData)
//        {
//            throw new NotImplementedException();
//        }

//        public string OAuthConsumerKey
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public string OAuthConsumerSecret
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public string OAuthToken
//        {
//            get
//            {
//                return "123";
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public string OAuthTokenSecret
//        {
//            get
//            {
//                return "123";
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public string OAuthUserAgent
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public string OAuthVerifier
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public string oAuthWebRequest(HttpMethod method, string url, string postData, string callback, AuthAccessType authAccessType)
//        {
//            throw new NotImplementedException();
//        }

//        public string WebRequest(HttpMethod method, string url, string authHeader, IDictionary<string, string> postData)
//        {
//            throw new NotImplementedException();
//        }

//        public string WebResponseGet(System.Net.HttpWebRequest webRequest)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetOAuthHeader(Uri requestUrl, Uri callbackUrl)
//        {
//            throw new NotImplementedException();
//        }

//        public void GetAccessTokenAsync(string verifier, Uri oauthAccessTokenUrl, Uri twitterCallbackUrl, AuthAccessType authAccessType, Action<TwitterAsyncResponse<UserIdentifier>> authenticationCompleteCallback)
//        {
//            authenticationCompleteCallback(new TwitterAsyncResponse<UserIdentifier>());
//        }

//        public string ProxyUrl
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//            set
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public void GetAccessTokenAsync(string verifier, Uri oauthAccessTokenUrl, string twitterCallbackUrl, AuthAccessType authAccessType, Action<TwitterAsyncResponse<UserIdentifier>> authenticationCompleteCallback)
//        {
//            authenticationCompleteCallback(new TwitterAsyncResponse<UserIdentifier>());
//        }

//        public string FilterRequestParameters(Uri fullUrl)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetUrlParamValue(string queryString, string paramKey)
//        {
//            throw new NotImplementedException();
//        }

//        public void PostAccessToken(Request accessTokenUrl, IDictionary<string, string> postData, out string screenName, out string userID)
//        {
//            throw new NotImplementedException();
//        }

//        public string OAuthWebRequest(HttpMethod method, Request url, IDictionary<string, string> postData, string callback)
//        {
//            throw new NotImplementedException();
//        }

//        public void PostAccessTokenAsync(Request uri, IDictionary<string, string> postData, Action<TwitterAsyncResponse<UserIdentifier>> authorizationCompleteCallback)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
