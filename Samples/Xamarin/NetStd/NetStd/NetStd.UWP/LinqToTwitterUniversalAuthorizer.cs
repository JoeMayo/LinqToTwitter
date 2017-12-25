using LinqToTwitter;
using NetStd.Models;

namespace NetStd.UWP
{
    public class LinqToTwitterUniversalAuthorizer : ILinqToTwitterAuthorizer
    {
        public IAuthorizer GetAuthorizer(string consumerKey, string consumerSecret)
        {
            return new UniversalAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret
                }
            };
        }
    }
}
