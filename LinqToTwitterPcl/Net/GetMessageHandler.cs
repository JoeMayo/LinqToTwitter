using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinqToTwitter.Net
{
    class GetMessageHandler : HttpClientHandler
    {
        TwitterExecute exe;
        IDictionary<string, string> parameters;
        string url;

        public GetMessageHandler(TwitterExecute exe, IDictionary<string, string> parameters, string url)
        {
            this.exe = exe;
            this.parameters = parameters;
            this.url = url;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            exe.SetAuthorizationHeader(HttpMethod.Get, url, parameters, request);
            request.Headers.Add("User-Agent", exe.UserAgent);
            request.Headers.ExpectContinue = false;
            AutomaticDecompression = DecompressionMethods.GZip;

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
