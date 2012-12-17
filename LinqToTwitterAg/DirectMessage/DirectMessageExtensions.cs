using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    public static class DirectMessageExtensions
    {
        /// <summary>
        /// sends a new direct message to specified user
        /// </summary>
        /// <param name="ctx">Twitter Context</param>
        /// <param name="user">UserID or ScreenName of user to send to</param>
        /// <param name="text">Direct message contents</param>
        /// <returns>Direct message element</returns>
        public static DirectMessage NewDirectMessage(this TwitterContext ctx, string user, string text)
        {
            return NewDirectMessage(ctx, user, text, false, null);
        }

        /// <summary>
        /// sends a new direct message to specified user
        /// </summary>
        /// <param name="ctx">Twitter Context</param>
        /// <param name="user">UserID or ScreenName of user to send to</param>
        /// <param name="text">Direct message contents</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <returns>Direct message element</returns>
        public static DirectMessage NewDirectMessage(this TwitterContext ctx, string user, string text, bool wrapLinks)
        {
            return NewDirectMessage(ctx, user, text, wrapLinks, null);
        }

        /// <summary>
        /// sends a new direct message to specified userr
        /// </summary>
        /// <param name="ctx">Twitter Context</param>
        /// <param name="user">UserID or ScreenName of user to send to</param>
        /// <param name="text">Direct message contents</param>
        /// <param name="wrapLinks">Shorten links using Twitter's t.co wrapper</param>
        /// <param name="callback">Async Callback</param>
        /// <returns>Direct message element</returns>
        public static DirectMessage NewDirectMessage(this TwitterContext ctx, string user, string text, bool wrapLinks, Action<TwitterAsyncResponse<DirectMessage>> callback)
        {
            if (string.IsNullOrEmpty(user))
            {
                throw new ArgumentException("user is a required parameter.", "user");
            }

            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("text is a required parameter.", "text");
            }

            var newUrl = ctx.BaseUrl + "direct_messages/new.json";

            var reqProc = new DirectMessageRequestProcessor<DirectMessage>();

            var twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.PostToTwitter(
                    newUrl,
                    new Dictionary<string, string>
                    {
                        {"user", user},
                        {"text", text},
                        {"wrap_links", wrapLinks ? true.ToString() : null }
                    },
                    response => reqProc.ProcessActionResult(response, DirectMessageType.Show));

            DirectMessage dm = reqProc.ProcessActionResult(resultsJson, DirectMessageType.Show);
            return dm;
        }

        /// <summary>
        /// deletes a direct message
        /// </summary>
        /// <param name="ctx">Twitter Context</param>
        /// <param name="id">id of direct message</param>
        /// <returns>direct message element</returns>
        public static DirectMessage DestroyDirectMessage(this TwitterContext ctx, string id)
        {
            return DestroyDirectMessage(ctx, id, null);
        }

        /// <summary>
        /// deletes a direct message
        /// </summary>
        /// <param name="ctx">Twitter Context</param>
        /// <param name="id">id of direct message</param>
        /// <param name="callback">Async Callback</param>
        /// <returns>direct message element</returns>
        public static DirectMessage DestroyDirectMessage(this TwitterContext ctx, string id, Action<TwitterAsyncResponse<DirectMessage>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var destroyUrl = ctx.BaseUrl + "direct_messages/destroy.json";

            var reqProc = new DirectMessageRequestProcessor<DirectMessage>();

            var twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsJson =
                twitExe.PostToTwitter(
                    destroyUrl,
                    new Dictionary<string, string>
                    {
                        {"id", id}
                    },
                    response => reqProc.ProcessActionResult(response, DirectMessageType.Show));

            DirectMessage dm = reqProc.ProcessActionResult(resultsJson, DirectMessageType.Show);
            return dm;
        }
    }
}
