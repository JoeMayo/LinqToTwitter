using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace LinqToTwitter
{
    public static class AccountActivityExtensions
    {
        public static string BuildCrcResponse(this AccountActivity accAct, string crc_token, string consumerSecret)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(consumerSecret);
            byte[] crcBytes = Encoding.UTF8.GetBytes(crc_token);

            var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(crcBytes);
            var base64Hmac = Convert.ToBase64String(hash);

            return "sha256=" + base64Hmac;
        }

        public static bool IsValidPostSignature(this AccountActivity accAct, HttpRequestMessage request, string message, string consumerSecret)
        {
            string webhooksSignature =
                request?.Headers
                    ?.GetValues("x-twitter-webhooks-signature")
                    ?.First()
                    ?.Replace("sha256=", "");

            if (webhooksSignature == null)
                return false;

            byte[] webhookSignatureByes = Convert.FromBase64String(webhooksSignature);

            byte[] keyBytes = Encoding.UTF8.GetBytes(consumerSecret);
            byte[] contentBytes = Encoding.UTF8.GetBytes(message);

            var hmac = new HMACSHA256(keyBytes);
            var contentHash = hmac.ComputeHash(contentBytes);

            if (!SecureCompareEqual(webhookSignatureByes, contentHash))
                return false;

            return true;
        }

        /// <summary>
        /// Avoid timing attack - see https://en.wikipedia.org/wiki/Timing_attack for more details.
        /// </summary>
        /// <param name="arrayA">First byte[].</param>
        /// <param name="arrayB">Second byte[].</param>
        /// <returns>True if both arrays are equal.</returns>
        static bool SecureCompareEqual(byte[] arrayA, byte[] arrayB)
        {
            if (arrayA.Length != arrayB.Length)
                return false;

            int result = 0;
            for (int i = 0; i < arrayA.Length; i++)
                result |= (arrayA[i] ^ arrayB[i]);

            return result == 0;
        }
    }
}
