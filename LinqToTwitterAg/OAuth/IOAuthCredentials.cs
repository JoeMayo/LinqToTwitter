namespace LinqToTwitter
{
    /// <summary>
    /// Classes implementing this interface populate the
    /// ConsumerKey and Consumer secret that you can get
    /// from Twitter by registering an app at http://dev.twitter.com/apps.
    /// 
    /// LINQ to Twitter populates the AccessToken when
    /// the user authorizes your app.  Each user has a
    /// unique access token that can be stored and reused
    /// any time the same user uses your app again.
    /// 
    /// Custom implementations of this interface will save
    /// and retrieve credentials from their own unique data
    /// store.
    /// </summary>
    public interface IOAuthCredentials
    {
        /// <summary>
        /// Key provided by Twitter for your application
        /// </summary>
        string ConsumerKey { get; set; }

        /// <summary>
        /// Secret provided by Twitter for your application
        /// </summary>
        string ConsumerSecret { get; set; }

        /// <summary>
        /// Token provided by Twitter for making request
        /// </summary>
        string OAuthToken { get; set; }

        /// <summary>
        /// Unique access token for a user
        /// </summary>
        string AccessToken { get; set; }


        /// <summary>
        /// Twitter screen name
        /// </summary>
        string ScreenName { get; set; }

        /// <summary>
        /// Twitter user ID
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// Populates this with credential values
        /// </summary>
        /// <param name="credentialsString">comma separated string of ConsumerKey,ConsumerSecret,AccessToken</param>
        void Load(string credentialsString);

        /// <summary>
        /// Saves current credentials to storage
        /// </summary>
        void Save();

        /// <summary>
        /// Removes credentials from storage
        /// </summary>
        void Clear();
    }
}
