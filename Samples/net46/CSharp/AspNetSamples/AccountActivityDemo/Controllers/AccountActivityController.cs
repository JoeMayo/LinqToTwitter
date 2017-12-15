using LinqToTwitter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
                response_token = new AccountActivity().BuildCrcResponse(crc_token, consumerSecret)
            };
        }

        static object dmReadLock = new object();

        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            try
            {
                Monitor.Enter(dmReadLock);
                string response = await request.Content.ReadAsStringAsync();

                if (!new AccountActivity().IsValidPostSignature(request, response, consumerSecret))
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
    }
}
