namespace LinqToTwitter
{
    /// <summary>
    /// Holds credentials in memory. You must persist values 
    /// to save or work in stateless apps (such as Web apps).
    /// </summary>
    public class InMemoryCredentials : IOAuthCredentials
    {
        public virtual string ConsumerKey { get; set; }

        public virtual string ConsumerSecret { get; set; }

        public virtual string OAuthToken { get; set; }

        public virtual string AccessToken { get; set; }

        public virtual string ScreenName { get; set; }

        public virtual string UserId { get; set; }

        /// <summary>
        /// Sets ConsumerKey, ConsumerSecret, and AccessToken with a comma-separated 
        /// list. You must reassign credentials to Authorizer after changing them.
        /// </summary>
        /// <param name="credentialString"></param>
        public virtual void Load(string credentialString)
        {
            string[] credentials = credentialString.Split(',');

            ConsumerKey = credentials[0];
            ConsumerSecret = credentials[1];
            OAuthToken = credentials[2];
            AccessToken = credentials[3];
            ScreenName = credentials[4];
            UserId = credentials[5];
        }

        /// <summary>
        /// Gets a comma-separated list of ConsumerKey, ConsumerSecret, 
        /// and AccessToken that can be saved or serialized.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ConsumerKey + "," + ConsumerSecret + "," + OAuthToken + "," + AccessToken + "," + ScreenName + "," + UserId;
        }
    }
}
