using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public interface IAuthorizer
    {
        Task AuthorizeAsync();

        string UserAgent { get; set; }

        ICredentialStore CredentialStore { get; set; }

        string GetAuthorizationString(HttpMethod method, string oauthUrl, IDictionary<string, string> parameters);
    }
}