using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public static class TwitterExtensions
    {
        /// <summary>
        /// Callback is invoked by LINQ to Twitter streaming support,
        /// allowing you to process each individual response from Twitter.
        /// 
        /// If your callback code fails to handle an exception,
        /// LINQ to Twitter will log and re-throw. Remember to ensure
        /// your code conforms with Twitter stream usage guidelines.
        /// LINQ to Twitter has a conformant backoff/retry strategy, but
        /// even that won't help if your code throws exceptions and
        /// tries to re-connect in a way that violates Twitter policy.
        /// Please review Twitter's Access and Rate Limiting policy
        /// for more information:
        /// 
        /// http://dev.twitter.com/pages/streaming_api_concepts#access-rate-limiting
        /// </summary>
        /// <param name="streaming">Query being extended</param>
        /// <param name="callback">Your code for handling Twitter content</param>
        /// <returns>Streaming instance to support further LINQ opertations</returns>
        public static IQueryable<Streaming> StreamingCallback(this IQueryable<Streaming> streaming, Action<StreamContent> callback)
        {
            (streaming.Provider as TwitterQueryProvider)
                .Context
                .TwitterExecutor
                .StreamingCallback = callback;

            return streaming;
        }

        /// <summary>
        /// Callback is invoked by LINQ to Twitter streaming support,
        /// allowing you to process each individual response from Twitter.
        /// 
        /// If your callback code fails to handle an exception
        /// LINQ to Twitter will log and re-throw. Remember to ensure
        /// your code conforms with Twitter stream usage guidelines.
        /// LINQ to Twitter has a conformant backoff/retry strategy, but
        /// even that won't help if your code throws exceptions and
        /// tries to re-connect in a way that violates Twitter policy.
        /// Please review Twitter's Access and Rate Limiting policy
        /// for more information:
        /// 
        /// http://dev.twitter.com/pages/streaming_api_concepts#access-rate-limiting
        /// </summary>
        /// <param name="streaming">Query being extended</param>
        /// <param name="callback">Your code for handling Twitter content</param>
        /// <returns>Streaming instance to support further LINQ opertations</returns>
        public static IQueryable<UserStream> StreamingCallback(this IQueryable<UserStream> streaming, Action<StreamContent> callback)
        {
            (streaming.Provider as TwitterQueryProvider)
                .Context
                .TwitterExecutor
                .StreamingCallback = callback;

            return streaming;
        }

        public static IQueryable<T> AsyncCallback<T>(this IQueryable<T> queryType, Action<IEnumerable<T>> callback)
        {
            (queryType.Provider as TwitterQueryProvider)
                .Context
                .TwitterExecutor
                .AsyncCallback = callback;

            return queryType;
        }

        public static void MaterializedAsyncCallback<T>(this IQueryable<T> queryType, Action<TwitterAsyncResponse<IEnumerable<T>>> callback)
        {
            (queryType.Provider as TwitterQueryProvider)
                .Context
                .TwitterExecutor
                .AsyncCallback = callback;

            queryType.ToList();
        }

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> query)
        {
            var provider = query.Provider as TwitterQueryProvider;

            IEnumerable<T> results = await provider.ExecuteAsync<IEnumerable<T>>(query.Expression);

            return results.ToList();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> query)
            where T : class
        {
            var provider = query.Provider as TwitterQueryProvider;

            IEnumerable<T> results = await provider.ExecuteAsync<T>(query.Expression);

            return results.FirstOrDefault();
        }
    }
}
