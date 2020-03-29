
namespace LinqToTwitter
{
    /// <summary>
    /// Type of result from processing an 
    /// asynchronous request to Twitter
    /// </summary>
    public enum TwitterErrorStatus
    {
        /// <summary>
        /// No error
        /// </summary>
        Success,

        /// <summary>
        /// Error received from Twitter
        /// </summary>
        TwitterApiError,

        /// <summary>
        /// Error detected by LINQ to Twitter
        /// while processing request
        /// </summary>
        RequestProcessingException,
    }
}
