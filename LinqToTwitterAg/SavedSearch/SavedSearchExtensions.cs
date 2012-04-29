using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqToTwitter
{
    public static class SavedSearchExtensions
    {
        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="query">Search query to add</param>
        /// <returns>SavedSearch object</returns>
        public static SavedSearch CreateSavedSearch(this TwitterContext ctx, string query)
        {
            return CreateSavedSearch(ctx, query, null);
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="query">Search query to add</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>SavedSearch object</returns>
        public static SavedSearch CreateSavedSearch(this TwitterContext ctx, string query, Action<TwitterAsyncResponse<SavedSearch>> callback)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("query is required.", "query");
            }

            var savedSearchUrl = ctx.BaseUrl + "saved_searches/create.json";

            var reqProc = new SavedSearchRequestProcessor<SavedSearch>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "query", query }
                    },
                    reqProc);

            SavedSearch result = reqProc.ProcessActionResult(resultsJson, SavedSearchAction.Create);
            return result;
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <returns>SavedSearch object</returns>
        public static SavedSearch DestroySavedSearch(this TwitterContext ctx, int id)
        {
            return DestroySavedSearch(ctx, id, null);
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="id">ID of saved search</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>SavedSearch object</returns>
        public static SavedSearch DestroySavedSearch(this TwitterContext ctx, int id, Action<TwitterAsyncResponse<SavedSearch>> callback)
        {
            if (id < 1)
            {
                throw new ArgumentException("Invalid Saved Search ID: " + id, "id");
            }

            var savedSearchUrl = ctx.BaseUrl + "saved_searches/destroy/" + id + ".json";

            var reqProc = new SavedSearchRequestProcessor<SavedSearch>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var resultsJson =
                exec.ExecuteTwitter(
                    savedSearchUrl,
                    new Dictionary<string, string>(),
                    reqProc);

            SavedSearch result = reqProc.ProcessActionResult(resultsJson, SavedSearchAction.Destroy);
            result.ID = id.ToString();

            return result;
        }
    }
}
