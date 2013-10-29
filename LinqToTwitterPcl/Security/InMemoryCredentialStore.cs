using System;
using System.Linq;
using System.Threading.Tasks;

namespace LinqToTwitter
{
    /// <summary>
    /// These credentials just reside in memory and only
    /// have a lifetime matching their containing AppDomain.
    /// </summary>
    public class InMemoryCredentialStore : ICredentialStore
    {
        public string ConsumerKey { get; set; }

        public string ConsumerSecret { get; set; }

        public string OAuthToken { get; set; }

        public string OAuthTokenSecret { get; set; }

        public string ScreenName { get; set; }

        public ulong UserID { get; set; }

        public bool HasAllCredentials()
        {
            return
                !string.IsNullOrWhiteSpace(ConsumerKey) &&
                !string.IsNullOrWhiteSpace(ConsumerSecret) &&
                !string.IsNullOrWhiteSpace(OAuthToken) &&
                !string.IsNullOrWhiteSpace(OAuthTokenSecret);
        }

#pragma warning disable 1998
        
        //
        // by definition, this type doesn't work 
        // with a data store other than memory.
        //

        public async Task LoadAsync()
        {
        }

        public async Task StoreAsync() { }

        public async Task ClearAsync() { }

#pragma warning restore 1998
    }
}
