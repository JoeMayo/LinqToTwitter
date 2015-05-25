using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public interface IAuthorizer
    {
        Task AuthorizeAsync();

        string UserAgent { get; set; }

        ICredentialStore CredentialStore { get; set; }

        bool SupportsCompression { get; set; }

        string GetAuthorizationString(string method, string oauthUrl, IDictionary<string, string> parameters);
    }
}