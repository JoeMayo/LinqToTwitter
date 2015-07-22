using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ModernHttpClient;

namespace LinqToTwitter.Net
{
    class PostMessageHandler : NativeMessageHandler
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
            exe.SetAuthorizationHeader(HttpMethod.Post.ToString(), url, postData, request);
            request.Headers.Add("User-Agent", exe.UserAgent);
            request.Headers.ExpectContinue = false;
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };

            //if (exe.Authorizer.Proxy != null && SupportsProxy)
            //    Proxy = exe.Authorizer.Proxy;

            //if (exe.ReadWriteTimeout != 0)
            //    ReadWriteTimeout = exe.ReadWriteTimeout;

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
