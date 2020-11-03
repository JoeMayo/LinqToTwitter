using LinqToTwitter.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public static class TwitterExtensions
    {
        /// <summary>
        /// Callback is invoked by LINQ to Twitter streaming support,
        /// allowing you to process each individual response from Twitter.
        /// For best results, please review the Twitter API streaming guidelines.
        /// </summary>
        /// <param name="streaming">Query being extended</param>
        /// <param name="callback">Your code for handling Twitter content</param>
        /// <returns>Streaming instance to support further LINQ opertations</returns>
        public static async Task<List<Streaming>> StartAsync(this IQueryable<Streaming> streaming, Func<StreamContent, Task> callback)
        {
            var provider = streaming.Provider as TwitterQueryProvider;
            provider!
                .Context
                .TwitterExecutor
                .StreamingCallbackAsync = callback;

            return await streaming.ToListAsync().ConfigureAwait(false);
        }

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> query)
        {
            var provider = query.Provider as TwitterQueryProvider;

            IEnumerable<T> results = (IEnumerable<T>)await provider!.ExecuteAsync<IEnumerable<T>>(query.Expression).ConfigureAwait(false);

            return results.ToList();
        }

        public static async Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> query)
            where T : class
        {
            var provider = query.Provider as TwitterQueryProvider;

            IEnumerable<T> results = (IEnumerable<T>)await provider!.ExecuteAsync<T>(query.Expression).ConfigureAwait(false);

            return results.FirstOrDefault();
        }

        public static async Task<T> FirstAsync<T>(this IQueryable<T> query)
            where T : class
        {
            var provider = query.Provider as TwitterQueryProvider;

            IEnumerable<T> results = (IEnumerable<T>)await provider!.ExecuteAsync<T>(query.Expression).ConfigureAwait(false);

            return results.First();
        }

        public static async Task<T?> SingleOrDefaultAsync<T>(this IQueryable<T> query)
            where T : class
        {
            var provider = query.Provider as TwitterQueryProvider;

            IEnumerable<T> results = (IEnumerable<T>)await provider!.ExecuteAsync<T>(query.Expression).ConfigureAwait(false);

            return results.SingleOrDefault();
        }

        public static async Task<T> SingleAsync<T>(this IQueryable<T> query)
            where T : class
        {
            var provider = query.Provider as TwitterQueryProvider;

            IEnumerable<T> results = (IEnumerable<T>)await provider!.ExecuteAsync<T>(query.Expression).ConfigureAwait(false);

            return results.Single();
        }

        /// <summary>
        /// Enables use of .NET Cancellation Framework for this query.
        /// </summary>
        /// <param name="streaming">Query being extended</param>
        /// <param name="callback">Your code for handling Twitter content</param>
        /// <returns>Streaming instance to support further LINQ opertations</returns>
        public static IQueryable<T> WithCancellation<T>(this IQueryable<T> query, CancellationToken cancelToken)
            where T : class
        {
            var provider = query.Provider as TwitterQueryProvider;
            provider!
                .Context
                .TwitterExecutor
                .CancellationToken = cancelToken;

            return query;
        }
    }
}
