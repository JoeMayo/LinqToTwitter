using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public partial class TwitterContext
    {
        public const ulong MissingID = 0ul;
        public const ulong NoReply = 0ul;
        public const decimal NoCoordinate = Decimal.MaxValue;

        /// <summary>
        /// Retweets a tweet.
        /// </summary>
        /// <param name="tweetID">ID of tweet being retweeted.</param>
        /// <returns>Retweeted tweet.</returns>
        public virtual async Task<Status?> RetweetAsync(ulong tweetID, CancellationToken cancelToken = default(CancellationToken))
        {
            if (tweetID == MissingID)
                throw new ArgumentException("0 is *not* a valid tweetID. You must provide the ID of the tweet you're retweeting.", "tweetID");

            var retweetUrl = BaseUrl + "statuses/retweet/" + tweetID + ".json";

            RawResult = await TwitterExecutor
                .PostFormUrlEncodedToTwitterAsync<Status>(HttpMethod.Post.ToString(), retweetUrl, new Dictionary<string, string?>(), cancelToken)
                .ConfigureAwait(false);

            return new StatusRequestProcessor<Status>()
                .ProcessActionResult(RawResult, StatusAction.SingleStatus);
        }
    }
}
