namespace LinqToTwitter
{
    /// <summary>
    /// type of direct message query
    /// </summary>
    public enum DirectMessageType
    {
        /// <summary>
        /// direct messages sent by a user
        /// </summary>
        SentBy,

        /// <summary>
        /// direct messages sent to a user
        /// </summary>
        SentTo,

        /// <summary>
        /// get a single direct message
        /// </summary>
        Show
    }
}
