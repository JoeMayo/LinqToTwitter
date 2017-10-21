using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        /// <summary>
        /// Lets you perform a query by specifying the raw URL and parameters yourself.
        /// Useful for when Twitter changes or adds new features before they are added to LINQ to Twitter.
        /// </summary>
        /// <param name="queryString">The segments that follow the base URL. i.e. "statuses/home_timeline.json" for a home timeline query</param>
        /// <param name="parameters">Querystring parameters that will be appended to the URL</param>
        /// <returns>Twitter JSON response.</returns>
        public async Task<string> ExecuteRawAsync(string queryString, Dictionary<string, string> parameters, CancellationToken cancelToken = default(CancellationToken))
        {
            string rawUrl = BaseUrl.TrimEnd('/') + "/" + queryString.TrimStart('/');

            RawResult = await TwitterExecutor.PostFormUrlEncodedToTwitterAsync<Raw>(HttpMethod.Post, rawUrl, parameters, cancelToken).ConfigureAwait(false);

            return RawResult;
        }
    }
}
