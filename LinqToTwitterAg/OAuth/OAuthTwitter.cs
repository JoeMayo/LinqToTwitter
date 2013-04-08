/***********************************************************
 * Credits:
 * 
 * Eran Sandler -
 * OAuthBase Class
 * 
 * http://oauth.googlecode.com/svn/code/csharp/
 * 
 * Shannon Whitley -
 * Example of how to use modified version of
 * Eran Sandler's OAuthBase class in C#
 * 
 * http://www.voiceoftech.com/swhitley/?p=681
 * 
 * Joe Mayo -
 * 
 * Modified 5/17/09
 ***********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

#if !SILVERLIGHT && !L2T_PCL
using System.IO.Compression;
#else
using Ionic.Zlib;
#endif
#if SILVERLIGHT && !WINDOWS_PHONE
using System.Windows.Browser;
#elif !SILVERLIGHT && !WINDOWS_PHONE && !NETFX_CORE
#endif

namespace LinqToTwitter
{
    /// <summary>
    /// helps perform OAuth Authorization for LINQ to Twitter
    /// </summary>
    public class OAuthTwitter : OAuthBase, IOAuthTwitter
    {
        /// <summary>
        /// user agent header sent to Twitter
        /// </summary>
        public string OAuthUserAgent { get; set; }

        /// <summary>
        /// Consumer Key
        /// </summary>
        public string OAuthConsumerKey { get; set; }

        /// <summary>
        /// Consumer Secret
        /// </summary>
        public string OAuthConsumerSecret { get; set; }

        /// <summary>
        /// OAuth Token
        /// </summary>
        public string OAuthToken { get; set; }

        /// <summary>
        /// OAuth Verifier
        /// </summary>
        public string OAuthVerifier { get; set; }

        /// <summary>
        /// OAuth Token Secret
        /// </summary>
        public string OAuthTokenSecret { get; set; }

        /// <summary>
        /// URL for Silverlight Proxy
        /// </summary>
        public string ProxyUrl { get; set; }

        /// <summary>
        /// Get the link to Twitter's authorization page for this application.
        /// </summary>
        /// <param name="readOnly">true for read-only, otherwise read/Write</param>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLinkGet(string requestToken, string authorizeUrl, string callback, bool forceLogin, AuthAccessType authAccessToken)
        {
            var request = new Request(requestToken);

            if (authAccessToken != AuthAccessType.NoChange)
            {
                request.RequestParameters.Add(
                    new QueryParameter(OAuthXAccessTypeKey, authAccessToken.ToString().ToLower()));
            }

            var response = OAuthWebRequest(HttpMethod.GET, request, null, callback);

            return PrepareAuthorizeUrl(authorizeUrl, forceLogin, response);
        }

        string PrepareAuthorizeUrl(string authorizeUrl, bool forceLogin, string response)
        {
            string authUrl = string.Empty;

            if (response.Length > 0)
            {
                var prefixChar = "?";

                string oAuthToken =
                    (from nameValPair in response.Split('&')
                     let pair = nameValPair.Split('=')
                     where pair[0] == "oauth_token"
                     select pair[1])
                    .SingleOrDefault();

                if (oAuthToken != null)
                {
                    OAuthToken = oAuthToken;
                    authUrl = authorizeUrl + "?oauth_token=" + oAuthToken;
                    prefixChar = "&";
                }

                if (forceLogin)
                {
                    authUrl += prefixChar + "force_login=true";
                }
            }
            return authUrl;
        }

        /// <summary>
        /// Requests an access token from Twitter
        /// </summary>
        /// <param name="accessTokenUrl">Base url for request</param>
        /// <param name="postData">POST body params</param>
        /// <param name="screenName">Returns user's Twitter screen name</param>
        /// <param name="userID">Returns user's Twitter ID</param>
        public void PostAccessToken(Request request, IDictionary<string, string> postData, out string screenName, out string userID)
        {
            screenName = string.Empty;
            userID = string.Empty;
            var response = OAuthWebRequest(HttpMethod.POST, request, postData, string.Empty);

            ProcessAccessTokenResponse(ref screenName, ref userID, response);
        }

        /// <summary>
        /// Exchange the request token for an access token.
        /// </summary>
        /// <param name="authToken">The oauth_token is supplied by Twitter's authorization page following the callback.</param>
        public void AccessTokenGet(string authToken, string verifier, string accessTokenUrl, string callback, out string screenName, out string userID)
        {
            screenName = string.Empty;
            userID = string.Empty;

            OAuthToken = authToken;
            OAuthVerifier = verifier;
            var request = new Request(accessTokenUrl);
            var response = OAuthWebRequest(HttpMethod.GET, request, null, callback);

            ProcessAccessTokenResponse(ref screenName, ref userID, response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="screenName"></param>
        /// <param name="userID"></param>
        /// <param name="response"></param>
        private void ProcessAccessTokenResponse(ref string screenName, ref string userID, string response)
        {
            if (response.Length > 0)
            {
                var qs =
                    (from nameValPair in response.Split('&')
                     let pair = nameValPair.Split('=')
                     select new
                     {
                         Key = pair[0],
                         Value = pair[1]
                     })
                    .ToDictionary(
                        pair => pair.Key,
                        pair => pair.Value);

                if (qs["oauth_token"] != null)
                {
                    OAuthToken = qs["oauth_token"];
                }

                if (qs["oauth_token_secret"] != null)
                {
                    OAuthTokenSecret = qs["oauth_token_secret"];
                }

                if (qs["screen_name"] != null)
                {
                    screenName = qs["screen_name"];
                }

                if (qs["user_id"] != null)
                {
                    userID = qs["user_id"];
                }
            }
        }

        /// <summary>
        /// returns a query string for an OAuth request
        /// </summary>
        /// <param name="url">Twitter query</param>
        /// <returns>Query string with OAuth parameters</returns>
        public void GetOAuthQueryString(HttpMethod method, Request request, string callback, out string outUrl, out string queryString)
        {
            string nonce = GenerateNonce();
            string timeStamp = GenerateTimeStamp();

            //Generate Signature
            string sig = GenerateSignature(request,
                OAuthConsumerKey,
                OAuthConsumerSecret,
                OAuthToken,
                OAuthTokenSecret,
                callback == "oob" ? this.OAuthVerifier : null,
                callback,
                method.ToString(),
                timeStamp,
                nonce,
                OAuthSignatureTypes.Hmacsha1,
                out outUrl,
                out queryString);

            queryString += "&oauth_signature=" + BuildUrlHelper.UrlEncode(sig);
        }

        /// <summary>
        /// processes POST request parameters
        /// </summary>
        /// <param name="reqest">request having endpoint without query string and any query parameters</param>
        /// <param name="args">extra query-string parameters</param>
        public string GetOAuthQueryStringForPost(Request request, IDictionary<string, string> postData)
        {
            OAuthVerifier = null;

            string outUrl;
            string queryString;
            GetOAuthQueryString(HttpMethod.POST, request, string.Empty, out outUrl, out queryString);

            const int Key = 0;
            const int Value = 1;

            var headerItems =
                from param in queryString.Split('&')
                let keyValPair = param.Split('=')
                select
                    keyValPair[Key] +
                    "=\"" +
                    keyValPair[Value] +
                    "\"";

            return "OAuth " + string.Join(",", headerItems.ToArray());
        }

        internal string PrepareAuthHeader(string authHeader)
        {
            var encodedParams =
                string.Join(
                    ",",
                    (from param in authHeader.Split('&')
                     let args = param.Split('=')
                     where !args[0].Contains("realm")
                     select args[0] + "=\"" + args[1] + "\"")
                    .ToArray());

            return "OAuth " + encodedParams;
        }

        internal string PrepareAuthHeader(string authHeader, Request request)
        {
            var reqParams = request.RequestParameters.Select(rp => rp.Name);
            var encodedParams =
                string.Join(
                    ",",
                    (from param in authHeader.Split('&')
                     let args = param.Split('=')
                     where !args[0].Contains("realm") &&
                           !reqParams.Contains(args[0])
                     select args[0] + "=\"" + args[1] + "\"")
                    .ToArray());

            return "OAuth " + encodedParams;
        }

        private string GatherPostData(IDictionary<string, string> postData)
        {
            var queryParams = new StringBuilder();

            if (postData != null && postData.Count > 0)
            {
                foreach (var entry in postData)
                {
                    queryParams
                        .Append(entry.Key)
                        .Append('=')
                        .Append(BuildUrlHelper.UrlEncode(entry.Value))
                        .Append('&');
                }

                queryParams.Length--;   // discard trailing &
            }

            return queryParams.ToString();
        }

        /// <summary>
        /// Submit a web request using oAuth.
        /// </summary>
        /// <param name="method">GET or POST</param>
        /// <param name="request">Request details</param>
        /// <param name="postData">Data to post (querystring format)</param>
        /// <returns>The web server response.</returns>
        public string OAuthWebRequest(HttpMethod method, Request request, IDictionary<string, string> postData, string callback)
        {
            //Setup postData for signing.
            //Add the postData to the querystring.
            var url = request.FullUrl;

            if (method == HttpMethod.POST)
            {
                if (postData != null && postData.Count > 0)
                {
                    foreach (var postEntry in postData)
                        if (!string.IsNullOrEmpty(postEntry.Value))
                            request.RequestParameters.Add(new QueryParameter(postEntry.Key, postEntry.Value));
                }
            }

            string nonce = GenerateNonce();
            string timeStamp = GenerateTimeStamp();
            string outUrl;
            string querystring;

            //Generate Signature
            string sig = GenerateSignature(request,
                OAuthConsumerKey,
                OAuthConsumerSecret,
                OAuthToken,
                OAuthTokenSecret,
                OAuthVerifier,
                callback, 
                method.ToString(),
                timeStamp,
                nonce,
                OAuthSignatureTypes.Hmacsha1,
                out outUrl,
                out querystring);

            querystring += "&oauth_signature=" + BuildUrlHelper.UrlEncode(sig);

            var ret = WebRequest(method, url, querystring, postData);
            return ret;
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequest(HttpMethod method, string url, string authHeader, IDictionary<string, string> postData)
        {
            string responseData = "";

            var webRequest = System.Net.WebRequest.Create(ProxyUrl + url) as HttpWebRequest;
            webRequest.Method = method.ToString();
#if !SILVERLIGHT && !NETFX_CORE
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = OAuthUserAgent; 
#endif
            webRequest.Headers[HttpRequestHeader.Authorization] = PrepareAuthHeader(authHeader);
            webRequest.Headers[HttpRequestHeader.AcceptEncoding] = "deflate,gzip";

            if (method == HttpMethod.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";
                var postBody = GatherPostData(postData);
                byte[] postDataBytes = Encoding.UTF8.GetBytes(postBody);

#if SILVERLIGHT
                webRequest.BeginGetRequestStream(
                    new AsyncCallback(
                        ar =>
                        {
                            using (var requestStream = webRequest.EndGetRequestStream(ar))
                            {
                                requestStream.Write(postDataBytes, 0, postDataBytes.Length);
                            }
                        }), null);
#else
                Exception asyncException = null;

                using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
                {
                    webRequest.BeginGetRequestStream(
                        new AsyncCallback(
                            ar =>
                            {
                                try
                                {
                                    using (var requestStream = webRequest.EndGetRequestStream(ar))
                                    {
                                        requestStream.Write(postDataBytes, 0, postDataBytes.Length);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    asyncException = ex;
                                }
                                finally
                                {
                                    resetEvent.Set();
                                }
                            }), null);

                    resetEvent.WaitOne();
                }

                if (asyncException != null)
	            {
                    throw asyncException;
	            }
#endif
            }

#if !SILVERLIGHT
            responseData = WebResponseGet(webRequest); 
#endif

            webRequest = null;

            return responseData;
        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public string WebResponseGet(HttpWebRequest webRequest)
        {
            string responseData = "";

            Exception asyncException = null;

            using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
            {
                HttpWebResponse res = null;

                webRequest.BeginGetResponse(
                    new AsyncCallback(
                        ar =>
                        {
                            try
                            {
                                res = webRequest.EndGetResponse(ar) as HttpWebResponse;
                                using (var respStream = res.GetResponseStream())
                                {
                                    string contentEncoding = res.Headers["Content-Encoding"] ?? "";
                                    if (contentEncoding.ToLower().Contains("gzip"))
                                    {
                                        using (var gzip = new GZipStream(respStream, CompressionMode.Decompress))
                                        {
                                            using (var reader = new StreamReader(gzip))
                                            {
                                                responseData = reader.ReadToEnd();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        using (var respReader = new StreamReader(respStream))
                                        {
                                            responseData = respReader.ReadToEnd();
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                asyncException = ex;
                            }
                            finally
                            {
                                resetEvent.Set();
                            }
                        }), null);

                resetEvent.WaitOne();
            }

            if (asyncException != null)
            {
                throw asyncException;
            }

            return responseData;
        }

        /// <summary>
        /// Extracts a value from a query string matching a key
        /// </summary>
        /// <param name="queryString">query string</param>
        /// <param name="paramKey">key to match val</param>
        /// <returns>value matching key</returns>
        public string GetUrlParamValue(string queryString, string paramKey)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return null;
            }

            string[] keyValPairs = queryString.TrimStart('?').Split('&');

            var paramVal =
                (from keyValPair in keyValPairs
                 let pair = keyValPair.Split('=')
                 let key = pair[0]
                 let val = pair[1]
                 where key == paramKey
                 select val)
                .SingleOrDefault();

            return paramVal;
        }

        /// <summary>
        /// Removes OAuth parameters from URL
        /// </summary>
        /// <param name="fullUrl">Raw url with OAuth parameters</param>
        /// <returns>Filtered url without OAuth parameters</returns>
        public string FilterRequestParameters(Uri fullUrl)
        {
            const int Domain = 0;
            const int Params = 1;

            if (fullUrl == null)
            {
                return string.Empty;
            }

            string filteredParams = string.Empty;

            string[] urlParts = fullUrl.ToString().Split('?');

            if (urlParts.Length == 2 && !string.IsNullOrEmpty(urlParts[Params]))
            {
                filteredParams =
                    string.Join(
                        "&",
                        (from param in urlParts[Params].Split('&')
                         let args = param.Split('=')
                         where !args[0].StartsWith("oauth_")
                         select param)
                        .ToArray());
            }

            return urlParts[Domain] + (filteredParams == string.Empty ? string.Empty : "?" + filteredParams);
        }

        public HttpWebRequest GetHttpGetRequest(Uri oauthUrl, string callbackUrl, AuthAccessType authAccessType)
        {
            string signedUrl = null;
            string queryString = null;
            string callback = callbackUrl ?? string.Empty;
            var request = new Request(oauthUrl.ToString());
            GetOAuthQueryString(HttpMethod.GET, request, callback, out signedUrl, out queryString);
            
            var finalUrl = ProxyUrl + request.FullUrl;

            var req = System.Net.WebRequest.Create(finalUrl) as HttpWebRequest;
            req.Headers[HttpRequestHeader.Authorization] = PrepareAuthHeader(queryString);
            req.Method = HttpMethod.GET.ToString();

#if !SILVERLIGHT || WINDOWS_PHONE
            req.Headers[HttpRequestHeader.AcceptEncoding] = "deflate,gzip";
#endif

#if !SILVERLIGHT && !NETFX_CORE
            req.ServicePoint.Expect100Continue = false;
            req.UserAgent = OAuthUserAgent;
#endif
            return req;
        }

        public HttpWebRequest GetHttpPostRequest(Uri oauthUrl)
        {
            string url = oauthUrl.ToString();
            var request = new Request(url);
            string oauthSig = GetOAuthQueryStringForPost(request, null);
            string baseUrl = url.Split('?')[0];

            var finalUrl = ProxyUrl + baseUrl;

            var req = System.Net.WebRequest.Create(finalUrl) as HttpWebRequest;
            req.Headers[HttpRequestHeader.Authorization] = oauthSig;
            req.Method = HttpMethod.POST.ToString();

#if !SILVERLIGHT && !NETFX_CORE
            req.ServicePoint.Expect100Continue = false;
            req.UserAgent = OAuthUserAgent;
#endif
            return req;
        }

#if OLDSCHOOL
        /// <summary>
        /// Gets a signed OAuth Header
        /// </summary>
        /// <param name="url">Request Url</param>
        /// <param name="callbackUrl">Callback Url</param>
        /// <returns></returns>
        public string GetOAuthHeader(Request request, Uri callbackUrl)
        {
            string outUrl = string.Empty;
            string queryString = string.Empty;
            string nonce = this.GenerateNonce();
            string timeStamp = this.GenerateTimeStamp();
            string callback = callbackUrl == null ? string.Empty : callbackUrl.ToString();

            //Generate Signature
            string sig = this.GenerateSignature(request,
                this.OAuthConsumerKey,
                this.OAuthConsumerSecret,
                this.OAuthToken,
                this.OAuthTokenSecret,
                this.OAuthVerifier,
                callback,
                HttpMethod.GET.ToString(),
                timeStamp,
                nonce,
                OAuthSignatureTypes.HMACSHA1,
                out outUrl,
                out queryString);

            queryString += "&oauth_signature=" + HttpUtility.UrlEncode(sig);

            return PrepareAuthHeader(queryString);
        }
#endif

        /// <summary>
        /// Asynchronous request for OAuth request token
        /// </summary>
        /// <param name="oauthRequestTokenUrl">Url to make initial request on</param>
        /// <param name="oauthAuthorizeUrl">Url to send user to for authorization</param>
        /// <param name="twitterCallbackUrl">Url for Twitter to redirect to after authorization (null for Pin authorization)</param>
        /// <param name="forceLogin">Should user be forced to log in to authorize this app</param>
        /// <param name="authorizationCallback">Lambda to let program perform redirect to authorization page</param>
        /// <param name="authenticationCompleteCallback">Lambda to invoke to let user know when authorization completes</param>
        public void GetRequestTokenAsync(
            Uri oauthRequestTokenUrl, 
            Uri oauthAuthorizeUrl, 
            string twitterCallbackUrl, 
            AuthAccessType authAccessType,
            bool forceLogin, 
            Action<string> authorizationCallback, 
            Action<TwitterAsyncResponse<object>> authenticationCompleteCallback)
        {
            var req = GetHttpGetRequest(oauthRequestTokenUrl, twitterCallbackUrl, authAccessType);

            req.BeginGetResponse(
                new AsyncCallback(
                    ar =>
                    {
                        var twitterResponse = new TwitterAsyncResponse<object>();

                        try
                        {
                            var res = req.EndGetResponse(ar) as HttpWebResponse;

                            string requestTokenResponse = GetHttpResponse(res);

                            string authorizationUrl = PrepareAuthorizeUrl(oauthAuthorizeUrl.ToString(), forceLogin, requestTokenResponse);

                            authorizationCallback(authorizationUrl);
                        }
                        catch (TwitterQueryException tqe)
                        {
                            twitterResponse.Status = TwitterErrorStatus.TwitterApiError;
                            twitterResponse.Message = "Error while communicating with Twitter. Please see Error property for details.";
                            twitterResponse.Exception = tqe;
                        }
                        catch (Exception ex)
                        {
                            twitterResponse.Status = TwitterErrorStatus.TwitterApiError;
                            twitterResponse.Message = "Error during LINQ to Twitter processing. Please see Error property for details.";
                            twitterResponse.Exception = ex;
                        }
                        finally
                        {
                            if (authenticationCompleteCallback != null)
                            {
                                authenticationCompleteCallback(twitterResponse); 
                            }
                        }
                    }), null);
        }
  
        /// <summary>
        /// Asynchronous request for OAuth access token
        /// </summary>
        /// <param name="verifier">Verification token provided by Twitter after user authorizes (7-digit number for Pin authorization too)</param>
        /// <param name="oauthAccessTokenUrl">Access token URL</param>
        /// <param name="twitterCallbackUrl">URL for your app that Twitter redirects to after authorization (null for Pin authorization)</param>
        /// <param name="authenticationCompleteCallback">Callback to application after response completes (contains UserID and ScreenName)</param>
        public void GetAccessTokenAsync(
            string verifier,
            Uri oauthAccessTokenUrl,
            string twitterCallbackUrl,
            AuthAccessType authAccessType,
            Action<TwitterAsyncResponse<UserIdentifier>> authenticationCompleteCallback)
        {
            OAuthVerifier = verifier;

            var req = GetHttpGetRequest(oauthAccessTokenUrl, twitterCallbackUrl, authAccessType);

            req.BeginGetResponse(
                new AsyncCallback(
                    ar =>
                    {
                        string screenName = string.Empty;
                        string userID = string.Empty;

                        var twitterResponse = new TwitterAsyncResponse<UserIdentifier>();

                        try
                        {

                            var res = req.EndGetResponse(ar) as HttpWebResponse;

                            string accessTokenResponse = GetHttpResponse(res);

                            ProcessAccessTokenResponse(ref screenName, ref userID, accessTokenResponse);
                        }
                        catch (TwitterQueryException tqe)
                        {
                            twitterResponse.Status = TwitterErrorStatus.TwitterApiError;
                            twitterResponse.Message = "Error while communicating with Twitter. Please see Error property for details.";
                            twitterResponse.Exception = tqe;
                        }
                        catch (Exception ex)
                        {
                            twitterResponse.Status = TwitterErrorStatus.TwitterApiError;
                            twitterResponse.Message = "Error during LINQ to Twitter processing. Please see Error property for details.";
                            twitterResponse.Exception = ex;
                        }
                        finally
                        {
                            if (authenticationCompleteCallback != null)
                            {
                                twitterResponse.State =
                                    new UserIdentifier
                                    {
                                        ID = userID,
                                        UserID = userID,
                                        ScreenName = screenName
                                    };
                                authenticationCompleteCallback(twitterResponse); 
                            }
                        }
                    }), null);

        }

        string GetHttpResponse(HttpWebResponse res)
        {
            const int WorkingBufferSize = 1024;
            string requestTokenResponse = string.Empty;
            string contentEncoding = string.Empty;

            using (var respStream = res.GetResponseStream())
            {
#if !SILVERLIGHT || WINDOWS_PHONE
                contentEncoding = res.Headers["Content-Encoding"] ?? "";
#endif
                if (contentEncoding.ToLower().Contains("gzip"))
                {
                    using (var gzip = new GZipStream(respStream, CompressionMode.Decompress))
                    {
                        using (var memStr = new MemoryStream())
                        {
                            byte[] buffer = new byte[WorkingBufferSize];
                            int n;
                            while ((n = gzip.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                memStr.Write(buffer, 0, n);
                            }
                            memStr.Position = 0;
                            using (var strmRdr = new StreamReader(memStr))
                            {
                                requestTokenResponse = strmRdr.ReadToEnd();
                            }
                        }
                    }
                }
                else
                {
                    using (var respReader = new StreamReader(respStream))
                    {
                        requestTokenResponse = respReader.ReadToEnd();
                    }
                }
            }

            return requestTokenResponse;
        }

        /// <summary>
        /// Posts asynchronously to Twitter for access token
        /// </summary>
        /// <param name="accessTokenUrl">Access token URL</param>
        /// <param name="postData">Post info</param>
        /// <param name="authorizationCompleteCallback">Invoked when request finishes</param>
        public void PostAccessTokenAsync(Request request, IDictionary<string, string> postData, Action<TwitterAsyncResponse<UserIdentifier>> authenticationCompleteCallback)
        {
            var accessTokenUrl = new Uri(request.FullUrl);
            var req = GetHttpPostRequest(accessTokenUrl);

            req.ContentType = "application/x-www-form-urlencoded";

            var postBody = GatherPostData(postData);
            byte[] postDataBytes = Encoding.UTF8.GetBytes(postBody);

#if SILVERLIGHT
                req.BeginGetRequestStream(
                    new AsyncCallback(
                        reqAr =>
                        {
                            using (var requestStream = req.EndGetRequestStream(reqAr))
                            {
                                requestStream.Write(postDataBytes, 0, postDataBytes.Length);
                            }

                            req.BeginGetResponse(
                                new AsyncCallback(
                                    resAr =>
                                    {
                                        string screenName = string.Empty;
                                        string userID = string.Empty;

                                        var twitterResponse = new TwitterAsyncResponse<UserIdentifier>();

                                        try
                                        {
                                            string accessTokenResponse = string.Empty;

                                            var res = req.EndGetResponse(resAr) as HttpWebResponse;

                                            using (var respStream = res.GetResponseStream())
                                            using (var respReader = new StreamReader(respStream))
                                            {
                                                accessTokenResponse = respReader.ReadToEnd();
                                            }

                                            ProcessAccessTokenResponse(ref screenName, ref userID, accessTokenResponse);
                                        }
                                        catch (TwitterQueryException tqe)
                                        {
                                            twitterResponse.Status = TwitterErrorStatus.TwitterApiError;
                                            twitterResponse.Message = "Error while communicating with Twitter. Please see Error property for details.";
                                            twitterResponse.Exception = tqe;
                                        }
                                        catch (Exception ex)
                                        {
                                            twitterResponse.Status = TwitterErrorStatus.TwitterApiError;
                                            twitterResponse.Message = "Error during LINQ to Twitter processing. Please see Error property for details.";
                                            twitterResponse.Exception = ex;
                                        }
                                        finally
                                        {
                                            if (authenticationCompleteCallback != null)
                                            {
                                                twitterResponse.State =
                                                    new UserIdentifier
                                                    {
                                                        ID = userID,
                                                        UserID = userID,
                                                        ScreenName = screenName
                                                    };
                                                authenticationCompleteCallback(twitterResponse);
                                            }
                                        }
                                    }), null);
                        }), null);
#else
            Exception asyncException = null;

            using (var resetEvent = new ManualResetEvent(/*initialStateSignaled:*/ false))
            {
                req.BeginGetRequestStream(
                    new AsyncCallback(
                        ar =>
                        {
                            try
                            {
                                using (var requestStream = req.EndGetRequestStream(ar))
                                {
                                    requestStream.Write(postDataBytes, 0, postDataBytes.Length);
                                }
                            }
                            catch (Exception ex)
                            {
                                asyncException = ex;
                            }
                            finally
                            {
                                resetEvent.Set();
                            }
                        }), null);

                resetEvent.WaitOne();
            }

            if (asyncException != null)
            {
                throw asyncException;
            }

            req.BeginGetResponse(
                new AsyncCallback(
                    ar =>
                    {
                        string screenName = string.Empty;
                        string userID = string.Empty;

                        var twitterResponse = new TwitterAsyncResponse<UserIdentifier>();

                        try
                        {
                            string accessTokenResponse = string.Empty;

                            var res = req.EndGetResponse(ar) as HttpWebResponse;

                            using (var respStream = res.GetResponseStream())
                            using (var respReader = new StreamReader(respStream))
                            {
                                accessTokenResponse = respReader.ReadToEnd();
                            }

                            ProcessAccessTokenResponse(ref screenName, ref userID, accessTokenResponse);
                        }
                        catch (TwitterQueryException tqe)
                        {
                            twitterResponse.Status = TwitterErrorStatus.TwitterApiError;
                            twitterResponse.Message = "Error while communicating with Twitter. Please see Error property for details.";
                            twitterResponse.Exception = tqe;
                        }
                        catch (Exception ex)
                        {
                            twitterResponse.Status = TwitterErrorStatus.TwitterApiError;
                            twitterResponse.Message = "Error during LINQ to Twitter processing. Please see Error property for details.";
                            twitterResponse.Exception = ex;
                        }
                        finally
                        {
                            if (authenticationCompleteCallback != null)
                            {
                                twitterResponse.State =
                                    new UserIdentifier
                                    {
                                        ID = userID,
                                        UserID = userID,
                                        ScreenName = screenName
                                    };
                                authenticationCompleteCallback(twitterResponse);
                            }
                        }
                    }), null);
#endif
        }
    }
}
