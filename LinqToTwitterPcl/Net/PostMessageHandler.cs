using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter.Net
{
    class PostMessageHandler : HttpClientHandler
    {
        readonly TwitterExecute exe;
        readonly IDictionary<string, string> postData;
        readonly string url;

        public PostMessageHandler(TwitterExecute exe, IDictionary<string, string> postData, string url)
        {
            this.exe = exe;
            this.postData = postData;
            this.url = url;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            exe.SetAuthorizationHeader(HttpMethod.Post, url, postData, request);
            request.Headers.Add("User-Agent", exe.UserAgent);
            request.Headers.ExpectContinue = false;
            AutomaticDecompression = DecompressionMethods.GZip;

            //if (exe.ReadWriteTimeout != 0)
            //    ReadWriteTimeout = exe.ReadWriteTimeout;

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
