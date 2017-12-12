using LinqToTwitter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AccountActivityDemo.Controllers
{
    public class AccountActivityController : ApiController
    {
        string consumerKey = "";
        string consumerSecret = "";
        string accessToken = "";
        string accessTokenSecret = "";
        string apiKey = "";

        ulong chatbotID = 15411837;

        public object Get(string crc_token)
        {
            return new
            {
                response_token = BuildCrcResponse(crc_token)
            };
        }

        static object dmReadLock = new object();

        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            try
            {
                Monitor.Enter(dmReadLock);
                string response = await request.Content.ReadAsStringAsync();

                if (!IsValidPostSignature(request, response))
                    return request.CreateResponse(HttpStatusCode.Unauthorized);

                JObject content = JObject.Parse(response);

                if (IsDuplicate(content))
                    return request.CreateResponse(HttpStatusCode.OK);

                JToken messageCreate = content["direct_message_events"][0]["message_create"];

                string recipientIDStr = (string)messageCreate["sender_id"];
                ulong.TryParse(recipientIDStr, out ulong recipientID);

                var authorizer = new SingleUserAuthorizer
                {
                    CredentialStore = new InMemoryCredentialStore
                    {
                        ConsumerKey = consumerKey,
                        ConsumerSecret = consumerSecret,
                        OAuthToken = accessToken,
                        OAuthTokenSecret = accessTokenSecret
                    }
                };
                var twitterCtx = new TwitterContext(authorizer);

                if (recipientID != chatbotID)
                {
                    string text = (string)messageCreate["message_data"]["text"];

                    await twitterCtx.NewDirectMessageEventAsync(
                        recipientID, $"You said: {text}, which is {text.Length} characters.");
                }

                return request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            finally
            {
                Monitor.Exit(dmReadLock);
            }
        }

        static List<string> previousMessageIDs = new List<string>();
        bool IsDuplicate(JObject content)
        {
            string msgID = content["direct_message_events"][0]["id"].Value<string>();
            bool isDuplicate = previousMessageIDs.IndexOf(msgID) > -1;
            previousMessageIDs.Add(msgID);
            return isDuplicate;
        }

        string BuildCrcResponse(string crc_token)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(consumerSecret);
            byte[] crcBytes = Encoding.UTF8.GetBytes(crc_token);

            var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(crcBytes);
            var base64Hmac = Convert.ToBase64String(hash);

            return "sha256=" + base64Hmac;
        }

        bool IsValidPostSignature(HttpRequestMessage request, string message)
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
        bool SecureCompareEqual(byte[] arrayA, byte[] arrayB)
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
