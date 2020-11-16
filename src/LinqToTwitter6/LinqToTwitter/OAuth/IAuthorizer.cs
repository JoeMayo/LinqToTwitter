using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    public interface IAuthorizer
    {
        Task AuthorizeAsync();

        string? UserAgent { get; set; }

        ICredentialStore? CredentialStore { get; set; }

        IWebProxy? Proxy { get; set; }

        bool SupportsCompression { get; set; }

        string? GetAuthorizationString(string method, string oauthUrl, IDictionary<string, string?> parameters);
    }
}