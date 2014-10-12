using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Adds a favorite to the logged-in user's profile.
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public async Task<Status> CreateFavoriteAsync(ulong id)
        {
            return await CreateFavoriteAsync(id, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds a favorite to the logged-in user's profile
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <param name="includeEntities">Response doesn't include entities when false. (default: true)</param>
        /// <returns>status of favorite</returns>
        public async Task<Status> CreateFavoriteAsync(ulong id, bool includeEntities)
        {
            if (id == 0)
                throw new ArgumentException("id is a required parameter.", "id");

            var favoritesUrl = BaseUrl + "favorites/create.json";

            var reqProc = new StatusRequestProcessor<Status>();

            RawResult =
                await TwitterExecutor.PostToTwitterAsync<Status>(
                    favoritesUrl,
                    new Dictionary<string, string>
                    {
                        {"id", id.ToString()},
                        {"include_entities", includeEntities.ToString()}
                    })
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, FavoritesAction.SingleStatus);
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile.
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <returns>status of favorite</returns>
        public async Task<Status> DestroyFavoriteAsync(ulong id)
        {
            return await DestroyFavoriteAsync(id, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a favorite from the logged-in user's profile.
        /// </summary>
        /// <param name="id">id of status to add to favorites</param>
        /// <param name="includeEntities">Response doesn't include entities when false. (default: true)</param>
        /// <returns>status of favorite</returns>
        public async Task<Status> DestroyFavoriteAsync(ulong id, bool includeEntities)
        {
            if (id == 0)
                throw new ArgumentException("id is a required parameter.", "id");

            var favoritesUrl = BaseUrl + "favorites/destroy.json";

            var reqProc = new StatusRequestProcessor<Status>();

            RawResult =
                await TwitterExecutor.PostToTwitterAsync<Status>(
                    favoritesUrl,
                    new Dictionary<string, string>
                    {
                        {"id", id.ToString()},
                        {"include_entities", includeEntities.ToString()}
                    })
                    .ConfigureAwait(false);

            return reqProc.ProcessActionResult(RawResult, FavoritesAction.SingleStatus);
        }
    }
}
