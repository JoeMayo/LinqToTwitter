using LinqToTwitter.Provider;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter.Net
{
    class GetMessageHandler : HttpClientHandler
    {
        readonly TwitterExecute exe;
        readonly IDictionary<string, string> parameters;
        readonly string url;
        readonly bool authorizerSupportsCompression;

        public GetMessageHandler(TwitterExecute exe, IDictionary<string, string> parameters, string url, bool authorizerSupportsCompression)
        {
            this.exe = exe;
            this.parameters = parameters;
            this.url = url;
            this.authorizerSupportsCompression = authorizerSupportsCompression;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            exe.SetAuthorizationHeader(HttpMethod.Get.ToString(), url, parameters, request);
            request.Headers.Add("User-Agent", exe.UserAgent);
            request.Headers.ExpectContinue = false;
            if (SupportsAutomaticDecompression && authorizerSupportsCompression)
                AutomaticDecompression = DecompressionMethods.GZip;
            if (exe.Authorizer.Proxy != null && SupportsProxy)
                Proxy = exe.Authorizer.Proxy;

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
