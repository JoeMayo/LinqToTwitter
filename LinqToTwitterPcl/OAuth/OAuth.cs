using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter.Common;

namespace LinqToTwitter.OAuth
{
    public class OAuth : IOAuth
    {
        const int Sha1Blocksize = 64;

        const string OAuthVersion = "1.0";
        const string OAuthSignatureMethod = "HMAC-SHA1";

        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string OAuthToken { get; set; }

        public string OAuthTokenSecret { get; set; }

        public string GetAuthorizationString(string method, string url, IDictionary<string, string> parameters)
        {
            return null;
        }

        public string BuildEncodedSortedString(IDictionary<string, string> parameters)
        {
            return
                string.Join("&",
                    (from parm in parameters
                     orderby parm.Key
                     select parm.Key + "=" + BuildUrlHelper.UrlEncode(parameters[parm.Key]))
                    .ToArray());
        }

        public string BuildSignatureBaseString(string method, string url, string encodedStringParameters)
        {
            return string.Join("&", new string[]
            {
                method.ToUpper(),
                BuildUrlHelper.UrlEncode(url),
                BuildUrlHelper.UrlEncode(encodedStringParameters)
            });
        }

        public string BuildSigningKey(string consumerSecret, string oAuthTokenSecret)
        {
            return BuildUrlHelper.UrlEncode(consumerSecret) + "&" + BuildUrlHelper.UrlEncode(oAuthTokenSecret);
        }

        public string CalculateSignature(string signingKey, string signatureBaseString)
        {
            byte[] key = Encoding.UTF8.GetBytes(signingKey);
            byte[] msg = Encoding.UTF8.GetBytes(signatureBaseString);

            byte[] initializedKey = InitializeKey(key);

            byte[] oKeyPad = new byte[Sha1Blocksize];
            byte[] iKeyPad = new byte[Sha1Blocksize];

            for (int i = 0; i < Sha1Blocksize; i++)
            {
                oKeyPad[i] = (byte)(0x5c ^ initializedKey[i]);
                iKeyPad[i] = (byte)(0x36 ^ initializedKey[i]);
            }

            byte[] innerHash = ComputeHash(Combine(iKeyPad, msg));
            byte[] outerHash = ComputeHash(Combine(oKeyPad, innerHash));

            string signature = Convert.ToBase64String(outerHash);

            return signature;
        }
  
        byte[] InitializeKey(byte[] key)
        {
            byte[] initializedKey = null;

            if (key.Length > Sha1Blocksize)
            {
                initializedKey = ComputeHash(key);
            }
            else if (key.Length < Sha1Blocksize)
            {
                byte[] padding = Enumerable.Repeat<byte>(0x00, Sha1Blocksize - key.Length).ToArray();
                initializedKey = Combine(key, padding);
            }
            else
            {
                initializedKey = key;
            }
            return initializedKey;
        }
  
        private byte[] Combine(byte[] first, byte[] second)
        {
            byte[] combinedBytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, combinedBytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, combinedBytes, first.Length, second.Length);
            return combinedBytes;
        }

        public byte[] ComputeHash(byte[] key)
        {
            return new byte[0];
        }
    }
}
