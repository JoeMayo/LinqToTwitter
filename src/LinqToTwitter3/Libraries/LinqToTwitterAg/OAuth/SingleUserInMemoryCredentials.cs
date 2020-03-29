namespace LinqToTwitter
{
    /// <summary>
    /// Maps the twitter-provided credentials to the in memory credentials 
    /// for in-memory use.
    /// </summary>
    public class SingleUserInMemoryCredentials : InMemoryCredentials
    {
        // The properties provided by Twitter are named AccessToken and
        // AccessTokenSecret, we need to map them to what the 
        // IOAuthCredentials interface uses.
        public virtual string TwitterAccessToken { get; set; }
        public virtual string TwitterAccessTokenSecret { get; set; }

        public override string OAuthToken
        {
            get
            {
                return TwitterAccessToken;
            }
            set
            {
                TwitterAccessToken = value;
            }
        }

        public override string AccessToken {
            get
            {
                return TwitterAccessTokenSecret;
            }
            set
            {
                TwitterAccessTokenSecret = value;
            }
        }
    }
}
