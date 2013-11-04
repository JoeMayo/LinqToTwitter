namespace LinqToTwitter
{
    /// <summary>
    /// Maps the twitter-provided credentials to the in memory credentials 
    /// for in-memory use.
    /// </summary>
    public class SingleUserInMemoryCredentialStore : InMemoryCredentialStore
    {
        // The properties provided by Twitter are named AccessToken and
        // AccessTokenSecret, we need to map them to what the 
        // IOAuthCredentials interface uses.
        public virtual string AccessToken { get; set; }
        public virtual string AccessTokenSecret { get; set; }

        public override string OAuthToken
        {
            get
            {
                return AccessToken;
            }
            set
            {
                AccessToken = value;
            }
        }

        public override string OAuthTokenSecret {
            get
            {
                return AccessTokenSecret;
            }
            set
            {
                AccessTokenSecret = value;
            }
        }
    }
}
