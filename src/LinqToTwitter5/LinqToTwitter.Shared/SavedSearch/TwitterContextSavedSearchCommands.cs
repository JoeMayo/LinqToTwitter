using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
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
        public async Task<SavedSearch> CreateSavedSearchAsync(string query, CancellationToken cancelToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentException("query is required.", "query");

            var savedSearchUrl = BaseUrl + "saved_searches/create.json";

            var reqProc = new SavedSearchRequestProcessor<SavedSearch>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<SavedSearch>(
                    HttpMethod.Post.ToString(),
                    savedSearchUrl,
                    new Dictionary<string, string>
                    {
                        { "query", query }
                    },
                    cancelToken)
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, SavedSearchAction.Create);
        }

        /// <summary>
        /// Adds a saved search to your twitter account
        /// </summary>
        /// <param name="id">ID of saved search</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns>SavedSearch object</returns>
        public async Task<SavedSearch> DestroySavedSearchAsync(ulong id, CancellationToken cancelToken = default(CancellationToken))
        {
            if (id == 0)
                throw new ArgumentException("Invalid Saved Search ID: " + id, "id");

            var savedSearchUrl = BaseUrl + "saved_searches/destroy/" + id + ".json";

            var reqProc = new SavedSearchRequestProcessor<SavedSearch>();

            RawResult =
                await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<SavedSearch>(
                    HttpMethod.Post.ToString(),
                    savedSearchUrl,
                    new Dictionary<string, string>(),
                    cancelToken)
                    .ConfigureAwait(false);

            SavedSearch result = reqProc.ProcessActionResult(RawResult, SavedSearchAction.Destroy);
            result.ID = id;

            return result;
        }
    }
}
