using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        public async Task<bool> HideTweetAsync(string tweetID, CancellationToken cancelToken = default)
        {
            return await HideTweetAsync(tweetID, true, cancelToken);
        }

        public async Task<bool> UnHideTweetAsync(string tweetID, CancellationToken cancelToken = default)
        {
            return await HideTweetAsync(tweetID, false, cancelToken);
        }

        async Task<bool> HideTweetAsync(string tweetID, bool shouldHide, CancellationToken cancelToken)
        {
            _ = tweetID ?? throw new ArgumentNullException(nameof(tweetID), $"{nameof(tweetID)} is required.");

            string url = $"{BaseUrl2}tweets/{tweetID}/hidden";

            var postData = new Dictionary<string, string>();
            var postObj = new TweetHidden() { Hidden = shouldHide };

            RawResult =
                await TwitterExecutor.SendJsonToTwitterAsync(
                    HttpMethod.Put.ToString(),
                    url,
                    postData,
                    postObj,
                    cancelToken)
                   .ConfigureAwait(false);

            TweetHideResponse? result = JsonSerializer.Deserialize<TweetHideResponse>(RawResult);
                

            return result?.Data?.Hidden ?? false;
        }
    }
}
