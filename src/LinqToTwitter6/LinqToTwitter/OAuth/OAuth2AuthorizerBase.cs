using System;
using System.Collections.Generic;
using System.Net;

namespace LinqToTwitter.OAuth
{
    public abstract class OAuth2AuthorizerBase
    {
        /// <summary>
        /// User-Agent header string sent to Twitter to represent your application. Defaults to LINQ to Twitter.
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Url that Twitter redirects to after user authorizes your app
        /// </summary>
        public string? Callback { get; set; }

        /// <summary>
        /// Allows user to specify a proxy for HTTP requests
        /// </summary>
        public IWebProxy? Proxy { get; set; }

        /// <summary>
        /// LINQ to Twitter will use gzip compression if the client supports it
        /// </summary>
        public bool SupportsCompression { get; set; }

        /// <summary>
        /// Get/Set Credentials
        /// </summary>
        public ICredentialStore? CredentialStore { get; set; }

        public abstract string? GetAuthorizationString(string method, string oauthUrl, IDictionary<string, string> parameters);
    }
}