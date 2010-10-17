using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqToTwitter
{
    public static class TwitterExtensions
    {
        public static IQueryable<Streaming> StreamingCallback(this IQueryable<Streaming> streaming, Action<StreamContent> callback)
        {
            (streaming.Provider as TwitterQueryProvider)
                .Context
                .TwitterExecutor
                .StreamingCallback = callback;

            return streaming;
        }
    }
}
