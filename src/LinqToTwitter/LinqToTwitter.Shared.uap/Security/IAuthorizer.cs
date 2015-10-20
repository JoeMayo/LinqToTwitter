using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace LinqToTwitter
{
    public interface IAuthorizer
    {
        Task AuthorizeAsync();

        string UserAgent { get; set; }

        ICredentialStore CredentialStore { get; set; }

        bool SupportsCompression { get; set; }

        PasswordCredential ProxyCredential { get; set; }

        bool UseProxy { get; set; }

        string GetAuthorizationString(string method, string oauthUrl, IDictionary<string, string> parameters);
    }
}