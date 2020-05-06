namespace LinqToTwitter.Common
{
    public enum TweetMode
    {
        /// <summary>
        /// Traditional compatibility mode (default)
        /// </summary>
        Compat,

        /// <summary>
        /// New extended mode allows more characters in tweets with Mentions and URL suffix not counted.
        /// </summary>
        Extended
    }
}
