using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;

namespace LinqToTwitter
{
    public static class FavoritesExtensions
    {
        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public static Status CreateFavorite(this TwitterContext ctx, string id)
        {
            return CreateFavorite(ctx, id, null);
        }

        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>status of favorite</returns>
        public static Status CreateFavorite(this TwitterContext ctx, string id, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = ctx.BaseUrl + "favorites/create/" + id + ".xml";

            var reqProc = new StatusRequestProcessor<Status>();

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsXml =
                twitExe.ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>(),
                    reqProc);

            List<Status> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public static Status DestroyFavorite(this TwitterContext ctx, string id)
        {
            return DestroyFavorite(ctx, id, null);
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>status of favorite</returns>
        public static Status DestroyFavorite(this TwitterContext ctx, string id, Action<TwitterAsyncResponse<Status>> callback)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("id is a required parameter.", "id");
            }

            var favoritesUrl = ctx.BaseUrl + "favorites/destroy/" + id + ".xml";

            var reqProc = new StatusRequestProcessor<Status>();

            ITwitterExecute twitExe = ctx.TwitterExecutor;

            twitExe.AsyncCallback = callback;
            var resultsXml =
                twitExe.ExecuteTwitter(
                    favoritesUrl,
                    new Dictionary<string, string>(),
                    reqProc);

            List<Status> results = reqProc.ProcessResults(resultsXml);
            return results.FirstOrDefault();
        }

    }
}
