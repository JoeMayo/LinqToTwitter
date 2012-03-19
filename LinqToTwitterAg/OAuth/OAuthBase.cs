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
 * PhotoBucket - Silverlight
 * 
 * http://code.google.com/p/photobucket-silverlight/source/browse/trunk/photobucketapi/OAuthBase.cs?r=46
 * 
 * Joe Mayo -
 * 
 * Modified 5/17/09
 ***********************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;

#if SILVERLIGHT && !WINDOWS_PHONE
    using System.Windows.Browser;
#elif !SILVERLIGHT && !WINDOWS_PHONE
    using System.Web;
#endif

namespace LinqToTwitter
{
    /// <summary>
    /// helps implement OAuth authentication
    /// </summary>
    public class OAuthBase
    {
        protected const string OAuthVersion = "1.0";
        protected const string OAuthParameterPrefix = "oauth_";

        //
        // List of known and used oauth parameters' names
        //        
        protected const string OAuthAccessTypeKey = "oauth_access_type";
        protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
        protected const string OAuthCallbackKey = "oauth_callback";
        protected const string OAuthVersionKey = "oauth_version";
        protected const string OAuthXAccessTypeKey = "x_auth_access_type";
        protected const string OAuthSignatureMethodKey = "oauth_signature_method";
        protected const string OAuthSignatureKey = "oauth_signature";
        protected const string OAuthTimestampKey = "oauth_timestamp";
        protected const string OAuthNonceKey = "oauth_nonce";
        protected const string OAuthTokenKey = "oauth_token";
        protected const string OAuthVerifierKey = "oauth_verifier";
        protected const string OAuthTokenSecretKey = "oauth_token_secret";

        protected const string HMACSHA1SignatureType = "HMAC-SHA1";
        protected const string PlainTextSignatureType = "PLAINTEXT";
        protected const string RSASHA1SignatureType = "RSA-SHA1";

        protected Random random = new Random();

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("You must specify a hashAlgorithm", "hashAlgorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("You must provide data to hash", "data");
            }

            byte[] dataBuffer = Encoding.UTF8.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private List<QueryParameter> CloneQueryParameters(IEnumerable<QueryParameter> parameters)
        {
            var result = new List<QueryParameter>();

            foreach (var pair in parameters)
            {
                string name = pair.Name;
                string value = pair.Value;

                if (!string.IsNullOrEmpty(name)
                    && (!name.StartsWith(OAuthParameterPrefix, StringComparison.Ordinal)
                        || name.StartsWith(OAuthAccessTypeKey, StringComparison.Ordinal)))
                {
                    result.Add(new QueryParameter(name, value ?? String.Empty));
                }
            }

            return result;
        }

        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < parameters.Count; i++)
            {
                QueryParameter p = parameters[i];
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}", p.Name, p.Value);
                sb.Append("&");
            }

            if (sb.Length > 1)
                sb.Length--;

            return sb.ToString();
        }

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="request">Request details</param>
        /// <param name="consumerKey">The consumer key</param>        
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="callback">Redirect URL for Web apps</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce">Unique value for this particular request</param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <param name="verifier">Number if using PIN authorization</param>
        /// <param name="timeStamp">Timestamp for this request</param>
        /// <param name="normalizedUrl">Url returned to caller</param>
        /// <param name="normalizedRequestParameters">Parameters returned to caller</param>
        /// <returns>The signature base</returns>
        public string GenerateSignatureBase(Request request, string consumerKey, string token, string tokenSecret, string verifier, string callback, string httpMethod, string timeStamp, string nonce, string signatureType, out string normalizedUrl, out string normalizedRequestParameters)
        {
            if (token == null)
            {
                token = string.Empty;
            }

            if (tokenSecret == null)
            {
                tokenSecret = string.Empty;
            }

            if (string.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("You must provide a consumerKey.", "consumerKey");
            }

            if (string.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("You must provide an httpMethod.", "httpMethod");
            }

            if (string.IsNullOrEmpty(signatureType))
            {
                throw new ArgumentNullException("You must provide a signatureType.", "signatureType");
            }

            var parameters = CloneQueryParameters(request.RequestParameters);
            parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
            parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));
            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, consumerKey));

            if (!string.IsNullOrEmpty(callback))
            {
                parameters.Add(new QueryParameter(OAuthCallbackKey, callback));
            }

            if (!string.IsNullOrEmpty(token))
            {
                parameters.Add(new QueryParameter(OAuthTokenKey, token));
            }

            if (!string.IsNullOrEmpty(verifier))
            {
                parameters.Add(new QueryParameter(OAuthVerifierKey, verifier));
            }

            // need to UrlEncode (per section 5.1) all the parameter values now, before sorting
            // see: http://hueniverse.com/2008/10/beginners-guide-to-oauth-part-iv-signing-requests/
            foreach (var parm in parameters)
                parm.Value = BuildUrlHelper.UrlEncode(parm.Value);

            parameters.Sort(QueryParameter.DefaultComparer);

            var url = new Uri(request.Endpoint);
            normalizedUrl = url.Scheme + "://";
#if !SILVERLIGHT
            normalizedUrl += url.Authority;
#else
            normalizedUrl += url.Host;

            if (!((url.Scheme == "http" && url.Port == 80)
                  || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
#endif
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            var signatureBase = new StringBuilder();

            signatureBase.AppendFormat(CultureInfo.InvariantCulture, "{0}&", httpMethod.ToUpper(CultureInfo.InvariantCulture));
            signatureBase.AppendFormat(CultureInfo.InvariantCulture, "{0}&", BuildUrlHelper.UrlEncode(normalizedUrl));
            signatureBase.AppendFormat(CultureInfo.InvariantCulture, "{0}", BuildUrlHelper.UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>
        /// <param name="request">Request details</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="callback">Redirect URL for Web apps</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="nonce">Unique value for this particular request</param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <param name="verifier">Number if using PIN authorization</param>
        /// <param name="timeStamp">Timestamp for this request</param>
        /// <param name="normalizedUrl">Url returned to caller</param>
        /// <param name="normalizedRequestParameters">Parameters returned to caller</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Request request, string consumerKey, string consumerSecret, string token, string tokenSecret, string verifier, string callback, string httpMethod, string timeStamp, string nonce, OAuthSignatureTypes signatureType, out string normalizedUrl, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (signatureType)
            {
                case OAuthSignatureTypes.PLAINTEXT:
                    return BuildUrlHelper.UrlEncode(
                        string.Format(CultureInfo.InvariantCulture, "{0}&{1}", consumerSecret, tokenSecret));
                case OAuthSignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(request, consumerKey, token, tokenSecret, verifier, callback, httpMethod, timeStamp, nonce, HMACSHA1SignatureType, out normalizedUrl, out normalizedRequestParameters);
                    var hmacsha1 = new HMACSHA1
                    {
                        Key =
                            Encoding.UTF8.GetBytes(
                                string.Format(
                                    CultureInfo.InvariantCulture, "{0}&{1}", 
                                    BuildUrlHelper.UrlEncode(consumerSecret),
                                    BuildUrlHelper.UrlEncode(tokenSecret)))
                    };
                    return ComputeHash(hmacsha1, signatureBase);
                case OAuthSignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns>timestamp</returns>
        public virtual string GenerateTimeStamp()
        {
            //// Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Math.Floor(ts.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns>nonce</returns>
        public virtual string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return random.Next(123400, 9999999).ToString(CultureInfo.InvariantCulture);
        }
    }
}
