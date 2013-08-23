using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using LinqToTwitter.Common;

namespace LinqToTwitter.Security
{
    public class OAuth : IOAuth
    {
        const string OAuthVersion = "1.0";
        const string OAuthSignatureMethod = "HMAC-SHA1";

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string OAuthToken { get; set; }

        public string OAuthTokenSecret { get; set; }

        public string GetAuthorizationString(string method, string url, IDictionary<string, string> parameters)
        {
            string encodedAndSortedString = BuildEncodedSortedString(parameters);
            string signatureBaseString = BuildSignatureBaseString(method, url, encodedAndSortedString);
            string signingKey = BuildSigningKey(ConsumerSecret, OAuthTokenSecret);
            string signature = CalculateSignature(signingKey, signatureBaseString);

            string[] allParms = (encodedAndSortedString + "&oauth_signature=" + Url.PercentEncode(signature)).Split('&');
            string allParmsString = 
                string.Join(", ",
                    (from parm in allParms
                     let keyVal = parm.Split('=')
                     where parm.StartsWith("oauth")
                     orderby keyVal[0]
                     select keyVal[0] + "=\"" + keyVal[1] + "\"")
                    .ToList());

            return "OAuth " + allParmsString;
        }

        public string BuildEncodedSortedString(IDictionary<string, string> parameters)
        {
            return
                string.Join("&",
                    (from parm in parameters
                     orderby parm.Key
                     select parm.Key + "=" + Url.PercentEncode(parameters[parm.Key]))
                    .ToArray());
        }

        public string BuildSignatureBaseString(string method, string url, string encodedStringParameters)
        {
            return string.Join("&", new string[]
            {
                method.ToUpper(),
                Url.PercentEncode(url),
                Url.PercentEncode(encodedStringParameters)
            });
        }

        public string BuildSigningKey(string consumerSecret, string oAuthTokenSecret)
        {
            return Url.PercentEncode(consumerSecret) + "&" + Url.PercentEncode(oAuthTokenSecret);
        }

        public string CalculateSignature(string signingKey, string signatureBaseString)
        {
            byte[] key = Encoding.UTF8.GetBytes(signingKey);
            byte[] msg = Encoding.UTF8.GetBytes(signatureBaseString);

            byte[] hash = new Hmac(new Sha1()).Sign(key, msg);

            return Convert.ToBase64String(hash);
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
            return new Random().Next(123400, 9999999).ToString(CultureInfo.InvariantCulture);
        }
    }
}
