using System;
using System.Collections.Generic;

namespace LinqToTwitter
{
    public static class RawExtensions
    {
        /// <summary>
        /// Lets you perform a query by specifying the raw URL and parameters yourself.
        /// Useful for when Twitter changes or adds new features before they are added to LINQ to Twitter.
        /// </summary>
        /// <param name="queryString">The segments that follow the base URL. i.e. "statuses/public_timeline.xml" for a public status query</param>
        /// <param name="parameters">Querystring parameters that will be appended to the URL</param>
        /// <returns></returns>
        public static string ExecuteRaw(this TwitterContext ctx, string queryString, Dictionary<string, string> parameters)
        {
            return ExecuteRaw(ctx, queryString, parameters, null);
        }

        /// <summary>
        /// Lets you perform a query by specifying the raw URL and parameters yourself.
        /// Useful for when Twitter changes or adds new features before they are added to LINQ to Twitter.
        /// </summary>
        /// <param name="queryString">The segments that follow the base URL. i.e. "statuses/public_timeline.xml" for a public status query</param>
        /// <param name="parameters">Querystring parameters that will be appended to the URL</param>
        /// <param name="callback">Async Callback used in Silverlight queries</param>
        /// <returns></returns>
        public static string ExecuteRaw(this TwitterContext ctx, string queryString, Dictionary<string, string> parameters, Action<TwitterAsyncResponse<string>> callback)
        {
            string rawUrl = ctx.BaseUrl.TrimEnd('/') + "/" + queryString.TrimStart('/');

            var reqProc = new RawRequestProcessor<Raw>();

            ITwitterExecute exec = ctx.TwitterExecutor;
            exec.AsyncCallback = callback;
            var results =
                exec.ExecuteTwitter(
                    rawUrl,
                    parameters,
                    reqProc);

            return results;
        }
    }
}
