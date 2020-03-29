namespace LinqToTwitter
{
    /// <summary>
    /// Type of welcome message query
    /// </summary>
    public enum WelcomeMessageType
    {
        /// <summary>
        /// All welcome messages
        /// </summary>
        ListMessages,

        /// <summary>
        /// All welcome message rules
        /// </summary>
        ListRules,

        /// <summary>
        /// A single welcome message
        /// </summary>
        ShowMessage,

        /// <summary>
        /// A single welcome message rule
        /// </summary>
        ShowRule
    }
}
