using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="query">Search query to add</param>
        /// <returns>SavedSearch object</returns>
        public async Task<SavedSearch> CreateSavedSearchAsync(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentException("query is required.", "query");
            }

            var savedSearchUrl = BaseUrl + "saved_searches/create.json";

            var reqProc = new SavedSearchRequestProcessor<SavedSearch>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<SavedSearch>(
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "query", query }
                    });

            return reqProc.ProcessActionResult(resultsJson, SavedSearchAction.Create);
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="id">ID of saved search</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>SavedSearch object</returns>
        public async Task<SavedSearch> DestroySavedSearchAsync(ulong id)
        {
            if (id < 1)
                throw new ArgumentException("Invalid Saved Search ID: " + id, "id");

            var savedSearchUrl = BaseUrl + "saved_searches/destroy/" + id + ".json";

            var reqProc = new SavedSearchRequestProcessor<SavedSearch>();

            var resultsJson =
                await TwitterExecutor.PostToTwitterAsync<SavedSearch>(
                    savedSearchUrl,
                    new Dictionary<string, string>());

            SavedSearch result = reqProc.ProcessActionResult(resultsJson, SavedSearchAction.Destroy);
            result.ID = id.ToString();

            return result;
        }
    }
}
