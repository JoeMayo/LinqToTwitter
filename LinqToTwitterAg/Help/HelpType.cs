namespace LinqToTwitter
{
    public enum HelpType
    {
        /// <summary>
        /// Lets you send a quick test and see headers like the Twitter server time
        /// </summary>
        Test,

        /// <summary>
        /// Various settings such as image size, t.co url sizes, and more (should be cached and reused, but refreshed no more than once a day)
        /// </summary>
        Configuration,

        /// <summary>
        /// Languages supported by Twitter
        /// </summary>
        Languages
    }
}
