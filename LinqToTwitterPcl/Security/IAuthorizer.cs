using System;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    public interface IAuthorizer
    {
        Task AuthorizeAsync();

        string UserAgent { get; set; }

        ICredentialStore CredentialStore { get; set; }
    }
}