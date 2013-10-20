using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// sends a new direct message to specified userr
        /// </summary>
        /// <param name="ctx">Twitter Context</param>
        /// <param name="user">UserID or ScreenName of user to send to</param>
        /// <param name="text">Direct message contents</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <returns>Direct message element</returns>
        public async Task<DirectMessage> NewDirectMessageAsync(string user, string text)
        {
            if (string.IsNullOrEmpty(user))
            {
                throw new ArgumentException("user is a required parameter.", "user");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text is a required parameter.", "text");
            }

            var newUrl = BaseUrl + "direct_messages/new.json";

            var reqProc = new DirectMessageRequestProcessor<DirectMessage>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<DirectMessage>(
                    newUrl,
                    new Dictionary<string, string>
                    {
                        {"user", user},
                        {"text", text}
                    });

            return reqProc.ProcessActionResult(resultsJson, DirectMessageType.Show);
        }

        /// <summary>
        /// deletes a direct message
        /// </summary>
        /// <param name="id">id of direct message</param>
        /// <param name="includeEntites">Set to false to prevent entities from being included (default: true).</param>
        /// <returns>direct message element</returns>
        public async Task<DirectMessage> DestroyDirectMessageAsync(string id, bool includeEntites)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = BaseUrl + "direct_messages/destroy.json";

            var reqProc = new DirectMessageRequestProcessor<DirectMessage>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<DirectMessage>(
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        {"id", id},
                        {"include_entities", includeEntites.ToString().ToLower()}
                    });

            return reqProc.ProcessActionResult(resultsJson, DirectMessageType.Show);
        }
    }
}
