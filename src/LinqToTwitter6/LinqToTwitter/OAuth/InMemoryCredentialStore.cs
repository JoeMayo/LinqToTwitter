using System.Threading.Tasks;

namespace LinqToTwitter.OAuth
{
    /// <summary>
    /// These credentials just reside in memory and only
    /// have a lifetime matching their containing AppDomain.
    /// </summary>
    public class InMemoryCredentialStore : ICredentialStore
    {
        public virtual string? ConsumerKey { get; set; }

        public virtual string? ConsumerSecret { get; set; }

        public virtual string? OAuthToken { get; set; }

        public virtual string? OAuthTokenSecret { get; set; }

        public virtual string? ScreenName { get; set; }

        public virtual ulong UserID { get; set; }

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

        public virtual async Task LoadAsync()
        {
        }

        public virtual async Task StoreAsync() { }

        public virtual async Task ClearAsync()
        {
            ConsumerKey = null;
            ConsumerSecret = null;
            OAuthToken = null;
            OAuthTokenSecret = null;
            ScreenName = null;
            UserID = 0ul;
        }

#pragma warning restore 1998
    }
}
